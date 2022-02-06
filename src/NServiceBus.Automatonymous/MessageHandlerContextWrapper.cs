namespace NServiceBus.Automatonymous;

/// <summary>
/// The <see cref="IMessageHandlerContext"/> wrapper
/// </summary>
public class MessageHandlerContextWrapper
{
    /// <summary>
    /// The <see cref="IMessageHandlerContext"/>.
    /// </summary>
    public IMessageHandlerContext? MessageHandlerContext { get; set; }
}