using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;

namespace NServiceBus.Automatonymous.Activities
{
    /// <summary>
    /// The <see cref="Activity{TInstance}"/> to replay a message.
    /// </summary>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The origin message type.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public class ReplayActivity<TInstance, TData, TMessage> : Activity<TInstance, TData>
        where TInstance : class, IContainSagaData
        where TMessage : class
    {
        
        private readonly Action<BehaviorContext<TInstance, TData>, ReplyOptions>? _configureOptions;
        private readonly Func<BehaviorContext<TInstance, TData>, TMessage>? _messageFactory;
        private readonly Func<BehaviorContext<TInstance, TData>, Task<TMessage>>? _asyncMessageFactory;
        
        /// <summary>
        /// Initialize new instance of <see cref="ReplayActivity{TInstance,TData,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="configureOptions">The <see cref="Action{T1,T2}"/> to configure <see cref="SendOptions"/>.</param>
        public ReplayActivity(Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory, 
            Action<BehaviorContext<TInstance, TData>, ReplyOptions>? configureOptions)
        {
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            _configureOptions = configureOptions;
        }
        
        /// <summary>
        /// Initialize new instance of <see cref="ReplayActivity{TInstance,TData,TMessage}"/>.
        /// </summary>
        /// <param name="asyncMessageFactory">The async factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="configureOptions">The <see cref="Action{T1,T2}"/> to configure <see cref="SendOptions"/>.</param>
        public ReplayActivity(Func<BehaviorContext<TInstance, TData>, Task<TMessage>> asyncMessageFactory, 
            Action<BehaviorContext<TInstance, TData>, ReplyOptions>? configureOptions)
        {
            _asyncMessageFactory = asyncMessageFactory ?? throw new ArgumentNullException(nameof(asyncMessageFactory));
            _configureOptions = configureOptions;
        }
        
        /// <inheritdoc />
        public void Probe(ProbeContext context)
            => context.CreateScope("response");

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
        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next) 
            where TException : Exception
            => next.Faulted(context);
        
        private async  Task Execute(BehaviorContext<TInstance, TData> context)
        {
            var message = _messageFactory?.Invoke(context) ?? await _asyncMessageFactory!(context).ConfigureAwait(false);
            var options = new ReplyOptions();

            _configureOptions?.Invoke(context, options);
            await context.GetPayload<IMessageHandlerContext>().Reply(message, options).ConfigureAwait(false);
        }
    }
}