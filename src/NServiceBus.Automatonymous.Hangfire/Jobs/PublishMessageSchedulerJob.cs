using System.Threading.Tasks;
using Hangfire.Server;
using NServiceBus.UniformSession;

namespace NServiceBus.Automatonymous.Hangfire.Jobs
{
    /// <summary>
    /// The publish message scheduler job.
    /// </summary>
    public class PublishMessageSchedulerJob
    {
        private readonly IUniformSession _session;
        private readonly PerformContext _context;

        /// <summary>
        /// Initialize a new instance of <see cref="PublishMessageSchedulerJob"/>. 
        /// </summary>
        /// <param name="session">The <see cref="IUniformSession"/>.</param>
        /// <param name="context">The <see cref="PerformContext"/>.</param>
        public PublishMessageSchedulerJob(IUniformSession session, PerformContext context)
        {
            _session = session;
            _context = context;
        }

        /// <summary>
        /// Execute job.
        /// </summary>
        /// <param name="message">The message</param>
        /// <typeparam name="T">The message type.</typeparam>
        public async Task Execute<T>(T message)
        {
            var options = new PublishOptions();
            options.SetHeader(MessageHeaders.SchedulingTokenId, _context.BackgroundJob.Id);
            await _session.Publish(message, options).ConfigureAwait(false);
        }
    }
}