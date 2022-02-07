using System.Diagnostics.CodeAnalysis;

namespace NServiceBus.Automatonymous;

/// <summary>
/// The <see cref="IMessageHandlerContext"/> wrapper
/// </summary>
[ExcludeFromCodeCoverage] 
public class MessageHandlerContextWrapper
{
    /// <summary>
    /// The <see cref="IMessageHandlerContext"/>.
    /// </summary>
    public IMessageHandlerContext? MessageHandlerContext { get; set; }
}