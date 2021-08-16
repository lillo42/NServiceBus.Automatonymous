using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;

namespace NServiceBus.Automatonymous.Activities
{
    /// <summary>
    /// The <see cref="Activity{TInstance}"/> To send message
    /// </summary>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public class SendActivity<TInstance, TMessage> : Activity<TInstance>
        where TInstance : class, IContainSagaData
        where TMessage : class
    {
        private readonly Action<BehaviorContext<TInstance>, SendOptions>? _configureOptions;
        private readonly Func<BehaviorContext<TInstance>, TMessage>? _messageFactory;
        private readonly Func<BehaviorContext<TInstance>, Task<TMessage>>? _asyncMessageFactory;

        /// <summary>
        /// Initialize new instance of <see cref="SendActivity{TInstance,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="configureOptions">The <see cref="Action{T1,T2}"/> to configure <see cref="SendOptions"/>.</param>
        public SendActivity(Func<BehaviorContext<TInstance>, TMessage> messageFactory, 
            Action<BehaviorContext<TInstance>, SendOptions>? configureOptions)
        {
            _messageFactory = messageFactory;
            _configureOptions = configureOptions;
        }

        /// <summary>
        /// Initialize new instance of <see cref="SendActivity{TInstance,TMessage}"/>.
        /// </summary>
        /// <param name="asyncMessageFactory">The async factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="configureOptions">The <see cref="Action{T1,T2}"/> to configure <see cref="SendOptions"/>.</param>
        public SendActivity(Func<BehaviorContext<TInstance>, Task<TMessage>> asyncMessageFactory, 
            Action<BehaviorContext<TInstance>, SendOptions>? configureOptions)
        {
            _asyncMessageFactory = asyncMessageFactory;
            _configureOptions = configureOptions;
        }

        /// <inheritdoc />
        public void Probe(ProbeContext context) => context.CreateScope("send");

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

        /// <inheritdoc />
        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TException> context, Behavior<TInstance> next) 
            where TException : Exception
            => next.Faulted(context);

        /// <inheritdoc />
        public Task Faulted<T, TException>(BehaviorExceptionContext<TInstance, T, TException> context, Behavior<TInstance, T> next) 
            where TException : Exception 
            => next.Faulted(context);

        private async  Task Execute(BehaviorContext<TInstance> context)
        {
            var message = _messageFactory?.Invoke(context) ?? await _asyncMessageFactory!(context).ConfigureAwait(false);
            var options = new SendOptions();
            
            _configureOptions?.Invoke(context, options);
            await context.GetPayload<IMessageHandlerContext>().Send(message, options).ConfigureAwait(false);
        }
    }
    
    /// <summary>
    /// The <see cref="Activity{TInstance}"/> To send message
    /// </summary>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The origin message type.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public class SendActivity<TInstance, TData, TMessage> : Activity<TInstance, TData>
        where TInstance : class, IContainSagaData
        where TMessage : class
    {
        private readonly Action<BehaviorContext<TInstance, TData>, SendOptions>? _configureOptions;
        private readonly Func<BehaviorContext<TInstance, TData>, TMessage>? _messageFactory;
        private readonly Func<BehaviorContext<TInstance, TData>, Task<TMessage>>? _asyncMessageFactory;

        /// <summary>
        /// Initialize new instance of <see cref="SendActivity{TInstance,TData,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="configureOptions">The <see cref="Action{T1,T2}"/> to configure <see cref="SendOptions"/>.</param>
        public SendActivity(Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory, 
            Action<BehaviorContext<TInstance, TData>, SendOptions>? configureOptions)
        {
            _messageFactory = messageFactory;
            _configureOptions = configureOptions;
        }
        
        /// <summary>
        /// Initialize new instance of <see cref="SendActivity{TInstance,TData,TMessage}"/>.
        /// </summary>
        /// <param name="asyncMessageFactory">The async factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="configureOptions">The <see cref="Action{T1,T2}"/> to configure <see cref="SendOptions"/>.</param>
        public SendActivity(Func<BehaviorContext<TInstance, TData>, Task<TMessage>> asyncMessageFactory, 
            Action<BehaviorContext<TInstance, TData>, SendOptions>? configureOptions)
        {
            _asyncMessageFactory = asyncMessageFactory;
            _configureOptions = configureOptions;
        }

        /// <inheritdoc />
        public void Probe(ProbeContext context) => context.CreateScope("send");

        /// <inheritdoc />
        public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);
        
        /// <inheritdoc />
        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next) where TException : Exception 
            => next.Faulted(context);

        /// <inheritdoc />
        public async Task Execute(BehaviorContext<TInstance, TData> context, Behavior<TInstance, TData> next)
        {
            await Execute(context).ConfigureAwait(false);
            await next.Execute(context).ConfigureAwait(false);
        }

        private async  Task Execute(BehaviorContext<TInstance, TData> context)
        {
            var message = _messageFactory?.Invoke(context) ?? await _asyncMessageFactory!(context).ConfigureAwait(false);
            var options = new SendOptions();
            
            _configureOptions?.Invoke(context, options);
            await context.GetPayload<IMessageHandlerContext>().Send(message, options).ConfigureAwait(false);
        }
    }
}