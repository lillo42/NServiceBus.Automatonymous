using System;
using Automatonymous;

// ReSharper disable InconsistentNaming

namespace NServiceBus.Automatonymous
{
    /// <summary>
    /// Holds the state of a scheduled message.
    /// </summary>
    /// <typeparam name="TInstance">The saga data.</typeparam>
    /// <typeparam name="TMessage">The message.</typeparam>
    public interface Schedule<in TInstance, TMessage> : Schedule<TInstance>
        where TInstance : class, IContainSagaData
        where TMessage : IMessage
    {
        /// <summary>
        /// This event is raised when the scheduled message is received. If a previous message
        /// was rescheduled, this event is filtered so that only the most recently scheduled
        /// message is allowed.
        /// </summary>
        Event<TMessage> Received { get; set; }
        
        /// <summary>
        /// This event is raised when any message is directed at the state machine, but it is
        /// not filtered to the currently scheduled event. So outdated or original events may
        /// be raised.
        /// </summary>
        Event<TMessage> AnyReceived { get; set; }
    }
    
    /// <summary>
    /// Holds the state of a scheduled message.
    /// </summary>
    /// <typeparam name="TInstance">The saga data.</typeparam>
    public interface Schedule<in TInstance>
        where TInstance : class, IContainSagaData
    {
        /// <summary>
        /// The name of the scheduled message.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the delay, given the instance, for the scheduled message.
        /// </summary>
        /// <returns>Returns the delay, given the instance, for the scheduled message.</returns>
        TimeSpan GetDelay(BehaviorContext<TInstance> context);
        
        /// <summary>
        /// Return the TokenId for the instance.
        /// </summary>
        /// <param name="data">The <typeparamref name="TInstance"/>.</param>
        /// <returns>Return the TokenId for the instance.</returns>
        Guid? GetTokenId(TInstance data);

        /// <summary>
        /// Set the token ID on the Instance.
        /// </summary>
        /// <param name="data">The <typeparamref name="TInstance"/>.</param>
        /// <param name="tokenId">The token id.</param>
        void SetTokenId(TInstance data, Guid? tokenId);
    }
}