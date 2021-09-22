using System;

namespace NServiceBus.Automatonymous.Schedules.Internals
{
    /// <summary>
    /// The message publish. 
    /// </summary>
    public class PublishMessageWithDelay : IMessage
    {
        /// <summary>
        /// The payload. 
        /// </summary>
        public string Payload { get; set; } = default!;

        /// <summary>
        /// The payload type.
        /// </summary>
        public Type PayloadType { get; set; } = default!;
    }
}