using System.Threading;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Contexts;
using GreenPipes;
using GreenPipes.Payloads;

namespace NServiceBus.Automatonymous
{
    public class NServiceBusStateMachineEventContext<TInstance, TMessage> : BasePipeContext, EventContext<TInstance, TMessage>
        where TInstance : class
    {
        public TInstance Instance { get; }
        public Event<TMessage> Event { get; }
        public TMessage Data { get; }
        
        Event EventContext<TInstance>.Event => Event;

        private readonly StateMachine<TInstance> _machine;
        public NServiceBusStateMachineEventContext(StateMachine<TInstance> machine, 
            TInstance instance, 
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
        
        public Task Raise(Event @event)
        {
            var eventContext = new EventContextProxy<TInstance>(this, @event);
            return _machine.RaiseEvent(eventContext);
        }

        public Task Raise<TData>(Event<TData> @event, TData data)
        {
            var eventContext = new EventContextProxy<TInstance>(this, @event);
            return _machine.RaiseEvent(eventContext);
        }
    }
}