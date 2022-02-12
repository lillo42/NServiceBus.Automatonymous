using System;
using System.Threading.Tasks;
using NServiceBus.Automatonymous.Exceptions;
using NServiceBus.Settings;

namespace NServiceBus.Automatonymous.Events;

/// <summary>
/// Implementation of <see cref="IMissingInstanceConfigurator{TState,TMessage}"/>.
/// </summary>
/// <typeparam name="TState">The state machine data.</typeparam>
/// <typeparam name="TMessage">The message.</typeparam>
public class EventMissingInstanceConfigurator<TState , TMessage> : IMissingInstanceConfigurator<TState, TMessage> 
    where TState :  IContainSagaData
{
    private Func<TMessage, IMessageProcessingContext, Task>? _action;
        
    /// <inheritdoc />
    public void Discard()
    {
        _action = (_, _) => Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Fault()
    {
        _action = (_, context) =>
        {
            if (!context.Extensions.Get<ReadOnlySettings>().TryGet<string>(ErrorQueueSettings.SettingsKey, out var value))
            {
                throw new DeadQueueNotSetupException();
            }
                
            return context.ForwardCurrentMessageTo(value);
        };
    }

    /// <inheritdoc />
    public void ExecuteAsync(Func<TMessage, IMessageProcessingContext, Task> action)
    {
        _action = action;
    }

    /// <inheritdoc />
    public void Execute(Action<TMessage, IMessageProcessingContext> action)
    {
        _action = (message, context) =>
        {
            action(message, context);
            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// The <see cref="Func{T1,T2,TResult}"/> to be executed when saga is missing.
    /// </summary>
    /// <returns>The <see cref="Func{T1,T2,TResult}"/>.</returns>
    public Func<TMessage, IMessageProcessingContext, Task>? Build()
        => _action;
}