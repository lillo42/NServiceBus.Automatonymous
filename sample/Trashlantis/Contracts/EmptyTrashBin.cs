using NServiceBus;

namespace Trashlantis.Contracts;

public class EmptyTrashBin : IMessage
{
    public string BinNumber { get; set; } = string.Empty;
}