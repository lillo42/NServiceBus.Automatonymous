using System.Threading.Tasks;

namespace NServiceBus.Automatonymous.Hangfire.Jobs
{
    /// <summary>
    /// The publish message scheduler job.
    /// </summary>
    public class PublishMessageSchedulerJob
    {
        private readonly IMessageSession _session;

        /// <summary>
        /// Initialize a new instance of <see cref="PublishMessageSchedulerJob"/>. 
        /// </summary>
        /// <param name="session"></param>
        public PublishMessageSchedulerJob(IMessageSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Execute job.
        /// </summary>
        /// <param name="message">The message</param>
        /// <typeparam name="T">The message type.</typeparam>
        public async Task Execute<T>(T message)
        {
            var options = new PublishOptions();
            await _session.Publish(message, options).ConfigureAwait(false);
        }
    }
}