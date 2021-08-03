using System;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous.Events
{
    public class EventMissingInstanceConfigurator<TState , TMessage> : IMissingInstanceConfigurator<TState, TMessage> 
        where TState :  IContainSagaData
    {
        private Func<TMessage, IMessageProcessingContext, Task>? _action;
        public void Discard()
        {
            _action = (message, context) => Task.CompletedTask;
        }

        public void Fault()
        {
            _action = (message, context) => Task.CompletedTask;
        }

        public void ExecuteAsync(Func<TMessage, IMessageProcessingContext, Task> action)
        {
            _action = action;
        }

        public void Execute(Action<TMessage, IMessageProcessingContext> action)
        {
            _action = (message, context) =>
            {
                action(message, context);
                return Task.CompletedTask;
            };
        }

        public Func<TMessage, IMessageProcessingContext, Task>? Build()
            => _action;
    }
}