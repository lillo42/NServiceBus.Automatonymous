using System;

namespace NServiceBus.Automatonymous
{
    /// <summary>
    /// The scheduler message.
    /// </summary>
    /// <typeparam name="T">The scheduler message.</typeparam>
    // ReSharper disable once InconsistentNaming
    public interface ScheduledMessage<out T> : ScheduledMessage
        where T : notnull
    {
        /// <summary>
        /// The message payload.
        /// </summary>
        new T Payload { get; }
        
        object ScheduledMessage.Payload => Payload;
        Type ScheduledMessage.PayloadType => typeof(T);
    }
    
    
    /// <summary>
    /// The scheduler message.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public interface ScheduledMessage
    {
        /// <summary>
        /// The message payload.
        /// </summary>
        object Payload { get; }
        
        /// <summary>
        /// The message type.
        /// </summary>
        Type PayloadType { get; }
        
        /// <summary>
        /// The message token.
        /// </summary>
        Guid TokenId { get; }
        
        /// <summary>
        /// The scheduled time.
        /// </summary>
        DateTime ScheduledTime { get; }
    }
}