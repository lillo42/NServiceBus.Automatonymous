using System;
using System.Linq.Expressions;

namespace NServiceBus.Automatonymous.Events
{
    public interface IEventCorrelationConfigurator<TState, TMessage>
        where TState : class, IContainSagaData
    {
        IEventCorrelationConfigurator<TState, TMessage> CorrelateBy(Expression<Func<TMessage, object>> propertyExpression);
        IEventCorrelationConfigurator<TState, TMessage> HowToFindSagaWithMessage(Expression<Func<TState, object>> propertyExpression);
        
        IEventCorrelationConfigurator<TState, TMessage> OnMissingSaga(Action<IMissingInstanceConfigurator<TState, TMessage>> config);
    }
}