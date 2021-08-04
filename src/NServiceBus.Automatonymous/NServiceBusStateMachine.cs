using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Automatonymous;
using GreenPipes.Internals.Extensions;
using NServiceBus.Automatonymous.Events;
using NServiceBus.Automatonymous.Extensions;

namespace NServiceBus.Automatonymous
{
    public abstract class NServiceBusStateMachine<TState> : AutomatonymousStateMachine<TState>
        where TState : class, IContainSagaData, new()
    {
        private readonly Dictionary<Event, IEventCorrelation> _correlations = new Dictionary<Event, IEventCorrelation>();
        private readonly Dictionary<Type, Event> _events = new Dictionary<Type, Event>();

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

        protected override void Event<T>(Expression<Func<Event<T>>> propertyExpression) 
            => Event(propertyExpression, x => { });

        protected void Event<T>(Expression<Func<Event<T>>> propertyExpression,
            Action<IEventCorrelationConfigurator<TState, T>> configureEventCorrelation)
        {
            base.Event(propertyExpression);

            var propertyInfo = propertyExpression.GetPropertyInfo();
            var @event = (Event<T>) propertyInfo.GetValue(this)!;
            RegisterEvent(@event, configureEventCorrelation);
        }

        private void RegisterEvent<T>(Event<T> @event, 
            Action<IEventCorrelationConfigurator<TState, T>>? configureEventCorrelation)
        {
            var correlateByProperty = GetDefaultCorrelationMessageByProperty<T>();
            if (typeof(T).IsAssignableFrom(typeof(ICorrelatedBy)))
            {
                correlateByProperty =
                    (Expression<Func<T, object>>) typeof(NServiceBusStateMachine<TState>).GetMethod(
                            nameof(CorrelatedByExpression))!
                        .MakeGenericMethod(typeof(T))
                        .Invoke(null, null);
            }

            var configurator = new NServiceBusEventCorrelationConfigurator<TState, T>(correlateByProperty, CorrelationByProperty());
            configureEventCorrelation?.Invoke(configurator);
            _correlations[@event] = configurator.Build();
        }

        private static Expression<Func<TMessage, object>> CorrelatedByExpression<TMessage>()
            where TMessage : ICorrelatedBy 
            => x => x.CorrelationId!;

        public abstract Expression<Func<TState, object>> CorrelationByProperty();

        protected virtual string DefaultCorrelationMessageByPropertyName { get; } = string.Empty;

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
            if (correlationProperty != null)
            {
                return correlationProperty;
            }

            return null;
        }

        internal IEventCorrelation GetCorrelations(Type eventType) 
            => _correlations[_events[eventType]];
    }
}
