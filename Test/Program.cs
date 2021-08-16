using System;
using System.Threading.Tasks;
using Automatonymous;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // var machine = new Order2StateMachine();
            // machine.RaiseEvent(new OrderState(), machine.SubmitOrder);
        }
    }
    
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
    }
    
    public class SubmitOrder
    {
        public Guid OrderId { get; set; }
    }
    
    public sealed class Order2StateMachine : MassTransitStateMachine<OrderState>
    {
        public Order2StateMachine()
        {
            Event(() => SubmitOrder,
                x =>
                {
                    x.CorrelateById(y => y.Message.OrderId);
                    x.CorrelateBy((state, context) => state.CorrelationId == context.ConversationId);
                });
            InstanceState(x => x.CurrentState);
            Event(() => SubmitOrder);
            
            // Initially(When(SubmitOrder)
            //     .Send()
            // );
        }
        
        public Event<SubmitOrder> SubmitOrder { get; private set; }
    }
}
