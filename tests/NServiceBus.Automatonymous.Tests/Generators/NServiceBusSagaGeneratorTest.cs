using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Xunit;

namespace NServiceBus.Automatonymous.Tests.Generators
{
    public class NServiceBusSagaGeneratorTest : BaseTest
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
        
        [StartStateMachine]
        public Event<StartOrder> SubmitOrder { get; private set; } = null!;
        
        public Event<CancelOrder> CancelOrder { get; private set; } = null!;
        
        public Event<CompleteOrder> CompleteOrder { get; private set; } = null!;
    }
}
");

            var result = driver.GetRunResult();
            result.Diagnostics.Should().BeEmpty();
            result.GeneratedTrees.Should().NotBeEmpty();
            result.GeneratedTrees.Should().ContainSingle();
            var generated = result.GeneratedTrees.Single();
            generated.ToString().Should().Be(@"// <auto-generated />
using System.Threading.Tasks;
using NServiceBus.Automatonymous;
using NServiceBus.ObjectBuilder;
using NServiceBus.Automatonymous.Tests;
using NServiceBus;

namespace NServiceBus.Automatonymous.Generated
{
    public class OrderStateMachineNServiceBusSaga : NServiceBusSaga<OrderStateMachine, OrderState>, IAmStartedByMessages<StartOrder>,
    IHandleMessages<CancelOrder>,
    IHandleMessages<CompleteOrder>
    {
        public OrderStateMachineNServiceBusSaga(OrderStateMachine stateMachine, IBuilder builder)
          : base(stateMachine, builder)
        {
        }
        public Task Handle(StartOrder message, IMessageHandlerContext context)
        {
            return Execute(message, context, StateMachine.SubmitOrder);
        }
        public Task Handle(CancelOrder message, IMessageHandlerContext context)
        {
            return Execute(message, context, StateMachine.CancelOrder);
        }
        public Task Handle(CompleteOrder message, IMessageHandlerContext context)
        {
            return Execute(message, context, StateMachine.CompleteOrder);
        }
    }
}
");
        }
        
        [Fact]
        public async Task Generate_Should_ReturnError_When_StateMachineHasNotStartStateMachineAttribute()
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
        
        public Event<StartOrder> SubmitOrder { get; private set; } = null!;
        
        public Event<CancelOrder> CancelOrder { get; private set; } = null!;
        
        public Event<CompleteOrder> CompleteOrder { get; private set; } = null!;
    }
}
");

            var result = driver.GetRunResult();
            result.Diagnostics.Should().NotBeEmpty();
            result.Diagnostics.Should().ContainSingle();
            
            var diagnostic = result.Diagnostics.First();
            diagnostic.Severity.Should().Be(DiagnosticSeverity.Error);
            diagnostic.Id.Should().Be("NSBA001");
            
            result.GeneratedTrees.Should().BeEmpty();
        }
        
        [Fact]
        public async Task GenerateSagaWithTimeout()
        {
            var driver = await GenerateMapperAsync(@"
using System;
using System.Linq.Expressions;
using Automatonymous;
using GreenPipes;
using NServiceBus;
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

            Initially(When(SubmitOrder)
                .Then(context =>
                {
                    var log = context.GetPayload<ILog>();
                    log.Info($""StartOrder received with OrderId {context.Data.OrderId}"");
                    log.Info(""Sending a CompleteOrder that will be delayed by 10 seconds"");
                })
                .Send(context => new CompleteOrder { OrderId = context.Instance.OrderId },
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
        
        [StartStateMachine]
        public Event<StartOrder> SubmitOrder { get; private set; } = null!;
        
        [TimeoutEvent]
        public Event<CancelOrder> CancelOrder { get; private set; } = null!;
        
        public Event<CompleteOrder> CompleteOrder { get; private set; } = null!;
    }

    public class OrderState : ContainSagaData
    {
        public string CurrentState { get; set; }
        public Guid OrderId { get; set; }
    }

    public class CancelOrder : IMessage { }
    public class CompleteOrder : IMessage
    {
        public Guid OrderId { get; set; }
    }

    public class StartOrder : IMessage
    {
        public Guid OrderId { get; set; }
    }
}
");

            var result = driver.GetRunResult();
            result.Diagnostics.Should().BeEmpty();
            result.GeneratedTrees.Should().NotBeEmpty();
            result.GeneratedTrees.Should().ContainSingle();
            var generated = result.GeneratedTrees.Single();
            generated.ToString().Should().Be(@"// <auto-generated />
using System.Threading.Tasks;
using NServiceBus.Automatonymous;
using NServiceBus.ObjectBuilder;
using SimpleStateMachine;
using NServiceBus;

namespace NServiceBus.Automatonymous.Generated
{
    public class OrderStateMachineNServiceBusSaga : NServiceBusSaga<OrderStateMachine, OrderState>, IAmStartedByMessages<StartOrder>,
    IHandleMessages<CompleteOrder>,
    IHandleTimeouts<CancelOrder>
    {
        public OrderStateMachineNServiceBusSaga(OrderStateMachine stateMachine, IBuilder builder)
          : base(stateMachine, builder)
        {
        }
        public Task Handle(StartOrder message, IMessageHandlerContext context)
        {
            return Execute(message, context, StateMachine.SubmitOrder);
        }
        public Task Handle(CompleteOrder message, IMessageHandlerContext context)
        {
            return Execute(message, context, StateMachine.CompleteOrder);
        }
        public Task Timeout(CancelOrder message, IMessageHandlerContext context)
        {
            return Execute(message, context, StateMachine.CancelOrder);
        }
    }
}
");
        }
    }
}