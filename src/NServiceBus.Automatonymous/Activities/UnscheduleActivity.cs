using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;
using NServiceBus.Automatonymous.Extensions;

namespace NServiceBus.Automatonymous.Activities
{
    /// <summary>
    /// The <see cref="Activity{TInstance}"/> to unschedule message
    /// </summary>
    /// <typeparam name="TInstance"></typeparam>
    public class UnscheduleActivity<TInstance> : Activity<TInstance>
        where TInstance : class, IContainSagaData
    {
        private readonly Schedule<TInstance> _schedule;

        /// <summary>
        /// Initialize a new instance of <see cref="UnscheduleActivity{TInstance}"/>.
        /// </summary>
        /// <param name="schedule">The <see cref="Schedule{TInstance}"/>.</param>
        public UnscheduleActivity(Schedule<TInstance> schedule)
        {
            _schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
        }

        /// <inheritdoc />
        public void Probe(ProbeContext context) => context.CreateScope("unschedule");

        /// <inheritdoc />
        public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);

        /// <inheritdoc />
        public async Task Execute(BehaviorContext<TInstance> context, Behavior<TInstance> next)
        {
            await Execute(context).ConfigureAwait(false);
            await next.Execute(context).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task Execute<T>(BehaviorContext<TInstance, T> context, Behavior<TInstance, T> next)
        {
            await Execute(context).ConfigureAwait(false);
            await next.Execute(context).ConfigureAwait(false);
        }

        private async Task Execute(BehaviorContext<TInstance> context)
        {
            var previousToken = _schedule.GetTokenId(context.Instance);
            if (previousToken.HasValue && context.TryGetPayload(out IMessageHandlerContext messageHandlerContext))
            {
                var messageTokenId = messageHandlerContext.GetSchedulingTokenId();
                if (!messageTokenId.HasValue || previousToken.Value != messageTokenId.Value)
                {
                    var schedulerContext = context.GetPayload<MessageSchedulerContext>();
                    await schedulerContext.CancelScheduledSendAsync(previousToken.Value).ConfigureAwait(false);
                    _schedule.SetTokenId(context.Instance, null);
                }
            }
        }

        /// <inheritdoc />
        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next) 
            where TException : Exception => next.Faulted(context);

        /// <inheritdoc />
        public Task Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next) 
            where TException : Exception => next.Faulted(context);
    }
}