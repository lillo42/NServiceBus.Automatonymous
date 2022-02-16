using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous.Events;

/// <summary>
/// The event configuration.
/// </summary>
/// <typeparam name="TState">The state machine data.</typeparam>
/// <typeparam name="TMessage">The message.</typeparam>
public interface IEventCorrelation<TState, TMessage> : IEventCorrelation
    where TState : class, IContainSagaData
{
    object? IEventCorrelation.CorrelateByProperty => CorrelateByProperty;
        
    /// <summary>
    /// The <see cref="Expression{TDelegate}"/> how to correlate the message.
    /// </summary>
    new Expression<Func<TMessage, object>>? CorrelateByProperty { get; }
        
    object IEventCorrelation.ToSaga => ToSaga;
        
    /// <summary>
    /// The <see cref="Expression{TDelegate}"/> how to find a saga with message. 
    /// </summary>
    new Expression<Func<TState, object>> ToSaga { get; }

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
        
    /// <summary>
    /// The <see cref="Func{T1,T2,TResult}"/> to be executed when saga is missing.
    /// </summary>
    new Func<TMessage, IMessageProcessingContext, Task>? OnMissingSaga { get; }


    void IEventCorrelation.Map(object mapper) => Map((SagaPropertyMapper<TState>)mapper);
        
    /// <summary>
    /// Execute the configured map.
    /// </summary>
    /// <param name="mapper">The <see cref="SagaPropertyMapper{TSagaData}"/>.</param>
    void Map(SagaPropertyMapper<TState> mapper);
}

/// <summary>
/// The event configuration.
/// </summary>
public interface IEventCorrelation
{
    /// <summary>
    /// The message <see cref="Type"/>.
    /// </summary>
    Type MessageType { get; }
        
        
    /// <summary>
    /// The <see cref="Expression{TDelegate}"/> how to correlate the message.
    /// </summary>
    object? CorrelateByProperty { get; }
        
    /// <summary>
    /// The header to correlate the message.
    /// </summary>
    string? CorrelateByHeader { get; }
        
    /// <summary>
    /// The <see cref="Expression{TDelegate}"/> how to find a saga with message. 
    /// </summary>
    object ToSaga { get; }
        
    /// <summary>
    /// The <see cref="Func{T1,T2,TResult}"/> to be executed when saga is missing.
    /// </summary>
    Func<object, IMessageProcessingContext, Task>? OnMissingSaga { get; }

    /// <summary>
    /// Execute the configured map.
    /// </summary>
    /// <param name="mapper">The <see cref="SagaPropertyMapper{TSagaData}"/>.</param>
    void Map(object mapper);
}