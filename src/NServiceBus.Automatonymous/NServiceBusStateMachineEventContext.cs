using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Contexts;
using GreenPipes;
using GreenPipes.Payloads;

namespace NServiceBus.Automatonymous
{
    /// <summary>
    /// The Event context.
    /// </summary>
    /// <typeparam name="TStateMachineData">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message data.</typeparam>
    public class NServiceBusStateMachineEventContext<TStateMachineData, TMessage> : BasePipeContext, EventContext<TStateMachineData, TMessage>
        where TStateMachineData : class
    {
        /// <inheritdoc />
        public TStateMachineData Instance { get; }
        
        /// <inheritdoc />
        public Event<TMessage> Event { get; }
        
        /// <inheritdoc />
        public TMessage Data { get; }
        
        Event EventContext<TStateMachineData>.Event => Event;

        private readonly StateMachine<TStateMachineData> _machine;
        
        /// <summary>
        /// Initialize new instance of <see cref="NServiceBusStateMachineEventContext{TStateMachineData,TMessage}"/>.
        /// </summary>
        /// <param name="machine">The <see cref="StateMachine{TInstance}"/>.</param>
        /// <param name="instance">The state machine data.</param>
        /// <param name="event">The <see cref="Event{TMessage}"/>.</param>
        /// <param name="data">The message data.</param>
        /// <param name="payloadCache">The <see cref="IPayloadCache"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        public NServiceBusStateMachineEventContext(StateMachine<TStateMachineData> machine, 
            TStateMachineData instance, 
            Event<TMessage> @event,
            TMessage data,
            IPayloadCache payloadCache,
            CancellationToken cancellationToken)
            : base(payloadCache, cancellationToken)
        {
            _machine = machine;
            Instance = instance;
            Event = @event;
            Data = data;
        }

        /// <summary>
        /// Not implement, please use the generic version.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">Always throw</exception>
        [DoesNotReturn]
        public Task Raise(Event @event)
            => throw new NotImplementedException();

        /// <inheritdoc />
        public Task Raise<TData>(Event<TData> @event, TData data)
        {
            var eventContext = new EventContextProxy<TStateMachineData>(this, @event);
            return _machine.RaiseEvent(eventContext);
        }
    }
}