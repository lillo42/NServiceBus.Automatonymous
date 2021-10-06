using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous.Events;

/// <summary>
/// Implementation of <see cref="IEventCorrelationConfigurator{TState,TMessage}"/>.
/// </summary>
/// <typeparam name="TState">The state machine data.</typeparam>
/// <typeparam name="TMessage">The message.</typeparam>
public class NServiceBusEventCorrelationConfigurator<TState, TMessage> : IEventCorrelationConfigurator<TState, TMessage>
    where TState : class, IContainSagaData, new()
{
    /// <summary>
    /// Initialize <see cref="NServiceBusEventCorrelationConfigurator{TState,TMessage}"/>.
    /// </summary>
    /// <param name="correlateByProperty">The <see cref="Expression{TDelegate}"/> to correlate by property.</param>
    /// <param name="howToFindSagaWithMessage">The <see cref="Expression{TDelegate}"/> to find saga with message.</param>
    public NServiceBusEventCorrelationConfigurator(
        Expression<Func<TMessage, object>>? correlateByProperty,
        Expression<Func<TState, object>> howToFindSagaWithMessage)
    {
        _howToFindSagaWithMessage = howToFindSagaWithMessage;
        _correlateByProperty = correlateByProperty;
    }

    private Expression<Func<TMessage, object>>? _correlateByProperty;
        
    /// <inheritdoc />
    public IEventCorrelationConfigurator<TState, TMessage> CorrelateBy(Expression<Func<TMessage, object>> propertyExpression)
    {
        _correlateByProperty = propertyExpression;
        return this;
    }

    private Expression<Func<TState, object>> _howToFindSagaWithMessage;
        
    /// <inheritdoc />
    public IEventCorrelationConfigurator<TState, TMessage> HowToFindSagaData(Expression<Func<TState, object>> propertyExpression)
    {
        _howToFindSagaWithMessage = propertyExpression;
        return this;
    }

    private Func<TMessage, IMessageProcessingContext, Task>? _onMissingSaga;
        
    /// <inheritdoc />
    public IEventCorrelationConfigurator<TState, TMessage> OnMissingSaga(Action<IMissingInstanceConfigurator<TState, TMessage>> config)
    {
        var configurator = new EventMissingInstanceConfigurator<TState, TMessage>();
        config(configurator);
        _onMissingSaga = configurator.Build();
        return this;
    }
        
    /// <summary>
    /// Create <see cref="NServiceBusEventCorrelation{TState,TMessage}"/>.
    /// </summary>
    /// <returns>Return new instance of <see cref="NServiceBusEventCorrelation{TState,TMessage}"/>.</returns>
    public NServiceBusEventCorrelation<TState, TMessage> Build() =>
        new(_correlateByProperty, _howToFindSagaWithMessage, _onMissingSaga);
}