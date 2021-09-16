using System;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous
{
    /// <summary>
    /// A message scheduler is able to schedule a message for delivery.
    /// </summary>
    public interface IMessageScheduler
    {
        /// <summary>
        /// Scheduler a send a message
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> ScheduleSendAsync<T>(string destinationAddress, DateTime scheduledTime, T message, CancellationToken cancellationToken = default)
            where T : class;
        
        /// <summary>
        /// Scheduler a send message.
        /// </summary>
        /// <param name="destinationAddress">The destination address where the schedule message should be sent.</param>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue.</param>
        /// <param name="message">The message object.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage> ScheduleSendAsync(string destinationAddress, DateTime scheduledTime, object message, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Cancel a scheduled message by TokenId.
        /// </summary>
        /// <param name="tokenId">The tokenId of the scheduled message.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        Task CancelScheduledSendAsync(Guid tokenId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Scheduler a publish a message
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue.</param>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage<T>> SchedulePublishAsync<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken = default)
            where T : class;
        
        /// <summary>
        /// Scheduler a publish an object as a message, using the type of the message instance.
        /// </summary>
        /// <param name="scheduledTime">The time at which the message should be delivered to the queue</param>
        /// <param name="message">The message object</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task which is completed once the Send is acknowledged by the broker</returns>
        Task<ScheduledMessage> SchedulePublishAsync(DateTime scheduledTime, object message, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Cancel a scheduled publish, using the tokenId. The message type <typeparamref name="T" /> is used to determine
        /// the destinationAddress.
        /// </summary>
        /// <param name="tokenId">The tokenId of the scheduled message</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        Task CancelScheduledPublishAsync<T>(Guid tokenId, CancellationToken cancellationToken = default)
            where T : class;
    }
}