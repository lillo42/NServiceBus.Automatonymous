using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;

namespace NServiceBus.Automatonymous.Activities
{
    /// <summary>
    /// The <see cref="Activity{TInstance}"/> to schedule message
    /// </summary>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public class ScheduleActivity<TInstance, TMessage> : Activity<TInstance>
        where TInstance : class, IContainSagaData
        where TMessage : class
    {
        private readonly Schedule<TInstance> _schedule;
        private readonly Func<BehaviorContext<TInstance>, DateTime> _timeProvider;
        private readonly Func<BehaviorContext<TInstance>, TMessage>? _messageFactory;
        private readonly Func<BehaviorContext<TInstance>, Task<TMessage>>? _asyncMessageFactory;
        
        /// <summary>
        /// Initialize new instance of <see cref="ScheduleActivity{TInstance,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="schedule">The <see cref="Schedule{TData,TMessage}"/>.</param>
        /// <param name="timeProvider">The func to retriever <see cref="DateTime"/>.</param>
        public ScheduleActivity(Func<BehaviorContext<TInstance>, TMessage> messageFactory, Schedule<TInstance> schedule,
            Func<BehaviorContext<TInstance>, DateTime> timeProvider)
            : this(schedule, timeProvider)
        {
            _messageFactory = messageFactory;
        }

        /// <summary>
        /// Initialize new instance of <see cref="ScheduleActivity{TInstance,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="schedule">The <see cref="Schedule{TData,TMessage}"/>.</param>
        /// <param name="timeProvider">The func to retriever <see cref="DateTime"/>.</param>
        public ScheduleActivity(Func<BehaviorContext<TInstance>, Task<TMessage>>messageFactory, Schedule<TInstance> schedule,
            Func<BehaviorContext<TInstance>, DateTime> timeProvider)
            : this(schedule, timeProvider)
        {
            _asyncMessageFactory = messageFactory;
        }

        private ScheduleActivity(Schedule<TInstance> schedule, Func<BehaviorContext<TInstance>, DateTime> timeProvider)
        {
            _schedule = schedule;
            _timeProvider = timeProvider;
        }
        
        /// <inheritdoc />
        public void Probe(ProbeContext context) 
            => context.CreateScope("schedule");

        /// <inheritdoc />
        public void Accept(StateMachineVisitor visitor)
            => visitor.Visit(this);

        /// <inheritdoc />
        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next) 
            where TException : Exception
            => next.Faulted(context);

        /// <inheritdoc />
        public Task Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next) 
            where TException : Exception
            => next.Faulted(context);

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
            var previousTokenId = _schedule.GetTokenId(context.Instance);
            var schedulerContext = context.GetPayload<MessageSchedulerContext>();
            var scheduledTime = _timeProvider(context);
            var message = _messageFactory?.Invoke(context) ?? await _asyncMessageFactory!(context).ConfigureAwait(false);

            var scheduledMessage = await schedulerContext.ScheduleSendAsync(scheduledTime, message).ConfigureAwait(false);
            _schedule.SetTokenId(context.Instance, scheduledMessage.TokenId);

            if (previousTokenId.HasValue)
            {
                await schedulerContext.CancelScheduledSendAsync(previousTokenId.Value).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// The <see cref="Activity{TInstance}"/> to schedule message
    /// </summary>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <typeparam name="TData">The origin message type.</typeparam>
    public class ScheduleActivity<TInstance, TData, TMessage> : Activity<TInstance, TData>
        where TInstance : class, IContainSagaData
        where TMessage : class
    {
        private readonly Schedule<TInstance> _schedule;
        private readonly Func<BehaviorContext<TInstance, TData>, DateTime> _timeProvider;
        private readonly Func<BehaviorContext<TInstance, TData>, TMessage>? _messageFactory;
        private readonly Func<BehaviorContext<TInstance, TData>, Task<TMessage>>? _asyncMessageFactory;
        
        /// <summary>
        /// Initialize new instance of <see cref="ScheduleActivity{TInstance,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="schedule">The <see cref="Schedule{TData,TMessage}"/>.</param>
        /// <param name="timeProvider">The func to retriever <see cref="DateTime"/>.</param>
        public ScheduleActivity(Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory, Schedule<TInstance> schedule,
            Func<BehaviorContext<TInstance, TData>, DateTime> timeProvider)
            : this(schedule, timeProvider)
        {
            _messageFactory = messageFactory;
        }

        /// <summary>
        /// Initialize new instance of <see cref="ScheduleActivity{TInstance,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="schedule">The <see cref="Schedule{TData,TMessage}"/>.</param>
        /// <param name="timeProvider">The func to retriever <see cref="DateTime"/>.</param>
        public ScheduleActivity(Func<BehaviorContext<TInstance, TData>, Task<TMessage>>messageFactory, Schedule<TInstance> schedule,
            Func<BehaviorContext<TInstance, TData>, DateTime> timeProvider)
            : this(schedule, timeProvider)
        {
            _asyncMessageFactory = messageFactory;
        }

        private ScheduleActivity(Schedule<TInstance> schedule, Func<BehaviorContext<TInstance, TData>, DateTime> timeProvider)
        {
            _schedule = schedule;
            _timeProvider = timeProvider;
        }
        
        /// <inheritdoc />
        public void Probe(ProbeContext context) 
            => context.CreateScope("schedule");

        /// <inheritdoc />
        public void Accept(StateMachineVisitor visitor)
            => visitor.Visit(this);
        
        /// <inheritdoc />
        public async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            await Execute(context).ConfigureAwait(false);
            await next.Execute(context).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next) where TException : Exception
            => next.Faulted(context);
        

        private async Task Execute(BehaviorContext<TInstance, TData> context)
        {
            var previousTokenId = _schedule.GetTokenId(context.Instance);
            var schedulerContext = context.GetPayload<MessageSchedulerContext>();
            var scheduledTime = _timeProvider(context);
            var message = _messageFactory?.Invoke(context) ?? await _asyncMessageFactory!(context).ConfigureAwait(false);

            var scheduledMessage = await schedulerContext.ScheduleSendAsync(scheduledTime, message).ConfigureAwait(false);
            _schedule.SetTokenId(context.Instance, scheduledMessage.TokenId);

            if (previousTokenId.HasValue)
            {
                await schedulerContext.CancelScheduledSendAsync(previousTokenId.Value).ConfigureAwait(false);
            }
        }
    }
}