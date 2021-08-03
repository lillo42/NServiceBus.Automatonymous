using System;
using System.Threading;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes.Introspection;
using NServiceBus;
using NServiceBus.Automatonymous;
using NServiceBus.Automatonymous.Tests;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var endpointConfiguration = new EndpointConfiguration("Samples.SimpleSaga");
            
            // endpointConfiguration.EnableFeature<AutomatonymousFeature>();
            endpointConfiguration.UsePersistence<LearningPersistence>();
            endpointConfiguration.UseTransport<LearningTransport>();

            
            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine();
            Console.WriteLine("Storage locations:");

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
            
            // var machine = new Order2StateMachine();
            // machine.RaiseEvent(new OrderState(), machine.SubmitOrder);
        }
    }
    
    // public class OrderState : SagaStateMachineInstance
    // {
    //     public Guid CorrelationId { get; set; }
    //     public string CurrentState { get; set; }
    // }
    //
    // public class SubmitOrder
    // {
    //     public Guid OrderId { get; set; }
    // }
    //
    // public sealed class Order2StateMachine : MassTransitStateMachine<OrderState>
    // {
    //     public Order2StateMachine()
    //     {
    //         Event(() => SubmitOrder,
    //             x =>
    //             {
    //                 x.CorrelateById(y => y.Message.OrderId);
    //             });
    //         InstanceState(x => x.CurrentState);
    //         Event(() => SubmitOrder);
    //         
    //         Initially(When(SubmitOrder));
    //     }
    //     
    //     public Event<SubmitOrder> SubmitOrder { get; private set; }
    // }
}
