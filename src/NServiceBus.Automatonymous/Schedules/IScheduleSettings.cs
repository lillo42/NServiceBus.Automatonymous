using System;
using Automatonymous;
using NServiceBus.Automatonymous.Events;

namespace NServiceBus.Automatonymous.Schedules
{
    /// <summary>
    /// The schedule settings, including the default delay for the message.
    /// </summary>
    public interface IScheduleSettings<TInstance, TMessage>
        where TInstance : class, IContainSagaData
        where TMessage : class
    {
        /// <summary>
        /// Provides the delay for the message.
        /// </summary>
        Func<BehaviorContext<TInstance>, TimeSpan> DelayProvider { get; }
        
        /// <summary>
        /// Configure the received correlation.
        /// </summary>
        Action<IEventCorrelationConfigurator<TInstance, TMessage>>? Received { get; }
    }
}