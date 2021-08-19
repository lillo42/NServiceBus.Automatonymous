using System;
using System.Threading.Tasks;
using Automatonymous.Binders;
using NServiceBus;
using NServiceBus.Automatonymous.Activities;

// ReSharper disable once CheckNamespace
namespace Automatonymous
{
    public static partial class AutomatonymousExtensions
    {
        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="message">The <see cref="TMessage"/>.</param>
        /// <param name="configureOptions">The <see cref="PublishOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Publish<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
            TMessage message,
            Action<BehaviorContext<TInstance>, PublishOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new PublishActivity<TInstance,TMessage>(_ => message, configureOptions));
        
        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="message">The <see cref="TMessage"/>.</param>
        /// <param name="configureOptions">The <see cref="PublishOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> PublishAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
            Task<TMessage> message,
            Action<BehaviorContext<TInstance>, PublishOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new PublishActivity<TInstance,TMessage>(_ => message, configureOptions));
        
        
        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="messageFactory">The <see cref="TMessage"/> factory.</param>
        /// <param name="configureOptions">The <see cref="PublishOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Publish<TInstance, TMessage>(this EventActivityBinder<TInstance> binder,
            Func<BehaviorContext<TInstance>, TMessage> messageFactory,
            Action<BehaviorContext<TInstance>, PublishOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new PublishActivity<TInstance,TMessage>(messageFactory, configureOptions));
        
        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="messageFactory">The <see cref="TMessage"/> factory.</param>
        /// <param name="configureOptions">The <see cref="PublishOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> PublishAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> binder,
            Func<BehaviorContext<TInstance>, Task<TMessage>> messageFactory,
            Action<BehaviorContext<TInstance>, PublishOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new PublishActivity<TInstance,TMessage>(messageFactory, configureOptions));


        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="message">The <see cref="TMessage"/>.</param>
        /// <param name="configureOptions">The <see cref="SendOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Publish<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
            TMessage message,
            Action<BehaviorContext<TInstance, TData>, PublishOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new PublishActivity<TInstance, TData,TMessage>(_ => message, configureOptions));
        
        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="message">The <see cref="TMessage"/>.</param>
        /// <param name="configureOptions">The <see cref="PublishOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
            Task<TMessage> message,
            Action<BehaviorContext<TInstance, TData>, PublishOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new PublishActivity<TInstance, TData,TMessage>(_ => message, configureOptions));
       
        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="messageFactory">The <see cref="TMessage"/> factory.</param>
        /// <param name="configureOptions">The <see cref="PublishOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Publish<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory,
            Action<BehaviorContext<TInstance, TData>, PublishOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new PublishActivity<TInstance, TData,TMessage>(messageFactory, configureOptions));
        
        /// <summary>
        /// Publish a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="messageFactory">The <see cref="TMessage"/> factory.</param>
        /// <param name="configureOptions">The <see cref="PublishOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> PublishAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, Task<TMessage>> messageFactory,
            Action<BehaviorContext<TInstance, TData>, PublishOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class 
            => binder.Add(new PublishActivity<TInstance, TData,TMessage>(messageFactory, configureOptions));
    }
}