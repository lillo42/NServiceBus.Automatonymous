using NServiceBus;

namespace Trashlantis.Contracts;

public class TakeOutTheTrash : IMessage
{
    public string BinNumber { get; set; } = string.Empty;
}