using System;
using Automatonymous;

namespace TestProject1
{
    public interface SubmitOrder
    {
        Guid OrderId { get; }

        DateTime OrderDate { get; }
    }
    
    public interface OrderAccepted
    {
        Guid OrderId { get; }

        DateTime OrderDate { get; }
    }
    
    public class OrderData : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        
        public Guid? OrderCompletionTimeoutId { get; set; }
        public DateTime? OrderDate { get; set; }
    }
    
    public sealed class OrderSaga : MassTransitStateMachine<OrderData>
    {
        public OrderSaga()
        {
            Schedule(() => OrderCompletionTimeout, x => x.OrderCompletionTimeoutId,
                x =>
                {
                    x.DelayProvider = _ => TimeSpan.Zero;
                });
            
            Initially(
                When(SubmitOrder)
                    .Then(x => x.Instance.OrderDate = x.Data.OrderDate)
                    .TransitionTo(Submitted),
                When(OrderAccepted)
                    .TransitionTo(Accepted));

            During(Submitted,
                When(OrderAccepted)
                    //.Request()
                    // .Schedule(x => x.Init<OrderCompletionTimeoutExpired>(new{}))
                      // .Unschedule()
                    //.Unschedule()
                    .TransitionTo(Accepted));

            During(Accepted,
                When(SubmitOrder)
                    .Then(x => x.Instance.OrderDate = x.Data.OrderDate));
        }

        public Event<SubmitOrder> SubmitOrder { get; private set; } = null!;
        public Event<OrderAccepted> OrderAccepted { get; private set; } = null!;
        public State Submitted { get; private set; } = null!;
        public State Accepted { get; private set; } = null!;

        public Schedule<OrderData, OrderCompletionTimeoutExpired> OrderCompletionTimeout { get; private set; } = null!;
    }
    
    public class OrderCompletionTimeoutExpired
    {
        Guid OrderId { get; }
    }
}