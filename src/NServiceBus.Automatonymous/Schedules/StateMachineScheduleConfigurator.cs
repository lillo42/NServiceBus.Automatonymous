using System;
using Automatonymous;
using NServiceBus.Automatonymous.Events;

namespace NServiceBus.Automatonymous.Schedules;

/// <summary>
/// The StateMachineScheduleConfigurator.
/// </summary>
/// <typeparam name="TInstance">The state.</typeparam>
/// <typeparam name="TMessage">The message</typeparam>
public class StateMachineScheduleConfigurator<TInstance, TMessage> : IScheduleConfigurator<TInstance, TMessage>, IScheduleSettings<TInstance, TMessage>
    where TInstance : class, IContainSagaData
    where TMessage : class, IMessage
{
    /// <summary>
    /// Initialize new instance of <see cref="StateMachineScheduleConfigurator{TInstance,TMessage}"/>.
    /// </summary>
    public StateMachineScheduleConfigurator()
    {
        Delay = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// The <see cref="IScheduleSettings{TInstance,TMessage}"/>.
    /// </summary>
    public IScheduleSettings<TInstance, TMessage> Settings => this;

    /// <inheritdoc />
    public TimeSpan Delay
    {
        set => DelayProvider = _ => value;
    }

    /// <inheritdoc cref="IScheduleConfigurator{TInstance, TMessage}"/>
    public Func<BehaviorContext<TInstance>, TimeSpan> DelayProvider { get; set; } = _ => TimeSpan.FromSeconds(30);

    /// <inheritdoc cref="IScheduleConfigurator{TInstance, TMessage}"/>
    public Action<IEventCorrelationConfigurator<TInstance, TMessage>>? Received { get; set; }
}