using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous.Events;

/// <summary>
/// Implementation of <see cref="IEventCorrelation{TState,TMessage}"/>.
/// </summary>
/// <typeparam name="TState">The state machine data.</typeparam>
/// <typeparam name="TMessage">The message.</typeparam>
public class NServiceBusEventCorrelation <TState, TMessage> : IEventCorrelation<TState, TMessage>
    where TState : class
{
    /// <summary>
    /// Initialize <see cref="NServiceBusEventCorrelation{TState,TMessage}"/>.
    /// </summary>
    /// <param name="correlateByProperty">The default <see cref="Expression{TDelegate}"/> correlate by property.</param>
    /// <param name="howToFindSagaWithMessage">The default <see cref="Expression{TDelegate}"/> how to find saga with message.</param>
    /// <param name="onMissingSaga">The default <see cref="Func{T1,TResult}"/> on missing saga.</param>
    public NServiceBusEventCorrelation(Expression<Func<TMessage, object>>? correlateByProperty, 
        Expression<Func<TState, object>> howToFindSagaWithMessage,
        Func<TMessage, IMessageProcessingContext, Task>? onMissingSaga)
    {
        CorrelateByProperty = correlateByProperty;
        OnMissingSaga = onMissingSaga;
        HowToFindSagaWithMessage = howToFindSagaWithMessage;
    }

    /// <inheritdoc />
    public Expression<Func<TMessage, object>>? CorrelateByProperty { get; }

    /// <inheritdoc />
    public Expression<Func<TState, object>> HowToFindSagaWithMessage { get; }

    /// <inheritdoc />
    public Func<TMessage, IMessageProcessingContext, Task>? OnMissingSaga { get; }

    /// <inheritdoc />
    public Type MessageType => typeof(TMessage);
}