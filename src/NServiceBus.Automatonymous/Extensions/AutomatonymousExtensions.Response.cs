using System;
using System.Threading.Tasks;
using Automatonymous.Binders;
using GreenPipes;
using NServiceBus;
using NServiceBus.Automatonymous.Activities;

// ReSharper disable once CheckNamespace
namespace Automatonymous
{
    public static class AutomatonymousResponseExtensions
    {
        /// <summary>
        /// Response a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="message">The <see cref="TMessage"/>.</param>
        /// <param name="configureOptions">The <see cref="ReplyOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Response<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> binder,
            TMessage message,
            Action<BehaviorContext<TInstance, TData>, ReplyOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class
            => binder.Add(new ResponseActivity<TInstance, TData, TMessage>(_ => message, configureOptions));

        /// <summary>
        /// Response a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="message">The <see cref="TMessage"/>.</param>
        /// <param name="configureOptions">The <see cref="ReplyOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> ResponseAsync<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> binder,
            Task<TMessage> message,
            Action<BehaviorContext<TInstance, TData>, ReplyOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class
            => binder.Add(new ResponseActivity<TInstance, TData, TMessage>(_ => message, configureOptions));

        /// <summary>
        /// Response a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="messageFactory">The <see cref="TMessage"/> factory.</param>
        /// <param name="configureOptions">The <see cref="ReplyOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Response<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory,
            Action<BehaviorContext<TInstance, TData>, ReplyOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class
            => binder.Add(new ResponseActivity<TInstance, TData, TMessage>(messageFactory, configureOptions));

        /// <summary>
        /// Response a message.
        /// </summary>
        /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
        /// <param name="messageFactory">The <see cref="TMessage"/> factory.</param>
        /// <param name="configureOptions">The <see cref="ReplyOptions"/> configurator</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TData">The event data</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> ResponseAsync<TInstance, TData, TMessage>(
            this EventActivityBinder<TInstance, TData> binder,
            Func<BehaviorContext<TInstance, TData>, Task<TMessage>> messageFactory,
            Action<BehaviorContext<TInstance, TData>, ReplyOptions>? configureOptions = null)
            where TInstance : class, IContainSagaData
            where TMessage : class
            => binder.Add(new ResponseActivity<TInstance, TData, TMessage>(messageFactory, configureOptions));
    }
}