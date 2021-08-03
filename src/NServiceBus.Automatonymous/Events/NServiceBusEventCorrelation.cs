using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous.Events
{
    public class NServiceBusEventCorrelation <TState, TMessage> : IEventCorrelation<TState, TMessage>
        where TState : class
    {
        public NServiceBusEventCorrelation(Expression<Func<TMessage, object>>? correlateByProperty, 
            Expression<Func<TState, object>> howToFindSagaWithMessage,
            Func<TMessage, IMessageProcessingContext, Task>? onMissingSaga)
        {
            CorrelateByProperty = correlateByProperty;
            OnMissingSaga = onMissingSaga;
            HowToFindSagaWithMessage = howToFindSagaWithMessage;
        }

        public Expression<Func<TMessage, object>>? CorrelateByProperty { get; }

        public Expression<Func<TState, object>> HowToFindSagaWithMessage { get; }

        public Func<TMessage, IMessageProcessingContext, Task>? OnMissingSaga { get; }

        public Type MessageType => typeof(TMessage);
    }
}