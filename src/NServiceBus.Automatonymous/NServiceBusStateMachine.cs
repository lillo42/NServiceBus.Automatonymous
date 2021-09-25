using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Automatonymous;
using GreenPipes;
using GreenPipes.Internals.Extensions;
using NServiceBus.Automatonymous.Events;
using NServiceBus.Automatonymous.Events.Imp;
using NServiceBus.Automatonymous.Extensions;
using NServiceBus.Automatonymous.Schedules;
using NServiceBus.Logging;

namespace NServiceBus.Automatonymous
{
    /// <summary>
    /// The base <see cref="AutomatonymousStateMachine{TInstance}"/>.
    /// </summary>
    /// <typeparam name="TState">The saga data.</typeparam>
    public abstract class NServiceBusStateMachine<TState> : AutomatonymousStateMachine<TState>
        where TState : class, IContainSagaData, new()
    {
        private readonly Dictionary<Event, IEventCorrelation> _correlations = new();
        private readonly Dictionary<Type, Event> _events = new();

        /// <summary>
        /// Initialize new <see cref="NServiceBusStateMachine{TState}" />.
        /// </summary>
        protected NServiceBusStateMachine()
        {
            RegisterAllEventImplicit();
        }
        
        private void RegisterAllEventImplicit()
        {
            var registerEvent = typeof(NServiceBusStateMachine<TState>).GetMethod(nameof(RegisterEvent), BindingFlags.Instance | BindingFlags.NonPublic)!;
            var type = GetType().GetTypeInfo();
            var properties = ConfigurationHelpers.GetStateMachineProperties(type);

            foreach (var property in properties)
            {
                var propertyType = property.PropertyType.GetTypeInfo();
                if (!propertyType.IsGenericType)
                {
                    continue;
                }

                if (!propertyType.ClosesType(typeof(Event<>), out var args))
                {
                    continue;
                }

                var @event = (Event?) property.GetValue(this);
                if (@event == null)
                {
                    continue;
                }

                _events[args[0]] = @event;
                registerEvent
                    .MakeGenericMethod(args[0])
                    .Invoke(this, new[] {@event, (object) null!});
            }
        }
        
        /// <summary>
        /// The <see cref="IEventCorrelation"/>.
        /// </summary>
        public IEnumerable<IEventCorrelation> Correlations
        {
            get
            {
                foreach (var @event in Events)
                {
                    if (_correlations.TryGetValue(@event, out var correlation))
                    {
                        yield return correlation;
                    }
                }
            }
        }

        /// <summary>
        /// Declares a data event on the state machine, and initializes the property.
        /// </summary>
        /// <param name="propertyExpression">The <see cref="Expression{T}"/> to get property expression.</param>
        /// <typeparam name="T">The message.</typeparam>
        protected override void Event<T>(Expression<Func<Event<T>>> propertyExpression) 
            => Event(propertyExpression, _ => { });

        /// <summary>
        /// Declares a data event on the state machine, and initializes the property.
        /// </summary>
        /// <param name="propertyExpression">The <see cref="Expression{T}" /> to get property expression.</param>
        /// <param name="configureEventCorrelation">The configuration of <see cref="IEventCorrelationConfigurator{TState,TMessage}" />.</param>
        /// <typeparam name="T">The message.</typeparam>
        protected void Event<T>(Expression<Func<Event<T>>> propertyExpression,
            Action<IEventCorrelationConfigurator<TState, T>> configureEventCorrelation)
        {
            base.Event(propertyExpression);

            var propertyInfo = propertyExpression.GetPropertyInfo();
            var @event = (Event<T>) propertyInfo.GetValue(this)!;
            RegisterEvent(@event, configureEventCorrelation);
        }
        
        /// <summary>
        /// Declares an Event on the state machine with the specified data type, and allows the correlation of the event
        /// to be configured.
        /// </summary>
        /// <typeparam name="T">The event data type</typeparam>
        /// <typeparam name="TProperty">The property type</typeparam>
        /// <param name="propertyExpression">The containing property</param>
        /// <param name="eventPropertyExpression">The event property expression</param>
        /// <param name="configureEventCorrelation">Configuration callback for the event</param>
        protected void Event<TProperty, T>(Expression<Func<TProperty>> propertyExpression, 
            Expression<Func<TProperty, Event<T>>> eventPropertyExpression,
            Action<IEventCorrelationConfigurator<TState, T>> configureEventCorrelation)
            where TProperty : class
        {
            base.Event(propertyExpression, eventPropertyExpression);

            var propertyInfo = propertyExpression.GetPropertyInfo();
            var property = (TProperty)propertyInfo.GetValue(this)!;

            var eventPropertyInfo = eventPropertyExpression.GetPropertyInfo();
            var @event = (Event<T>)eventPropertyInfo.GetValue(property)!;

            RegisterEvent(@event, configureEventCorrelation);
        }


        private void RegisterEvent<T>(Event<T> @event, 
            Action<IEventCorrelationConfigurator<TState, T>>? configureEventCorrelation)
        {
            var correlateByProperty = GetDefaultCorrelationMessageByProperty<T>();
            if (typeof(T).IsAssignableFrom(typeof(ICorrelatedBy)))
            {
                correlateByProperty = (Expression<Func<T, object>>) typeof(NServiceBusStateMachine<TState>).GetMethod(nameof(CorrelatedByExpression))!
                    .MakeGenericMethod(typeof(T))
                    .Invoke(null, null)!;
            }

            var configurator = new NServiceBusEventCorrelationConfigurator<TState, T>(correlateByProperty, CorrelationByProperty());
            configureEventCorrelation?.Invoke(configurator);
            _correlations[@event] = configurator.Build();
        }

        private static Expression<Func<TMessage, object>> CorrelatedByExpression<TMessage>()
            where TMessage : ICorrelatedBy 
            => x => x.CorrelationId!;

        /// <summary>
        /// The saga correlation property.
        /// Used to find saga.
        /// </summary>
        /// <returns>The saga correlation property.</returns>
        public abstract Expression<Func<TState, object>> CorrelationByProperty();

        /// <summary>
        /// Default property name in message.
        /// </summary>
        protected virtual string DefaultCorrelationMessageByPropertyName => string.Empty;

        private Expression<Func<TMessage, object>>? GetDefaultCorrelationMessageByProperty<TMessage>()
        {
            if (DefaultCorrelationMessageByPropertyName == string.Empty)
            {
                return null;
            }
            var correlationProperty = GetCorrelationProperty<TMessage>();
            if (correlationProperty == null)
            {
                return null;
            }
            var parameterExpression = Expression.Parameter(typeof(TMessage));
            var propertyExpression = Expression.Property(parameterExpression, correlationProperty);
            var convert = Expression.Convert(propertyExpression, typeof(object));
            return Expression.Lambda<Func<TMessage, object>>(convert, parameterExpression);
        }

        private PropertyInfo? GetCorrelationProperty<TMessage>()
        {
            var correlationProperty = typeof(TMessage)
                .GetProperty(DefaultCorrelationMessageByPropertyName, BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
            return correlationProperty != null ? correlationProperty : null;
        }

        internal IEventCorrelation GetCorrelations(Type eventType) 
            => _correlations[_events[eventType]];
        
        /// <summary>
        /// Declares a schedule placeholder that is stored with the state machine instance.
        /// </summary>
        /// <typeparam name="TMessage">The request type.</typeparam>
        /// <param name="propertyExpression">The schedule property on the state machine.</param>
        /// <param name="tokenIdExpression">The property where the tokenId is stored.</param>
        /// <param name="configureSchedule">The callback to configure the schedule.</param>
        protected void Schedule<TMessage>(Expression<Func<Schedule<TState, TMessage>>> propertyExpression,
            Expression<Func<TState, Guid?>> tokenIdExpression,
            Action<IScheduleConfigurator<TState, TMessage>>? configureSchedule = null)
            where TMessage : class, IMessage
        {
            var configurator = new StateMachineScheduleConfigurator<TState, TMessage>();

            configureSchedule?.Invoke(configurator);

            Schedule(propertyExpression, tokenIdExpression, configurator.Settings);
        }
        
        /// <summary>
        /// Declares a schedule placeholder that is stored with the state machine instance.
        /// </summary>
        /// <typeparam name="TMessage">The scheduled message type.</typeparam>
        /// <param name="propertyExpression">The schedule property on the state machine.</param>
        /// <param name="tokenIdExpression">The property where the tokenId is stored.</param>
        /// <param name="settings">The request settings (which can be read from configuration, etc.).</param>
        protected void Schedule<TMessage>(Expression<Func<Schedule<TState, TMessage>>> propertyExpression,
            Expression<Func<TState, Guid?>> tokenIdExpression,
            IScheduleSettings<TState, TMessage> settings)
            where TMessage : class, IMessage
        {
            var property = propertyExpression.GetPropertyInfo();
            var name = property.Name;

            var schedule = new StateMachineSchedule<TState, TMessage>(name, tokenIdExpression, settings);

            InitializeSchedule(this, property, schedule);
            
            Event(propertyExpression, x => x.Received);
            
            Event(propertyExpression, x => x.AnyReceived, x => settings.Received?.Invoke(x));

            DuringAny(
            When(schedule.AnyReceived)
                .ThenAsync(async context =>
                {
                    var tokenId = schedule.GetTokenId(context.Instance);
                    if (context.TryGetPayload(out IMessageHandlerContext handler))
                    {
                        var messageTokenId = handler.GetSchedulingTokenId();
                        if(messageTokenId.HasValue && (!tokenId.HasValue || messageTokenId.Value != tokenId.Value))
                        {
                            context.GetPayload<ILog>()
                                .DebugFormat("SAGA: {0} Scheduled message not current: {1}", handler.MessageHeaders[Headers.SagaId], messageTokenId.Value);
                            return;
                        }
                    }

                    var eventContext = context.GetProxy(schedule.Received, context.Data);
                    await ((StateMachine<TState>)this).RaiseEvent(eventContext).ConfigureAwait(false);
                    if (schedule.GetTokenId(context.Instance) == tokenId)
                    {
                        schedule.SetTokenId(context.Instance, default);
                    }
                }));
            
            static void InitializeSchedule(NServiceBusStateMachine<TState> stateMachine, PropertyInfo property, Schedule<TState, TMessage> schedule)
            {
                if (property.CanWrite)
                {
                    property.SetValue(stateMachine, schedule);
                }
                else if (ConfigurationHelpers.TryGetBackingField(stateMachine.GetType().GetTypeInfo(), property, out var backingField))
                {
                    backingField.SetValue(stateMachine, schedule);
                }
                else
                {
                    throw new ArgumentException($"The schedule property is not writable: {property.Name}");
                }
            }
        }
    }
}
