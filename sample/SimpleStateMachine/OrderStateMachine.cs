using System;
using System.Linq.Expressions;
using Automatonymous;
using GreenPipes;
using NServiceBus.Automatonymous;
using NServiceBus.Logging;

namespace SimpleStateMachine
{
    public sealed class OrderStateMachine : NServiceBusStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => CompleteOrder);
            
            Schedule(() => CancelOrder, state => state.CancelOrderId, x =>
            {
                x.Received = cfg => cfg.CorrelateBy(y => y.OrderId);
            });

            Initially(When(SubmitOrder)
                .Then(context =>
                {
                    var log = context.GetPayload<ILog>();
                    log.Info($"StartOrder received with OrderId {context.Data.OrderId}");
                    log.Info("Sending a CompleteOrder that will be delayed by 10 seconds");
                })
                // .Send(context => new CompleteOrder { OrderId = context.Instance.OrderId },
                //     (_, opt) =>
                //     {
                //         opt.DelayDeliveryWith(TimeSpan.FromSeconds(50));
                //         opt.RouteToThisEndpoint();
                //     })
                .Then(context => context.GetPayload<ILog>().Info("Requesting a CancelOrder that will be executed in 30 seconds."))
                .Schedule(CancelOrder,context => new CancelOrder { OrderId = context.Instance.OrderId }, DateTime.UtcNow.AddSeconds(30))
                .TransitionTo(OrderStarted));
            
            During(OrderStarted, When(CompleteOrder)
                .Then(context => context.GetPayload<ILog>().Info($"CompleteOrder received with OrderId {context.Data.OrderId}"))
                .Finalize());
            
            DuringAny(When(CancelOrder.Received)
                .Then(context => context.GetPayload<ILog>().Info($"CompleteOrder not received soon enough OrderId {context.Instance.OrderId}. Calling MarkAsComplete"))
                .Finalize());
        }

        public override Expression<Func<OrderState, object>> CorrelationByProperty() => x => x.OrderId;
        protected override string DefaultCorrelationMessageByPropertyName => "OrderId";
        
        public State OrderStarted { get; private set; } = null!;
        
        [StartStateMachine]
        public Event<StartOrder> SubmitOrder { get; private set; } = null!;
        
        //[TimeoutEvent]
        //public Event<CancelOrder> CancelOrder { get; private set; } = null!;
        public Schedule<OrderState, CancelOrder> CancelOrder { get; private set; } = null!;
        
        public Event<CompleteOrder> CompleteOrder { get; private set; } = null!;
        
    }
    
    // public class OrderStateMachineNServiceBusSaga2 : NServiceBusSaga<OrderStateMachine, OrderState>, IAmStartedByMessages<StartOrder>,
    //     IHandleMessages<CompleteOrder>,
    //     IHandleMessages<CancelOrder>
    // {
    //     public OrderStateMachineNServiceBusSaga2(OrderStateMachine stateMachine, IBuilder builder)
    //         : base(stateMachine, builder)
    //     {
    //     }
    //     
    //     public Task Handle(StartOrder message, IMessageHandlerContext context)
    //     {
    //         return Execute(message, context, StateMachine.SubmitOrder);
    //     }
    //     public Task Handle(CompleteOrder message, IMessageHandlerContext context)
    //     {
    //         return Execute(message, context, StateMachine.CompleteOrder);
    //     }
    //     public Task Handle(CancelOrder message, IMessageHandlerContext context)
    //     {
    //         return Execute(message, context, StateMachine.CancelOrder.AnyReceived);
    //     }
    // }
}