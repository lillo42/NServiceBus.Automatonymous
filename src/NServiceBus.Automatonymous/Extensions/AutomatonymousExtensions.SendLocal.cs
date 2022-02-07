using System;
using System.Threading.Tasks;
using Automatonymous.Binders;
using NServiceBus;
using NServiceBus.Automatonymous.Activities;

// ReSharper disable once CheckNamespace
namespace Automatonymous;

public static partial class AutomatonymousExtensions
{
    /// <summary>
    /// Send local a message.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="configureOptions">The <see cref="SendOptions"/> configurator</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> SendLocal<TInstance, TMessage>(
        this EventActivityBinder<TInstance> binder,
        Action<BehaviorContext<TInstance>, SendOptions>? configureOptions = null)
        where TInstance : class, IContainSagaData
        where TMessage : class, IMessage , new()
        => binder.Add(new SendActivity<TInstance, TMessage>(_ => new TMessage(), configureOptions));
        
    /// <summary>
    /// Send local a message.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> SendLocal<TInstance, TMessage>(
        this EventActivityBinder<TInstance> binder,
        TMessage message)
        where TInstance : class, IContainSagaData
        where TMessage : class, IMessage 
        => binder.Add(new SendActivity<TInstance, TMessage>(_ => message, (_, opt) => opt.RouteToThisEndpoint()));

    /// <summary>
    /// Send local a message.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> SendAsync<TInstance, TMessage>(
        this EventActivityBinder<TInstance> binder,
        Task<TMessage> message)
        where TInstance : class, IContainSagaData
        where TMessage : class, IMessage 
        => binder.Add(new SendActivity<TInstance, TMessage>(_ => message, (_, opt) => opt.RouteToThisEndpoint()));


    /// <summary>
    /// Send local a message.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> Send<TInstance, TMessage>(
        this EventActivityBinder<TInstance> binder,
        Func<BehaviorContext<TInstance>, TMessage> messageFactory)
        where TInstance : class, IContainSagaData
        where TMessage : class, IMessage 
        => binder.Add(new SendActivity<TInstance, TMessage>(messageFactory, (_, opt) => opt.RouteToThisEndpoint()));

    /// <summary>
    /// Send local a message.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> SendLocalAsync<TInstance, TMessage>(
        this EventActivityBinder<TInstance> binder,
        Func<BehaviorContext<TInstance>, Task<TMessage>> messageFactory)
        where TInstance : class, IContainSagaData
        where TMessage : class, IMessage 
        => binder.Add(new SendActivity<TInstance, TMessage>(messageFactory, (_, opt) => opt.RouteToThisEndpoint()));


    /// <summary>
    /// Send local a message.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> SendLocal<TInstance, TData, TMessage>(
        this EventActivityBinder<TInstance, TData> binder,
        TMessage message)
        where TInstance : class, IContainSagaData
        where TMessage : class, IMessage 
        => binder.Add(new SendActivity<TInstance, TData, TMessage>(_ => message, (_, opt) => opt.RouteToThisEndpoint()));

    /// <summary>
    /// Send local a message.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> SendLocalAsync<TInstance, TData, TMessage>(
        this EventActivityBinder<TInstance, TData> binder,
        Task<TMessage> message)
        where TInstance : class, IContainSagaData
        where TMessage : class, IMessage 
        => binder.Add(new SendActivity<TInstance, TData, TMessage>(_ => message, (_, opt) => opt.RouteToThisEndpoint()));

    /// <summary>
    /// Send local a message.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> SendLocal<TInstance, TData, TMessage>(
        this EventActivityBinder<TInstance, TData> binder,
        Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory)
        where TInstance : class, IContainSagaData
        where TMessage : class, IMessage 
        => binder.Add(new SendActivity<TInstance, TData, TMessage>(messageFactory, (_, opt) => opt.RouteToThisEndpoint()));

    /// <summary>
    /// Send local a message.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance,TData}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage" /> factory.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> SendLocalAsync<TInstance, TData, TMessage>(
        this EventActivityBinder<TInstance, TData> binder,
        Func<BehaviorContext<TInstance, TData>, Task<TMessage>> messageFactory)
        where TInstance : class, IContainSagaData
        where TMessage : class, IMessage 
        => binder.Add(new SendActivity<TInstance, TData, TMessage>(messageFactory, (_, opt) => opt.RouteToThisEndpoint()));
}