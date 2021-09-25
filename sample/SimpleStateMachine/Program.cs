using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SimpleStateMachine
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointConfiguration = new EndpointConfiguration("Samples.SimpleSaga");
            
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseTransport<LearningTransport>();
            endpointConfiguration.EnableUniformSession();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to send a StartOrder message");
            Console.WriteLine("Press any other key to exit");
            
            while (true)
            {
                Console.WriteLine();
                if (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    break;
                }
                var orderId = Guid.NewGuid();
                var startOrder = new StartOrder
                {
                    OrderId = orderId
                };
                await endpointInstance.SendLocal(startOrder)
                    .ConfigureAwait(false);
                Console.WriteLine($"Sent StartOrder with OrderId {orderId}.");
            }

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}
