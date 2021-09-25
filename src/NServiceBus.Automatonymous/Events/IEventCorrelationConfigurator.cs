using System;
using System.Linq.Expressions;

namespace NServiceBus.Automatonymous.Events
{
    /// <summary>
    /// The event correlation configurator. 
    /// </summary>
    /// <typeparam name="TState">The saga state.</typeparam>
    /// <typeparam name="TMessage">The message.</typeparam>
    public interface IEventCorrelationConfigurator<TState, TMessage>
        where TState : class, IContainSagaData
    {
        /// <summary>
        /// Correlate to the saga instance by CorrelationId, using the id from the event data.
        /// </summary>
        /// <param name="propertyExpression">The CorrelationId from the event data.</param>
        /// <returns>The <see cref="IEventCorrelationConfigurator{TState,TMessage}"/>.</returns>
        IEventCorrelationConfigurator<TState, TMessage> CorrelateBy(Expression<Func<TMessage, object>> propertyExpression);

        /// <summary>
        /// Correlate to the saga instance by header, using the id from the event data.
        /// </summary>
        /// <param name="header">The correlation header.</param>
        /// <returns>The <see cref="IEventCorrelationConfigurator{TState,TMessage}"/>.</returns>
        IEventCorrelationConfigurator<TState, TMessage> CorrelateByHeader(string header);
        
        /// <summary>
        /// Configure how the <typeparamref name="TMessage"/> will find saga's data.
        /// </summary>
        /// <param name="propertyExpression">An <see cref="Expression{TDelegate}" /> that represents the message.</param>
        /// <returns>The <see cref="IEventCorrelationConfigurator{TState,TMessage}"/>.</returns>
        IEventCorrelationConfigurator<TState, TMessage> HowToFindSagaData(Expression<Func<TState, object>> propertyExpression);
        
        
        /// <summary>
        /// If an event is consumed that is not matched to an existing saga instance, discard the event without throwing an exception.
        /// The default behavior is to throw an exception, which moves the event into the error queue for later processing
        /// </summary>
        /// <param name="config">The configuration call to specify the behavior on missing instance.</param>
        /// <returns>The <see cref="IEventCorrelationConfigurator{TState,TMessage}"/>.</returns>
        IEventCorrelationConfigurator<TState, TMessage> OnMissingSaga(Action<IMissingInstanceConfigurator<TState, TMessage>> config);
    }
}