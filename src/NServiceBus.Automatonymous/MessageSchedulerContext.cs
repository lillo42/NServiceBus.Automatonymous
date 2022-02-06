using System;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous;

/// <summary>
/// The Message scheduler context
/// </summary>
// ReSharper disable once InconsistentNaming
public interface MessageSchedulerContext : IMessageScheduler
{
    /// <summary>
    /// Scheduler a send a message.
    /// </summary>
    /// <typeparam name="T">The message type.</typeparam>
    /// <param name="scheduledTime">The time at which the message should be delivered to the queue.</param>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The task which is completed once the Send is acknowledged by the broker.</returns>
    Task<ScheduledMessage<T>> ScheduleSendAsync<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken = default)
        where T : class;
        
    /// <summary>
    /// Scheduler a send a message.
    /// </summary>
    /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
    Task<ScheduledMessage> ScheduleSendAsync(DateTime scheduledTime, object message, CancellationToken cancellationToken = default);

}