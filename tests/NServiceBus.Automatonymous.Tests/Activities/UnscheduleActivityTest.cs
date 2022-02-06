using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes;
using NServiceBus.Automatonymous.Activities;
using NSubstitute;
using Xunit;

namespace NServiceBus.Automatonymous.Tests.Activities;

public class UnscheduleActivityTest
{
    [Fact]
    public void Ctor_Should_Throw_When_SchedulerIsNull()
    {
        object Create() => new UnscheduleActivity<OrderState>(null!);
        Assert.Throws<ArgumentNullException>((Func<object>)Create);
    }

    [Fact]
    public void Probe_Should_CreateScope()
    {
        var activity = new UnscheduleActivity<OrderState>(Substitute.For<Schedule<OrderState>>());
            
        var context = Substitute.For<ProbeContext>();
            
        activity.Probe(context);

        context.Received(1).CreateScope("unschedule");
    }
        
    [Fact]
    public void Accept_Should_Visit()
    {
        var activity = new UnscheduleActivity<OrderState>(Substitute.For<Schedule<OrderState>>());

        var visitor = Substitute.For<StateMachineVisitor>();
            
        activity.Accept(visitor);

        visitor.Received(1).Visit(activity);
    }

    [Fact]
    public async Task Faulted_Should_Faulted()
    {
        var activity = new UnscheduleActivity<OrderState>(Substitute.For<Schedule<OrderState>>());

        var context = Substitute.For<BehaviorExceptionContext<OrderState, PaymentTimeout, Exception>>();
        var next = Substitute.For<Behavior<OrderState, PaymentTimeout>>();
            
        await activity.Faulted(context, next);
            
        await next.Received(1).Faulted(context);
    }

    [Fact]
    public async Task Execute_Should_DoNothing_When_PreviousTokenIsNull()
    {
        var next = Substitute.For<Behavior<OrderState>>();
            
        var context = Substitute.For<BehaviorContext<OrderState>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns((Guid?)null);

        var activity = new UnscheduleActivity<OrderState>(scheduler);
        await activity.Execute(context, next);

        scheduler
            .Received(1)
            .GetTokenId(instance);
    }
        
    [Fact]
    public async Task Execute_Should_DoNothing_When_MessageTokenIdIsEqualThanPreviousId()
    {
        var tokenId = Guid.NewGuid();
        var next = Substitute.For<Behavior<OrderState>>();
            
        var context = Substitute.For<BehaviorContext<OrderState>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(tokenId);

        var handler = Substitute.For<IMessageHandlerContext>();
        handler.MessageHeaders.Returns(new Dictionary<string, string>
        {
            [MessageHeaders.SchedulingTokenId] = tokenId.ToString()
        });
            
        context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
            .Returns(x =>
            {
                x[0] = handler;
                return true;
            });
            
        var activity = new UnscheduleActivity<OrderState>(scheduler);
        await activity.Execute(context, next);

        scheduler
            .Received(1)
            .GetTokenId(instance);
    }

    [Fact]
    public async Task Execute_Should_CancelScheduler_When_MessageIdIsNull()
    {
        var tokenId = Guid.NewGuid();
        var next = Substitute.For<Behavior<OrderState>>();
            
        var context = Substitute.For<BehaviorContext<OrderState>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(tokenId);

        var handler = Substitute.For<IMessageHandlerContext>();
        handler.MessageHeaders.Returns(new Dictionary<string, string>());
        context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
            .Returns(x =>
            {
                x[0] = handler;
                return true;
            });
            
        var messageSchedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = messageSchedulerContext;
                return true;
            });
            
        var activity = new UnscheduleActivity<OrderState>(scheduler);
        await activity.Execute(context, next);

        scheduler
            .Received(1)
            .GetTokenId(instance);

        await messageSchedulerContext
            .Received(1)
            .CancelScheduledSendAsync(tokenId, Arg.Any<CancellationToken>());
            
            
        scheduler
            .Received(1)
            .SetTokenId(instance, null);
    }
        
    [Fact]
    public async Task Execute_Should_CancelScheduler_When_MessageIdIsDifferentThanPrevious()
    {
        var tokenId = Guid.NewGuid();
        var next = Substitute.For<Behavior<OrderState>>();
            
        var context = Substitute.For<BehaviorContext<OrderState>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(tokenId);

        var handler = Substitute.For<IMessageHandlerContext>();
        handler.MessageHeaders.Returns(new Dictionary<string, string>
        {
            [MessageHeaders.SchedulingTokenId] = Guid.NewGuid().ToString()
        });
        context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
            .Returns(x =>
            {
                x[0] = handler;
                return true;
            });
            
        var messageSchedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = messageSchedulerContext;
                return true;
            });
            
        var activity = new UnscheduleActivity<OrderState>(scheduler);
        await activity.Execute(context, next);

        scheduler
            .Received(1)
            .GetTokenId(instance);

        await messageSchedulerContext
            .Received(1)
            .CancelScheduledSendAsync(tokenId, Arg.Any<CancellationToken>());
            
            
        scheduler
            .Received(1)
            .SetTokenId(instance, null);
    }
        
    [Fact]
    public async Task ExecuteWithOriginalMessage_Should_DoNothing_When_PreviousTokenIsNull()
    {
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns((Guid?)null);

        var activity = new UnscheduleActivity<OrderState>(scheduler);
        await activity.Execute(context, next);

        scheduler
            .Received(1)
            .GetTokenId(instance);
    }
        
    [Fact]
    public async Task ExecuteWithOriginalMessage_Should_DoNothing_When_MessageTokenIdIsEqualThanPreviousId()
    {
        var tokenId = Guid.NewGuid();
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(tokenId);

        var handler = Substitute.For<IMessageHandlerContext>();
        handler.MessageHeaders.Returns(new Dictionary<string, string>
        {
            [MessageHeaders.SchedulingTokenId] = tokenId.ToString()
        });
            
        context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
            .Returns(x =>
            {
                x[0] = handler;
                return true;
            });
            
        var activity = new UnscheduleActivity<OrderState>(scheduler);
        await activity.Execute(context, next);

        scheduler
            .Received(1)
            .GetTokenId(instance);
    }

    [Fact]
    public async Task ExecuteWithOriginalMessage_Should_CancelScheduler_When_MessageIdIsNull()
    {
        var tokenId = Guid.NewGuid();
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(tokenId);

        var handler = Substitute.For<IMessageHandlerContext>();
        handler.MessageHeaders.Returns(new Dictionary<string, string>());
        context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
            .Returns(x =>
            {
                x[0] = handler;
                return true;
            });
            
        var messageSchedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = messageSchedulerContext;
                return true;
            });
            
        var activity = new UnscheduleActivity<OrderState>(scheduler);
        await activity.Execute(context, next);

        scheduler
            .Received(1)
            .GetTokenId(instance);

        await messageSchedulerContext
            .Received(1)
            .CancelScheduledSendAsync(tokenId, Arg.Any<CancellationToken>());
            
            
        scheduler
            .Received(1)
            .SetTokenId(instance, null);
    }
        
    [Fact]
    public async Task ExecuteWithOriginalMessage_Should_CancelScheduler_When_MessageIdIsDifferentThanPrevious()
    {
        var tokenId = Guid.NewGuid();
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(tokenId);

        var handler = Substitute.For<IMessageHandlerContext>();
        handler.MessageHeaders.Returns(new Dictionary<string, string>
        {
            [MessageHeaders.SchedulingTokenId] = Guid.NewGuid().ToString()
        });
        context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
            .Returns(x =>
            {
                x[0] = handler;
                return true;
            });
            
        var messageSchedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = messageSchedulerContext;
                return true;
            });
            
        var activity = new UnscheduleActivity<OrderState>(scheduler);
        await activity.Execute(context, next);

        scheduler
            .Received(1)
            .GetTokenId(instance);

        await messageSchedulerContext
            .Received(1)
            .CancelScheduledSendAsync(tokenId, Arg.Any<CancellationToken>());
            
            
        scheduler
            .Received(1)
            .SetTokenId(instance, null);
    }

        
    public class OriginalMessage : IMessage { }
        
    public class PaymentTimeout : IMessage
    {
        public Guid OrderId { get; set; }
    }
        
    public class OrderState : ContainSagaData
    {
        public Guid CorrelationId { get; set; }
        public Guid? PaymentTimeoutToken { get; set; }
    }
}