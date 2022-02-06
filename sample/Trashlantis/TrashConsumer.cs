using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Trashlantis.Contracts;

namespace Trashlantis;

public class TrashConsumer  : IHandleMessages<EmptyTrashBin>
{
    private static readonly ILog Log = LogManager.GetLogger<TrashConsumer>();
        
    public async Task Handle(EmptyTrashBin message, IMessageHandlerContext context)
    {
        Log.InfoFormat("Emptying Trash bin: {0}", message.BinNumber);
        await Task.Delay(1_000);
    }
}