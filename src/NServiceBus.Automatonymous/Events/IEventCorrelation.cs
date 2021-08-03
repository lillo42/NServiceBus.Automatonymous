using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous.Events
{
    public interface IEventCorrelation<TState, TMessage> : IEventCorrelation
        where TState : class
    {
        object? IEventCorrelation.CorrelateByProperty => CorrelateByProperty;
        new Expression<Func<TMessage, object>>? CorrelateByProperty { get; }
        
        object IEventCorrelation.HowToFindSagaWithMessage => HowToFindSagaWithMessage;
        new Expression<Func<TState, object>> HowToFindSagaWithMessage { get; }

        Func<object, IMessageProcessingContext, Task>? IEventCorrelation.OnMissingSaga
        {
            get
            {
                if (OnMissingSaga == null)
                {
                    return null;
                }

                return (message, context) => OnMissingSaga((TMessage) message, context);
            }
        }
        new Func<TMessage, IMessageProcessingContext, Task>? OnMissingSaga { get; }
    }

    public interface IEventCorrelation
    {
        Type MessageType { get; }
        
        object? CorrelateByProperty { get; }
        object HowToFindSagaWithMessage { get; }
        Func<object, IMessageProcessingContext, Task>? OnMissingSaga { get; }
    }
}