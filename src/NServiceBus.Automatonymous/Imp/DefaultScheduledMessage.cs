using System;

namespace NServiceBus.Automatonymous.Imp
{
    /// <summary>
    /// Default implementation of <see cref="ScheduledMessage"/>
    /// </summary>
    public class DefaultScheduledMessage : ScheduledMessage
    {
        /// <summary>
        /// Initialize new instance of <see cref="DefaultScheduledMessage"/>.
        /// </summary>
        /// <param name="tokenId">The token id.</param>
        /// <param name="scheduledTime">The scheduler <see cref="DateTime"/>.</param>
        /// <param name="payload">The message payload.</param>
        /// <param name="payloadType">The message type.</param>
        public DefaultScheduledMessage(Guid tokenId, DateTime scheduledTime, object payload, Type payloadType)
        {
            TokenId = tokenId;
            ScheduledTime = scheduledTime;
            Payload = payload;
            PayloadType = payloadType;
        }

        /// <inheritdoc />
        public object Payload { get; }

        /// <inheritdoc />
        public Type PayloadType { get; }

        /// <inheritdoc />
        public Guid TokenId { get; }

        /// <inheritdoc />
        public DateTime ScheduledTime { get; }
    }
    
    /// <summary>
    /// Default implementation of <see cref="ScheduledMessage"/>
    /// </summary>
    public class DefaultScheduledMessage<T> : ScheduledMessage<T>
        where T : notnull
    {
        /// <summary>
        /// Initialize new instance of <see cref="DefaultScheduledMessage"/>.
        /// </summary>
        /// <param name="tokenId">The token id.</param>
        /// <param name="scheduledTime">The scheduler <see cref="DateTime"/>.</param>
        /// <param name="payload">The message payload.</param>
        public DefaultScheduledMessage(Guid tokenId, DateTime scheduledTime, T payload)
        {
            TokenId = tokenId;
            ScheduledTime = scheduledTime;
            Payload = payload;
        }

        /// <inheritdoc />
        public T Payload { get; }
        
        /// <inheritdoc />
        public Guid TokenId { get; }

        /// <inheritdoc />
        public DateTime ScheduledTime { get; }
    }
}