using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Automatonymous.Imp;
using NServiceBus.Automatonymous.Schedules.Internals;
using NServiceBus.UniformSession;

namespace NServiceBus.Automatonymous.Schedules
{
    /// <summary>
    /// The default implementation of <see cref="MessageSchedulerContext"/>.
    /// </summary>
    /// <remarks>
    /// It's using NServiceBus to handle the delays, so it's not possible cancel a scheduler message 
    /// </remarks>
    public class DefaultMessageSchedulerContext : MessageSchedulerContext
    {
        private readonly IMessageCreator _messageCreator;
        private readonly IUniformSession _session;

        /// <summary>
        /// Initialize a new instance of <see cref="DefaultMessageSchedulerContext"/>.
        /// </summary>
        /// <param name="session">The <see cref="IMessageSession"/>.</param>
        /// <param name="messageCreator">The <see cref="IMessageCreator"/>.</param>
        public DefaultMessageSchedulerContext(IUniformSession session, IMessageCreator messageCreator)
        {
            _session = session;
            _messageCreator = messageCreator;
        }

        /// <inheritdoc />
        public async Task<ScheduledMessage<T>> ScheduleSendAsync<T>(string destinationAddress, DateTime scheduledTime, T message,
            CancellationToken cancellationToken = default) where T : class
        {
            var id = Guid.NewGuid();
            
            var options = new SendOptions();
            options.DoNotDeliverBefore(scheduledTime);
            options.SetMessageId(id.ToString());
            options.SetHeader(MessageHeaders.SchedulingTokenId, id.ToString());
            options.RouteReplyTo(destinationAddress);
            
            var payload = _messageCreator.CreateInstance<T>();
            
            await _session.Send(payload, options).ConfigureAwait(false);
            return new DefaultScheduledMessage<T>(id, scheduledTime, payload);
        }

        /// <inheritdoc />
        public async Task<ScheduledMessage> ScheduleSendAsync(string destinationAddress, DateTime scheduledTime, object message,
            CancellationToken cancellationToken = default)
        {
            var id = Guid.NewGuid();
            
            var options = new SendOptions();
            options.DoNotDeliverBefore(scheduledTime);
            options.SetMessageId(id.ToString());
            options.SetHeader(MessageHeaders.SchedulingTokenId, id.ToString());
            options.RouteReplyTo(destinationAddress);
            
            await _session.Send(message, options).ConfigureAwait(false);
            return new DefaultScheduledMessage(id, scheduledTime, message, message.GetType());
        }

        /// <inheritdoc />
        public async Task<ScheduledMessage<T>> ScheduleSendAsync<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken = default) 
            where T : class
        {
            var id = Guid.NewGuid();
            
            var options = new SendOptions();
            options.DoNotDeliverBefore(scheduledTime);
            options.SetMessageId(id.ToString());
            options.RouteToThisEndpoint();
            options.SetHeader(MessageHeaders.SchedulingTokenId, id.ToString());
            
            var payload = _messageCreator.CreateInstance<T>();
            
            await _session.Send(payload, options).ConfigureAwait(false);
            return new DefaultScheduledMessage<T>(id, scheduledTime, payload);
        }

        /// <inheritdoc />
        public async Task<ScheduledMessage> ScheduleSendAsync(DateTime scheduledTime, object message, CancellationToken cancellationToken = default)
        {
            var id = Guid.NewGuid();
            
            var options = new SendOptions();
            options.DoNotDeliverBefore(scheduledTime);
            options.SetMessageId(id.ToString());
            options.SetHeader(MessageHeaders.SchedulingTokenId, id.ToString());
            
            await _session.Send(message, options).ConfigureAwait(false);
            return new DefaultScheduledMessage(id, scheduledTime, message, message.GetType());
        }
        
        /// <inheritdoc />
        public Task CancelScheduledSendAsync(Guid tokenId, CancellationToken cancellationToken = default) => Task.CompletedTask;

        /// <inheritdoc />
        public async Task<ScheduledMessage<T>> SchedulePublishAsync<T>(DateTime scheduledTime, T message, CancellationToken cancellationToken = default) where T : class
        {
            var id = Guid.NewGuid();
            
            var options = new SendOptions();
            options.DoNotDeliverBefore(scheduledTime);
            options.SetHeader(MessageHeaders.SchedulingTokenId, id.ToString());

            var delay = new PublishMessageWithDelay
            {
                Payload = System.Text.Json.JsonSerializer.Serialize(message),
                PayloadType = message.GetType()
            };
            
            await _session.Send(delay, options).ConfigureAwait(false);
            return new DefaultScheduledMessage<T>(id, scheduledTime, message);
        }

        /// <inheritdoc />
        public async Task<ScheduledMessage> SchedulePublishAsync(DateTime scheduledTime, object message, CancellationToken cancellationToken = default)
        {
            var id = Guid.NewGuid();
            
            var options = new SendOptions();
            options.DoNotDeliverBefore(scheduledTime);
            options.SetHeader(MessageHeaders.SchedulingTokenId, id.ToString());
            
            var delay = new PublishMessageWithDelay
            {
                Payload = System.Text.Json.JsonSerializer.Serialize(message),
                PayloadType = message.GetType()
            };
            
            await _session.Send(delay, options).ConfigureAwait(false);
            return new DefaultScheduledMessage(id, scheduledTime, message, message.GetType());
        }

        /// <inheritdoc />
        public Task CancelScheduledPublishAsync<T>(Guid tokenId, CancellationToken cancellationToken = default) 
            where T : class => Task.CompletedTask;
    }
}