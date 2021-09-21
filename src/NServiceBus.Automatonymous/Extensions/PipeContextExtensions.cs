using System;
using GreenPipes;

namespace NServiceBus.Automatonymous.Extensions
{
    public static class PipeContextExtensions
    {
        public static TMessage Init<TMessage>(this PipeContext context)
            where TMessage : IMessage 
        {
            var messageCreator = context.GetPayload<IMessageCreator>();
            return messageCreator.CreateInstance<TMessage>();
        }
        
        public static TMessage Init<TMessage>(this PipeContext context, Action<TMessage> configure)
            where TMessage : IMessage 
        {
            var messageCreator = context.GetPayload<IMessageCreator>();
            return messageCreator.CreateInstance(configure);
        }
    }
}