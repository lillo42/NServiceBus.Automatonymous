using System;
using System.Linq.Expressions;
using Automatonymous;
using GreenPipes;
using NServiceBus;
using NServiceBus.Automatonymous;
using NServiceBus.Logging;

namespace Test
{
    public class CancelOrder : IMessage { }
    
    public class CompleteOrder : IMessage
    {
        public Guid OrderId { get; set; }
    }
    
    public class StartOrder : IMessage
    {
        public Guid OrderId { get; set; }
    }
    
    public class OrderState : ContainSagaData
    {
        public string CurrentState { get; set; }
        public Guid OrderId { get; set; }
    }
    
    public sealed class OrderStateMachine : NServiceBusStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Initially(When(SubmitOrder)
                .Then(context =>
                {
                    var log = context.GetPayload<ILog>();
                    log.Info($"StartOrder received with OrderId {context.Data.OrderId}");
                    log.Info("Sending a CompleteOrder that will be delayed by 10 seconds");
                })
                .SendAsync(context => new CompleteOrder { OrderId = context.Instance.Id },
                    (_, opt) =>
                    {
                        opt.DelayDeliveryWith(TimeSpan.FromSeconds(10));
                        opt.RouteToThisEndpoint();
                    })
                .Then(context => context.GetPayload<ILog>().Info(@"Requesting a CancelOrder that will be executed in 30 seconds."))
                .RequestTimeout(_ => new CancelOrder(), DateTime.UtcNow.AddSeconds(30))
                .TransitionTo(OrderStarted));
            
            During(OrderStarted, When(CompleteOrder)
                .Then(context => context.GetPayload<ILog>().Info($"CompleteOrder received with OrderId {context.Data.OrderId}"))
                .Finalize());
            
            DuringAny(When(CancelOrder)
                .Then(context => context.GetPayload<ILog>().Info($"CompleteOrder not received soon enough OrderId {context.Instance.OrderId}. Calling MarkAsComplete"))
                .Finalize());

            DoSomething();
            DoSomething2();
            
            void DoSomething2() 
            {
                Initially(When(SubmitOrder), When(CancelOrder));
            }
        }
        
        private void DoSomething()
        {
            Initially(When(SubmitOrder), When(CancelOrder));
            
            During(OrderStarted, When(CompleteOrder)
                .Then(context => context.GetPayload<ILog>().Info($"CompleteOrder received with OrderId {context.Data.OrderId}"))
                .Finalize());

        }

        public override Expression<Func<OrderState, object>> CorrelationByProperty() => x => x.OrderId;
        protected override string DefaultCorrelationMessageByPropertyName => "OrderId";

        public State OrderStarted { get; private set; } = null!;
        
        [StartSaga]
        public Event<StartOrder> SubmitOrder { get; private set; } = null!;
        
        public Event<CancelOrder> CancelOrder { get; private set; } = null!;
        
        public Event<CompleteOrder> CompleteOrder { get; private set; } = null!;
    }
}