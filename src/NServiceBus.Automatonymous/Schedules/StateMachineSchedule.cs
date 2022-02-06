using System;
using System.Linq.Expressions;
using Automatonymous;
using GreenPipes.Internals.Extensions;
using GreenPipes.Internals.Reflection;

namespace NServiceBus.Automatonymous.Schedules;

/// <summary>
/// The scheduler state machine.
/// </summary>
/// <typeparam name="TInstance">The state machine data.</typeparam>
/// <typeparam name="TMessage">The scheduler message.</typeparam>
public class StateMachineSchedule<TInstance, TMessage> : Schedule<TInstance, TMessage>
    where TInstance : class, IContainSagaData
    where TMessage : class, IMessage
{
    private readonly IScheduleSettings<TInstance, TMessage> _settings;
    private readonly ReadWriteProperty<TInstance, Guid?> _tokenIdProperty;
        
    /// <summary>
    /// Initialize new instance of <see cref="StateMachineSchedule{TInstance,TMessage}"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="tokenIdExpression">The token id expression</param>
    /// <param name="settings">The setting.</param>
    public StateMachineSchedule(string name, Expression<Func<TInstance, Guid?>> tokenIdExpression, IScheduleSettings<TInstance, TMessage> settings)
    {
        Name = name;
        _settings = settings;
        _tokenIdProperty = new ReadWriteProperty<TInstance, Guid?>(tokenIdExpression.GetPropertyInfo());
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public Event<TMessage> Received { get; set; } = null!;

    /// <inheritdoc />
    public Event<TMessage> AnyReceived { get; set; } = null!;

    /// <inheritdoc />
    public TimeSpan GetDelay(BehaviorContext<TInstance> context) => _settings.DelayProvider(context);

    /// <inheritdoc />
    public Guid? GetTokenId(TInstance data) => _tokenIdProperty.Get(data);

    /// <inheritdoc />
    public void SetTokenId(TInstance data, Guid? tokenId) => _tokenIdProperty.SetProperty(data, tokenId);
}