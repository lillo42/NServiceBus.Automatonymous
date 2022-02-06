using System;

namespace NServiceBus.Automatonymous.Extensions;

internal static class IMessageHandlerContextExtensions
{
    public static Guid? GetSchedulingTokenId(this IMessageHandlerContext handlerContext)
    {
        if (handlerContext.MessageHeaders.TryGetValue(MessageHeaders.SchedulingTokenId, out var token)
            && Guid.TryParse(token, out var value))
        {
            return value;
        }

        return null;
    }
}