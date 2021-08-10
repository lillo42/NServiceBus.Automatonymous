using System;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous
{
    /// <summary>
    /// Configure the action when saga is not found.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMissingInstanceConfigurator<TState, out TMessage>
        where TState :  IContainSagaData
    {
        /// <summary>
        /// Discard the event, silently ignoring the missing instance for the event
        /// </summary>
        void Discard();
        
        /// <summary>
        /// Fault the saga consumer, which moves the message to the error queue
        /// </summary>
        void Fault();
        
        /// <summary>
        /// Execute an asynchronous method when the instance is missed, allowing a custom behavior to be specified.
        /// </summary>
        /// <param name="action">The <see cref="Action"/>.</param>
        void ExecuteAsync(Func<TMessage, IMessageProcessingContext, Task> action);
        
        /// <summary>
        /// Execute a method when the instance is missed, allowing a custom behavior to be specified.
        /// </summary>
        /// <param name="action">The <see cref="Action"/>.</param>
        void Execute(Action<TMessage, IMessageProcessingContext> action);
    }
}