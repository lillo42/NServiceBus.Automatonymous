using System;
using System.Threading.Tasks;
using NServiceBus;
using Trashlantis.Contracts;

namespace Trashlantis
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointConfiguration = new EndpointConfiguration("Trashlantis");
            
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseTransport<LearningTransport>();
            
            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to send a EmptyTrashBin message");
            Console.WriteLine("Press any other key to exit");
            
            while (true)
            {
                Console.WriteLine();
                if (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    break;
                }
                
                var binNumber = Guid.NewGuid();
                var emptyTrashBin = new EmptyTrashBin
                {
                    BinNumber = binNumber.ToString()
                };
                await endpointInstance.SendLocal(emptyTrashBin)
                    .ConfigureAwait(false);
                Console.WriteLine($"Sent EmptyTrashBin with BinNumber {binNumber}.");
            }

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}