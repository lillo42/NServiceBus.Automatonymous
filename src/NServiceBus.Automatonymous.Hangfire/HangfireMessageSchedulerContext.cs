using System;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using NServiceBus.Automatonymous.Hangfire.Jobs;
using NServiceBus.Automatonymous.Imp;

namespace NServiceBus.Automatonymous.Hangfire
{
    /// <summary>
    /// The hangfire implementation of message scheduler.
    /// </summary>
    public class HangfireMessageSchedulerContext : MessageSchedulerContext
    {
        private IMessageHandlerContext _context;

        /// <summary>
        /// Initialize new instance of <see cref="HangfireMessageSchedulerContext"/>.
        /// </summary>
        /// <param name="wrapper">The <see cref="MessageHandlerContextWrapper"/>.</param>
        /// <exception cref="ArgumentException"></exception>
        public HangfireMessageSchedulerContext(MessageHandlerContextWrapper wrapper)
        {
            _context = wrapper.MessageHandlerContext ?? throw new ArgumentNullException(nameof(wrapper));
        }
        
        /// <inheritdoc />
        public Task<ScheduledMessage<T>> ScheduleSendAsync<T>(string destinationAddress, DateTime scheduledTime, T message, CancellationToken cancellationToken = default) where T : class
        {
            var jobId = BackgroundJob.Schedule<SendMessageSchedulerJob>(x => x.Execute(message, destinationAddress), scheduledTime);
            return Task.FromResult<ScheduledMessage<T>>(new DefaultScheduledMessage<T>(Guid.Parse(jobId), scheduledTime, message));
        }

        /// <inheritdoc />
        public Task<ScheduledMessage> ScheduleSendAsync(string destinationAddress, DateTime scheduledTime, object message, CancellationToken cancellationToken = default)
        {
            var jobId = BackgroundJob.Schedule<SendMessageSchedulerJob>(x => x.Execute(message, destinationAddress), scheduledTime);
            return Task.FromResult<ScheduledMessage>(new DefaultScheduledMessage(Guid.Parse(jobId), scheduledTime, message, message.GetType()));
        }
        
        /// <inheritdoc />
        public Task<ScheduledMessage<T>> ScheduleSendAsync<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken = default) where T : class
        {
            var jobId = BackgroundJob.Schedule<SendMessageSchedulerJob>(x => x.Execute(message, _context.ReplyToAddress), scheduledTime);
            return Task.FromResult<ScheduledMessage<T>>(new DefaultScheduledMessage<T>(Guid.Parse(jobId), scheduledTime, message));
        }

        /// <inheritdoc />
        public Task<ScheduledMessage> ScheduleSendAsync(DateTime scheduledTime, object message, CancellationToken cancellationToken = default)
        {
            var jobId = BackgroundJob.Schedule<SendMessageSchedulerJob>(x => x.Execute(message, _context.ReplyToAddress), scheduledTime);
            return Task.FromResult<ScheduledMessage>(new DefaultScheduledMessage(Guid.Parse(jobId), scheduledTime, message, message.GetType()));
        }

        /// <inheritdoc />
        public Task CancelScheduledSendAsync(Guid tokenId, CancellationToken cancellationToken = default)
        {
            BackgroundJob.Delete(tokenId.ToString());
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<ScheduledMessage<T>> SchedulePublishAsync<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken = default) 
            where T : class
        {
            var jobId = BackgroundJob.Schedule<PublishMessageSchedulerJob>(x => x.Execute(message), scheduledTime);
            return Task.FromResult<ScheduledMessage<T>>(new DefaultScheduledMessage<T>(Guid.Parse(jobId), scheduledTime, message));
        }

        /// <inheritdoc />
        public Task<ScheduledMessage> SchedulePublishAsync(DateTime scheduledTime, object message, CancellationToken cancellationToken = default)
        {
            var jobId = BackgroundJob.Schedule<PublishMessageSchedulerJob>(x => x.Execute(message), scheduledTime);
            return Task.FromResult<ScheduledMessage>(new DefaultScheduledMessage(Guid.Parse(jobId), scheduledTime, message, message.GetType()));
        }

        /// <inheritdoc />
        public Task CancelScheduledPublishAsync<T>(Guid tokenId, CancellationToken cancellationToken = default) 
            where T : class
        {
            BackgroundJob.Delete(tokenId.ToString());
            return Task.CompletedTask;
        }
    }
}