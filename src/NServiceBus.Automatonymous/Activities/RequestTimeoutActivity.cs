using System;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;

namespace NServiceBus.Automatonymous.Activities
{
    /// <summary>
    /// The <see cref="Activity{TInstance}"/> to request timeout message
    /// </summary>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public class RequestTimeoutActivity<TInstance, TMessage> : Activity<TInstance>
        where TInstance : class, IContainSagaData
        where TMessage : class
    {
        private readonly Func<BehaviorContext<TInstance>, TMessage>? _messageFactory;
        private readonly Func<BehaviorContext<TInstance>, Task<TMessage>>? _asyncMessageFactory;

        private readonly DateTime? _at;
        private readonly TimeSpan? _within;

        /// <summary>
        /// Initialize new instance of <see cref="RequestTimeoutActivity{TInstance,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="at">The <see cref="DateTime"/> to request time.</param>
        public RequestTimeoutActivity(Func<BehaviorContext<TInstance>, TMessage> messageFactory, DateTime at)
        {
            if (at.Kind == DateTimeKind.Unspecified)
            {
                throw new InvalidOperationException("Kind property of DateTime 'at' must be specified.");
            }
            
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            _at = at;
        }
        
        /// <summary>
        /// Initialize new instance of <see cref="RequestTimeoutActivity{TInstance,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="within">The <see cref="TimeSpan"/> to request time.</param>
        public RequestTimeoutActivity(Func<BehaviorContext<TInstance>, TMessage> messageFactory, TimeSpan within)
        {
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            _within = within;
        }

        /// <summary>
        /// Initialize new instance of <see cref="RequestTimeoutActivity{TInstance,TMessage}"/>.
        /// </summary>
        /// <param name="asyncMessageFactory">The async factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="at">The <see cref="DateTime"/> to request time.</param>
        public RequestTimeoutActivity(Func<BehaviorContext<TInstance>, Task<TMessage>> asyncMessageFactory, DateTime at)
        {
            if (at.Kind == DateTimeKind.Unspecified)
            {
                throw new InvalidOperationException("Kind property of DateTime 'at' must be specified.");
            }
            
            _asyncMessageFactory = asyncMessageFactory ?? throw new ArgumentNullException(nameof(asyncMessageFactory));
            _at = at;
        }
        
        /// <summary>
        /// Initialize new instance of <see cref="RequestTimeoutActivity{TInstance,TMessage}"/>.
        /// </summary>
        /// <param name="asyncMessageFactory">The async factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="within">The <see cref="TimeSpan"/> to request time.</param>
        public RequestTimeoutActivity(Func<BehaviorContext<TInstance>, Task<TMessage>> asyncMessageFactory, TimeSpan within)
        {
            _asyncMessageFactory = asyncMessageFactory ?? throw new ArgumentNullException(nameof(asyncMessageFactory));
            _within = within;
        }

        /// <inheritdoc />
        public void Probe(ProbeContext context) => context.CreateScope("request-timeout");

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

            if (_at.HasValue)
            {
                options.DoNotDeliverBefore(_at.Value);
            }
            else
            {
                options.DelayDeliveryWith(_within.GetValueOrDefault());
            }
            
            options.RouteToThisEndpoint();
            options.SetHeader(Headers.SagaId, context.Instance.Id.ToString());
            options.SetHeader(Headers.IsSagaTimeoutMessage, bool.TrueString);
            options.SetHeader(Headers.SagaType, context.GetPayload<SagaType>().Saga.AssemblyQualifiedName);
            await context.GetPayload<IMessageHandlerContext>().Send(message, options).ConfigureAwait(false);
        }
    }
    
    /// <summary>
    /// The <see cref="Activity{TInstance}"/> to request timeout message
    /// </summary>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The origin message type.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    public class RequestTimeoutActivity<TInstance, TData, TMessage> : Activity<TInstance, TData>
        where TInstance : class, IContainSagaData
        where TMessage : class
    {
        private readonly Func<BehaviorContext<TInstance, TData>, TMessage>? _messageFactory;
        private readonly Func<BehaviorContext<TInstance, TData>, Task<TMessage>>? _asyncMessageFactory;
        
        private readonly DateTime? _at;
        private readonly TimeSpan? _within;

        /// <summary>
        /// Initialize new instance of <see cref="RequestTimeoutActivity{TInstance,TData,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="at">The <see cref="DateTime"/> to request time.</param>
        public RequestTimeoutActivity(Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory, DateTime at)
        {
            if (at.Kind == DateTimeKind.Unspecified)
            {
                throw new InvalidOperationException("Kind property of DateTime 'at' must be specified.");
            }
            
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            _at = at;
        }
        
        /// <summary>
        /// Initialize new instance of <see cref="RequestTimeoutActivity{TInstance,TData,TMessage}"/>.
        /// </summary>
        /// <param name="messageFactory">The sync factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="within">The <see cref="TimeSpan"/> to request time.</param>
        public RequestTimeoutActivity(Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory, TimeSpan within)
        {
            _messageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
            _within = within;
        }

        /// <summary>
        /// Initialize new instance of <see cref="RequestTimeoutActivity{TInstance,TData,TMessage}"/>.
        /// </summary>
        /// <param name="asyncMessageFactory">The async factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="at">The <see cref="DateTime"/> to request time.</param>
        public RequestTimeoutActivity(Func<BehaviorContext<TInstance, TData>, Task<TMessage>> asyncMessageFactory, DateTime at)
        {
            if (at.Kind == DateTimeKind.Unspecified)
            {
                throw new InvalidOperationException("Kind property of DateTime 'at' must be specified.");
            }
            
            _asyncMessageFactory = asyncMessageFactory ?? throw new ArgumentNullException(nameof(asyncMessageFactory));
            _at = at;
        }
        
        /// <summary>
        /// Initialize new instance of <see cref="RequestTimeoutActivity{TInstance,TData,TMessage}"/>.
        /// </summary>
        /// <param name="asyncMessageFactory">The async factory of <typeparamref name="TInstance"/>.</param>
        /// <param name="within">The <see cref="TimeSpan"/> to request time.</param>
        public RequestTimeoutActivity(Func<BehaviorContext<TInstance, TData>, Task<TMessage>> asyncMessageFactory, TimeSpan within)
        {
            _asyncMessageFactory = asyncMessageFactory ?? throw new ArgumentNullException(nameof(asyncMessageFactory));
            _within = within;
        }

        /// <inheritdoc />
        public void Probe(ProbeContext context) => context.CreateScope("request-timeout");

        /// <inheritdoc />
        public void Accept(StateMachineVisitor visitor) => visitor.Visit(this);
        
        /// <inheritdoc />
        public Task Faulted<TException>(BehaviorExceptionContext<TInstance, TData, TException> context, Behavior<TInstance, TData> next) 
            where TException : Exception 
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
            
            if (_at.HasValue)
            {
                options.DoNotDeliverBefore(_at.Value);
            }
            else
            {
                options.DelayDeliveryWith(_within.GetValueOrDefault());
            }
            
            options.RouteToThisEndpoint();
            options.SetHeader(Headers.SagaId, context.Instance.Id.ToString());
            options.SetHeader(Headers.IsSagaTimeoutMessage, bool.TrueString);
            options.SetHeader(Headers.SagaType, context.GetPayload<SagaType>().Saga.AssemblyQualifiedName);
            await context.GetPayload<IMessageHandlerContext>().Send(message, options).ConfigureAwait(false);
        }
    }
}