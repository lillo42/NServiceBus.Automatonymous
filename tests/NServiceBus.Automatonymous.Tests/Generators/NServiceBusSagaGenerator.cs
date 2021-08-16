using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace NServiceBus.Automatonymous.Tests.Generators
{
    public class NServiceBusSagaGenerator : BaseTest
    {
        [Fact]
        public async Task GenerateSimpleSaga()
        {
            var driver = await GenerateMapperAsync(@"
using System;
using System.Linq.Expressions;
using Automatonymous;
using GreenPipes;
using NServiceBus.Logging;

namespace NServiceBus.Automatonymous.Tests
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
                    log.Info($""StartOrder received with OrderId {context.Data.OrderId}"");
                    log.Info(""Sending a CompleteOrder that will be delayed by 10 seconds"");
                })
                .SendAsync(context => new CompleteOrder { OrderId = context.Instance.Id },
                    (_, opt) =>
                    {
                        opt.DelayDeliveryWith(TimeSpan.FromSeconds(10));
                        opt.RouteToThisEndpoint();
                    })
                .Then(context => context.GetPayload<ILog>().Info(@""Requesting a CancelOrder that will be executed in 30 seconds.""))
                .RequestTimeout(_ => new CancelOrder(), DateTime.UtcNow.AddSeconds(30))
                .TransitionTo(OrderStarted));
            
            During(OrderStarted, When(CompleteOrder)
                .Then(context => context.GetPayload<ILog>().Info($""CompleteOrder received with OrderId {context.Data.OrderId}""))
                .Finalize());
            
            DuringAny(When(CancelOrder)
                .Then(context => context.GetPayload<ILog>().Info($""CompleteOrder not received soon enough OrderId {context.Instance.OrderId}. Calling MarkAsComplete""))
                .Finalize());
        }

        public override Expression<Func<OrderState, object>> CorrelationByProperty() => x => x.OrderId;
        protected override string DefaultCorrelationMessageByPropertyName => ""OrderId"";

        public State OrderStarted { get; private set; } = null!;
        
        [StartSaga]
        public Event<StartOrder> SubmitOrder { get; private set; } = null!;
        
        public Event<CancelOrder> CancelOrder { get; private set; } = null!;
        
        public Event<CompleteOrder> CompleteOrder { get; private set; } = null!;
    }
}
");

            var result = driver.GetRunResult();
            result.Diagnostics.Should().BeEmpty();
            result.Results.Should().ContainSingle();
        }
    }
}