using System;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous
{
    public interface IMissingInstanceConfigurator<TState, out TMessage>
        where TState :  IContainSagaData
    {
        void Discard();
        void Fault();
        void ExecuteAsync(Func<TMessage, IMessageProcessingContext, Task> action);
        void Execute(Action<TMessage, IMessageProcessingContext> action);

    }
}