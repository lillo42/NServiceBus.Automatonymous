using System.Threading.Tasks;

namespace NServiceBus.Automatonymous.Hangfire.Jobs
{
    /// <summary>
    /// The send message scheduler job.
    /// </summary>
    public class SendMessageSchedulerJob
    {
        private readonly IMessageSession _session;

        /// <summary>
        /// Initialize a new instance of <see cref="SendMessageSchedulerJob"/>. 
        /// </summary>
        /// <param name="session"></param>
        public SendMessageSchedulerJob(IMessageSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Execute job.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="destinationAddress">The destiny address</param>
        /// <typeparam name="T">The message type.</typeparam>
        public async Task Execute<T>(T message, string? destinationAddress)
        {
            var options = new SendOptions();
            if (!string.IsNullOrEmpty(destinationAddress))
            {
                options.RouteReplyTo(destinationAddress);
            }
            
            await _session.Send(message, options).ConfigureAwait(false);
        }
    }
}