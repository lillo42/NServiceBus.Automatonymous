using System.Threading.Tasks;
using NServiceBus.Logging;

namespace NServiceBus.Automatonymous.Schedules.Internals
{
    /// <summary>
    /// The <see cref="PublishMessageWithDelay"/> handler.
    /// </summary>
    public class PublishMessageWithDelayHandler : IHandleMessages<PublishMessageWithDelay>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PublishMessageWithDelayHandler));
        /// <inheritdoc />
        public async Task Handle(PublishMessageWithDelay message, IMessageHandlerContext context)
        {
            var payload = System.Text.Json.JsonSerializer.Deserialize(message.Payload, message.PayloadType);
            if (payload == null)
            {
                Log.WarnFormat("Unable to deserialize '{0}' to '{1}'", message.Payload, message.PayloadType);
            }
            else
            {
                var opt = new PublishOptions();
                opt.SetHeader(MessageHeaders.SchedulingTokenId, context.MessageHeaders[MessageHeaders.SchedulingTokenId]);
                opt.SetMessageId(context.MessageHeaders[MessageHeaders.SchedulingTokenId]);
                await context.Publish(payload, opt);
            }
        }
    }
}