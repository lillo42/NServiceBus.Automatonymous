using System;
using GreenPipes;

namespace NServiceBus.Automatonymous.Extensions
{
    /// <summary>
    /// Pipe context
    /// </summary>
    public static class PipeContextExtensions
    {
        /// <summary>
        /// Create new instance <typeparamref name="TMessage"/>.
        /// </summary>
        /// <param name="context">The <see cref="PipeContext"/>.</param>
        /// <typeparam name="TMessage">The message.</typeparam>
        /// <returns>The new instance <typeparamref name="TMessage"/>.</returns>
        public static TMessage Init<TMessage>(this PipeContext context)
            where TMessage : IMessage 
        {
            var messageCreator = context.GetPayload<IMessageCreator>();
            return messageCreator.CreateInstance<TMessage>();
        }

        /// <summary>
        /// Create new instance <typeparamref name="TMessage"/>.
        /// </summary>
        /// <param name="context">The <see cref="PipeContext"/>.</param>
        /// <param name="configure">The <typeparamref name="TMessage"/> config.</param>
        /// <typeparam name="TMessage">The message.</typeparam>
        /// <returns>The new instance <typeparamref name="TMessage"/>.</returns>
        public static TMessage Init<TMessage>(this PipeContext context, Action<TMessage> configure)
            where TMessage : IMessage 
        {
            var messageCreator = context.GetPayload<IMessageCreator>();
            return messageCreator.CreateInstance(configure);
        }
    }
}