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
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="at">The <see cref="DateTime"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> RequestTimeout<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, DateTime at)
        where TInstance : class, IContainSagaData
        where TMessage : class, new() 
        => binder.Add(new RequestTimeoutActivity<TInstance,TMessage>(_ => new TMessage(), at));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="within">The <see cref="TimeSpan"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> RequestTimeout<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, TimeSpan within)
        where TInstance : class, IContainSagaData
        where TMessage : class, new() 
        => binder.Add(new RequestTimeoutActivity<TInstance,TMessage>(_ => new TMessage(), within));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <param name="at">The <see cref="DateTime"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> RequestTimeout<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
        TMessage message, DateTime at)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance,TMessage>(_ => message, at));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <param name="within">The <see cref="TimeSpan"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> RequestTimeout<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
        TMessage message, TimeSpan within)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance,TMessage>(_ => message, within));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <param name="at">The <see cref="DateTime"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> RequestTimeoutAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
        Task<TMessage> message, DateTime at)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance,TMessage>(_ => message, at));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <param name="within">The <see cref="TimeSpan"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> RequestTimeoutAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
        Task<TMessage> message, TimeSpan within)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance,TMessage>(_ => message, within));

    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <param name="at">The <see cref="DateTime"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> RequestTimeout<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
        Func<BehaviorContext<TInstance>, TMessage> messageFactory, DateTime at)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance,TMessage>(messageFactory, at));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <param name="within">The <see cref="TimeSpan"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> RequestTimeout<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
        Func<BehaviorContext<TInstance>, TMessage> messageFactory, TimeSpan within)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance,TMessage>(messageFactory, within));
        
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <param name="at">The <see cref="DateTime"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> RequestTimeoutAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
        Func<BehaviorContext<TInstance>, Task<TMessage>> messageFactory, DateTime at)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance,TMessage>(messageFactory, at));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <param name="within">The <see cref="TimeSpan"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance> RequestTimeoutAsync<TInstance, TMessage>(this EventActivityBinder<TInstance> binder, 
        Func<BehaviorContext<TInstance>, Task<TMessage>> messageFactory, TimeSpan within)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance,TMessage>(messageFactory, within));

    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="at">The <see cref="DateTime"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> RequestTimeout<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, DateTime at)
        where TInstance : class, IContainSagaData
        where TMessage : class, new() 
        => binder.Add(new RequestTimeoutActivity<TInstance, TData,TMessage>(_ => new TMessage(), at));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="within">The <see cref="TimeSpan"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> RequestTimeout<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, TimeSpan within)
        where TInstance : class, IContainSagaData
        where TMessage : class, new() 
        => binder.Add(new RequestTimeoutActivity<TInstance, TData,TMessage>(_ => new TMessage(), within));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <param name="at">The <see cref="DateTime"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> RequestTimeout<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
        TMessage message, DateTime at)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance, TData,TMessage>(_ => message, at));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <param name="within">The <see cref="TimeSpan"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> RequestTimeout<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
        TMessage message, TimeSpan within)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance, TData,TMessage>(_ => message, within));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <param name="at">The <see cref="DateTime"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> RequestTimeoutAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
        Task<TMessage> message, DateTime at)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance, TData,TMessage>(_ => message, at));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="message">The <typeparamref name="TMessage"/>.</param>
    /// <param name="within">The <see cref="TimeSpan"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> RequestTimeoutAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
        Task<TMessage> message, TimeSpan within)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance, TData,TMessage>(_ => message, within));

    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <param name="at">The <see cref="DateTime"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> RequestTimeout<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
        Func<BehaviorContext<TInstance>, TMessage> messageFactory, DateTime at)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance, TData,TMessage>(messageFactory, at));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <param name="within">The <see cref="TimeSpan"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> RequestTimeout<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
        Func<BehaviorContext<TInstance>, TMessage> messageFactory, TimeSpan within)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance, TData,TMessage>(messageFactory, within));
        
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <param name="at">The <see cref="DateTime"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> RequestTimeoutAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
        Func<BehaviorContext<TInstance, TData>, Task<TMessage>> messageFactory, DateTime at)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance, TData,TMessage>(messageFactory, at));
        
    /// <summary>
    /// Request timeout.
    /// </summary>
    /// <param name="binder">The <see cref="EventActivityBinder{TInstance}"/>.</param>
    /// <param name="messageFactory">The <typeparamref name="TMessage"/> factory.</param>
    /// <param name="within">The <see cref="TimeSpan"/> that the <typeparamref name="TMessage"/> should be send.</param>
    /// <typeparam name="TInstance">The state machine data.</typeparam>
    /// <typeparam name="TData">The event data.</typeparam>
    /// <typeparam name="TMessage">The message type.</typeparam>
    /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
    public static EventActivityBinder<TInstance, TData> RequestTimeoutAsync<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> binder, 
        Func<BehaviorContext<TInstance, TData>, Task<TMessage>> messageFactory, TimeSpan within)
        where TInstance : class, IContainSagaData
        where TMessage : class 
        => binder.Add(new RequestTimeoutActivity<TInstance, TData,TMessage>(messageFactory, within));
        
}