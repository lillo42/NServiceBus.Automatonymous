using System.Threading.Tasks;
using Hangfire.Server;
using NServiceBus.UniformSession;

namespace NServiceBus.Automatonymous.Hangfire.Jobs
{
    /// <summary>
    /// The send message scheduler job.
    /// </summary>
    public class SendMessageSchedulerJob
    {
        private readonly IUniformSession _session;
        private readonly PerformContext _context;

        /// <summary>
        /// Initialize a new instance of <see cref="SendMessageSchedulerJob"/>. 
        /// </summary>
        /// <param name="session">The <see cref="IUniformSession"/>.</param>
        /// <param name="context">The <see cref="PerformContext"/>.</param>
        public SendMessageSchedulerJob(IUniformSession session, PerformContext context)
        {
            _session = session;
            _context = context;
        }

        /// <summary>
        /// Execute job.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="destinationAddress">The destiny address.</param>
        /// <typeparam name="T">The message type.</typeparam>
        public async Task Execute<T>(T message, string destinationAddress)
        {
            var options = new SendOptions();
            options.RouteReplyTo(destinationAddress);
            options.RouteReplyToAnyInstance();
            
            options.SetHeader(MessageHeaders.SchedulingTokenId, _context.BackgroundJob.Id);
            await _session.Send(message, options).ConfigureAwait(false);
        }
    }
}