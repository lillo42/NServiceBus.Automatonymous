using System;
using System.Threading.Tasks;
using Automatonymous.Activities;
using Automatonymous.Binders;
using GreenPipes;
using NServiceBus;
using NServiceBus.Automatonymous.Activities;

// ReSharper disable once CheckNamespace
namespace Automatonymous
{
    public static class AutomatonymousExtensions
    {
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="message">The <see cref="TMessage"/>.</param>
        /// <param name="sendOptionsConfiguration">The <see cref="SendOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
            TMessage message,
            Action<BehaviorContext<TInstance>, SendOptions>? sendOptionsConfiguration = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new SendActivity<TInstance,TMessage>(_ => message, sendOptionsConfiguration));
        
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="message">The <see cref="TMessage"/>.</param>
        /// <param name="sendOptionsConfiguration">The <see cref="SendOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
            Task<TMessage> message,
            Action<BehaviorContext<TInstance>, SendOptions>? sendOptionsConfiguration = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new SendActivity<TInstance,TMessage>(_ => message, sendOptionsConfiguration));
        
        
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="messageFactory">The <see cref="TMessage"/> factory.</param>
        /// <param name="configSendOptions">The <see cref="SendOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(this EventActivityBinder<TInstance> binder,
            Func<BehaviorContext<TInstance>, TMessage> messageFactory,
            Action<BehaviorContext<TInstance>, SendOptions>? configSendOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new SendActivity<TInstance,TMessage>(messageFactory, configSendOptions));
        
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="messageFactory">The <see cref="TMessage"/> factory.</param>
        /// <param name="configSendOptions">The <see cref="SendOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> binder,
            Func<BehaviorContext<TInstance>, Task<TMessage>> messageFactory,
            Action<BehaviorContext<TInstance>, SendOptions>? configSendOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new SendActivity<TInstance,TMessage>(messageFactory, configSendOptions));


        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="message">The <see cref="TMessage"/>.</param>
        /// <param name="configSendOptions">The <see cref="SendOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
            TMessage message,
            Action<BehaviorContext<TInstance, TData>, SendOptions>? configSendOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new SendActivity<TInstance, TData,TMessage>(_ => message, configSendOptions));
        
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="message">The <see cref="TMessage"/>.</param>
        /// <param name="configSendOptions">The <see cref="SendOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
            Task<TMessage> message,
            Action<BehaviorContext<TInstance, TData>, SendOptions>? configSendOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new SendActivity<TInstance, TData,TMessage>(_ => message, configSendOptions));
       
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="messageFactory">The <see cref="TMessage"/> factory.</param>
        /// <param name="configSendOptions">The <see cref="SendOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Send<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory,
            Action<BehaviorContext<TInstance, TData>, SendOptions>? configSendOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new SendActivity<TInstance, TData,TMessage>(messageFactory, configSendOptions));
        
        /// <summary>
        /// Send a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="messageFactory">The <see cref="TMessage"/> factory.</param>
        /// <param name="configSendOptions">The <see cref="SendOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> SendAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, Task<TMessage>> messageFactory,
            Action<BehaviorContext<TInstance, TData>, SendOptions>? configSendOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new SendActivity<TInstance, TData,TMessage>(messageFactory, configSendOptions));
        

        // TODO move
        public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, TMessage> createMessage,
            Action<BehaviorContext<TInstance, TData>, PublishOptions>? configSendOptions = null)
            where TInstance : class
        {
            return binder.Add(new AsyncActivity<TInstance, TData>(context =>
            {
                var message = createMessage(context);

                var options = new PublishOptions();
                configSendOptions?.Invoke(context, options);    
                
                return context.GetPayload<IMessageHandlerContext>()
                    .Publish(message, options);
            }));
        }  
        
        public static EventActivityBinder<TInstance, TMessage> RequestTimeout<TInstance, TMessage, TRequestMessage>(
            this EventActivityBinder<TInstance, TMessage> binder,
            Func<BehaviorContext<TInstance, TMessage>, TRequestMessage> createMessage, DateTime at)
            where TInstance : class, IContainSagaData
        {
            if (at.Kind == DateTimeKind.Unspecified)
            {
                throw new InvalidOperationException("Kind property of DateTime 'at' must be specified.");
            }
            
            return binder.Send(createMessage, (context, opt) =>
            {
                opt.DoNotDeliverBefore(at);
                opt.RouteToThisEndpoint();
                opt.SetHeader(Headers.SagaId, context.Instance.Id.ToString());
                opt.SetHeader(Headers.IsSagaTimeoutMessage, bool.TrueString);
                opt.SetHeader(Headers.SagaType, context.GetPayload<Type>().AssemblyQualifiedName);
            });
        } 
        
        public static EventActivityBinder<TInstance, TMessage> RequestTimeout<TInstance, TMessage, TRequestMessage>(
            this EventActivityBinder<TInstance, TMessage> binder, DateTime at)
            where TInstance : class, IContainSagaData
            where TRequestMessage : new() 
            => binder.RequestTimeout(_ => new TRequestMessage(), at);
    }
}