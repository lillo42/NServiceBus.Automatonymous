using System;
using System.Linq.Expressions;
using Automatonymous;
using GreenPipes;
using NServiceBus;
using NServiceBus.Automatonymous;
using NServiceBus.Logging;

namespace SimpleStateMachine;

public sealed class OrderStateMachine : NServiceBusStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => CompleteOrder);

        Initially(When(SubmitOrder)
            .Then(context =>
            {
                var log = context.GetPayload<ILog>();
                log.Info($"StartOrder received with OrderId {context.Data.OrderId}");
                log.Info("Sending a CompleteOrder that will be delayed by 10 seconds");
            })
            .Send(context => new CompleteOrder { OrderId = context.Instance.OrderId },
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
    }

    public override Expression<Func<OrderState, object>> CorrelationByProperty() => x => x.OrderId;
    protected override string DefaultCorrelationMessageByPropertyName => "OrderId";
        
    public State OrderStarted { get; private set; } = null!;
        
    [StartStateMachine]
    public Event<StartOrder> SubmitOrder { get; private set; } = null!;
        
    [TimeoutEvent]
    public Event<CancelOrder> CancelOrder { get; private set; } = null!;
        
    public Event<CompleteOrder> CompleteOrder { get; private set; } = null!;
}