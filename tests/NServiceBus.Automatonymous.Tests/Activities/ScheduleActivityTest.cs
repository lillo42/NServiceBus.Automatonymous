using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Automatonymous;
using GreenPipes;
using NServiceBus.Automatonymous.Activities;
using NSubstitute;
using Xunit;

namespace NServiceBus.Automatonymous.Tests.Activities;

public class ScheduleActivityTest
{
    [Fact]
    public void Ctor_Should_Throw_When_TimeProviderIsNull()
    {
        object Create() => new ScheduleActivity<OrderState, PaymentTimeout>((Func<BehaviorContext<OrderState>, PaymentTimeout>)null!, null!, null!);
        Assert.Throws<ArgumentNullException>((Func<object>)Create);
    }
        
    [Fact]
    public void Ctor_Should_Throw_When_SchedulerIsNull()
    {
        object Create() => new ScheduleActivity<OrderState, PaymentTimeout>((Func<BehaviorContext<OrderState>, PaymentTimeout>)null!, null!, _ => DateTime.UtcNow);
        Assert.Throws<ArgumentNullException>((Func<object>)Create);
    }
        
    [Fact]
    public void Ctor_Should_Throw_When_SyncFactoryIsNull()
    {
        object Create() => new ScheduleActivity<OrderState, PaymentTimeout>((Func<BehaviorContext<OrderState>, PaymentTimeout>)null!, 
            Substitute.For<Schedule<OrderState>>(), _ => DateTime.UtcNow);
        Assert.Throws<ArgumentNullException>((Func<object>)Create);
    }
        
    [Fact]
    public void Ctor_Should_Throw_When_AsyncFactoryIsNull()
    {
        object Create() => new ScheduleActivity<OrderState, PaymentTimeout>((Func<BehaviorContext<OrderState>, Task<PaymentTimeout>>)null!, 
            Substitute.For<Schedule<OrderState>>(), _ => DateTime.UtcNow);
        Assert.Throws<ArgumentNullException>((Func<object>)Create);
    }

    [Fact]
    public void Probe_Should_CreateScope()
    {
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => new PaymentTimeout(),
            Substitute.For<Schedule<OrderState>>(), _ => DateTime.UtcNow);
            
        var context = Substitute.For<ProbeContext>();
            
        activity.Probe(context);

        context.Received(1).CreateScope("schedule");
    }
        
    [Fact]
    public void Accept_Should_Visit()
    {
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => new PaymentTimeout(),
            Substitute.For<Schedule<OrderState>>(), _ => DateTime.UtcNow);
            
        var visitor = Substitute.For<StateMachineVisitor>();
            
        activity.Accept(visitor);

        visitor.Received(1).Visit(activity);
    }

    [Fact]
    public async Task Faulted_Should_Faulted()
    {
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => new PaymentTimeout(), 
            Substitute.For<Schedule<OrderState>>(), _ => DateTime.UtcNow);
            
        var context = Substitute.For<BehaviorExceptionContext<OrderState, PaymentTimeout, Exception>>();
        var next = Substitute.For<Behavior<OrderState, PaymentTimeout>>();
            
        await activity.Faulted(context, next);
            
        await next.Received(1).Faulted(context);
    }

    [Fact]
    public async Task Execute_Should_SchedulerMessageWithSyncFactoryAndHasNotPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState>>();
            
        var context = Substitute.For<BehaviorContext<OrderState>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns((Guid?)null);

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => message, scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .DidNotReceive()
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
        
    [Fact]
    public async Task Execute_Should_SchedulerMessageWithSyncFactoryAndHasPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState>>();
            
        var context = Substitute.For<BehaviorContext<OrderState>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(fixture.Create<Guid>());

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => message, scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .Received(1)
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
        
    [Fact]
    public async Task Execute_Should_SchedulerMessageWithAsyncFactoryAndHasNotPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState>>();
            
        var context = Substitute.For<BehaviorContext<OrderState>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns((Guid?)null);

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => Task.FromResult(message), scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .DidNotReceive()
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
        
    [Fact]
    public async Task Execute_Should_SchedulerMessageWithAsyncFactoryAndHasPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState>>();
            
        var context = Substitute.For<BehaviorContext<OrderState>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(fixture.Create<Guid>());

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => Task.FromResult(message), scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .Received(1)
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
        
    [Fact]
    public async Task ExecuteWithOriginalMessage_Should_SchedulerMessageWithSyncFactoryAndHasNotPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState, OriginalMessage>>();
        scheduler.GetTokenId(instance)
            .Returns((Guid?)null);

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => message, scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .DidNotReceive()
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
        
    [Fact]
    public async Task ExecuteWithOriginalMessage_Should_SchedulerMessageWithSyncFactoryAndHasPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(fixture.Create<Guid>());

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => message, scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .Received(1)
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
        
    [Fact]
    public async Task ExecuteWithOriginalMessage_Should_SchedulerMessageWithAsyncFactoryAndHasNotPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns((Guid?)null);

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => Task.FromResult(message), scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .DidNotReceive()
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
        
    [Fact]
    public async Task ExecuteWithOriginalMessage_Should_SchedulerMessageWithAsyncFactoryAndHasPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(fixture.Create<Guid>());

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, PaymentTimeout>(_ => Task.FromResult(message), scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .Received(1)
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
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
    
public class ScheduleActivityWithOriginalMessageTest
{
    [Fact]
    public void Ctor_Should_Throw_When_TimeProviderIsNull()
    {
        object Create() => new ScheduleActivity<OrderState, OriginalMessage, PaymentTimeout>((Func<BehaviorContext<OrderState>, PaymentTimeout>)null!, null!, null!);
        Assert.Throws<ArgumentNullException>((Func<object>)Create);
    }
        
    [Fact]
    public void Ctor_Should_Throw_When_SchedulerIsNull()
    {
        object Create() => new ScheduleActivity<OrderState, OriginalMessage, PaymentTimeout>((Func<BehaviorContext<OrderState>, PaymentTimeout>)null!, null!, _ => DateTime.UtcNow);
        Assert.Throws<ArgumentNullException>((Func<object>)Create);
    }
        
    [Fact]
    public void Ctor_Should_Throw_When_SyncFactoryIsNull()
    {
        object Create() => new ScheduleActivity<OrderState, OriginalMessage,  PaymentTimeout>((Func<BehaviorContext<OrderState>, PaymentTimeout>)null!, 
            Substitute.For<Schedule<OrderState>>(), _ => DateTime.UtcNow);
        Assert.Throws<ArgumentNullException>((Func<object>)Create);
    }
        
    [Fact]
    public void Ctor_Should_Throw_When_AsyncFactoryIsNull()
    {
        object Create() => new ScheduleActivity<OrderState, PaymentTimeout>((Func<BehaviorContext<OrderState>, Task<PaymentTimeout>>)null!, 
            Substitute.For<Schedule<OrderState>>(), _ => DateTime.UtcNow);
        Assert.Throws<ArgumentNullException>((Func<object>)Create);
    }

    [Fact]
    public void Probe_Should_CreateScope()
    {
        var activity = new ScheduleActivity<OrderState, OriginalMessage, PaymentTimeout>(_ => new PaymentTimeout(),
            Substitute.For<Schedule<OrderState>>(), _ => DateTime.UtcNow);
            
        var context = Substitute.For<ProbeContext>();
            
        activity.Probe(context);

        context.Received(1).CreateScope("schedule");
    }
        
    [Fact]
    public void Accept_Should_Visit()
    {
        var activity = new ScheduleActivity<OrderState, OriginalMessage, PaymentTimeout>(_ => new PaymentTimeout(),
            Substitute.For<Schedule<OrderState>>(), _ => DateTime.UtcNow);
            
        var visitor = Substitute.For<StateMachineVisitor>();
            
        activity.Accept(visitor);

        visitor.Received(1).Visit(activity);
    }

    [Fact]
    public async Task Faulted_Should_Faulted()
    {
        var activity = new ScheduleActivity<OrderState, OriginalMessage, PaymentTimeout>(_ => new PaymentTimeout(), 
            Substitute.For<Schedule<OrderState>>(), _ => DateTime.UtcNow);
            
        var context = Substitute.For<BehaviorExceptionContext<OrderState, OriginalMessage, Exception>>();
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        await activity.Faulted(context, next);
            
        await next.Received(1).Faulted(context);
    }

    [Fact]
    public async Task Execute_Should_SchedulerMessageWithSyncFactoryAndHasNotPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns((Guid?)null);

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, OriginalMessage, PaymentTimeout>(_ => message, scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .DidNotReceive()
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
        
    [Fact]
    public async Task Execute_Should_SchedulerMessageWithSyncFactoryAndHasPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(fixture.Create<Guid>());

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, OriginalMessage, PaymentTimeout>(_ => message, scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .Received(1)
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
        
    [Fact]
    public async Task Execute_Should_SchedulerMessageWithAsyncFactoryAndHasNotPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns((Guid?)null);

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, OriginalMessage, PaymentTimeout>(_ => Task.FromResult(message), scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .DidNotReceive()
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
        
    [Fact]
    public async Task Execute_Should_SchedulerMessageWithAsyncFactoryAndHasPreviousToken()
    {
        var fixture = new Fixture();
        var message = fixture.Create<PaymentTimeout>();
            
        var next = Substitute.For<Behavior<OrderState, OriginalMessage>>();
            
        var context = Substitute.For<BehaviorContext<OrderState, OriginalMessage>>();
        var instance = new OrderState();
        context.Instance.Returns(instance);

        var scheduler = Substitute.For<Schedule<OrderState>>();
        scheduler.GetTokenId(instance)
            .Returns(fixture.Create<Guid>());

        var schedulerContext = Substitute.For<MessageSchedulerContext>();
        context.TryGetPayload(out Arg.Any<MessageSchedulerContext>())
            .Returns(x =>
            {
                x[0] = schedulerContext;
                return true;
            });

        var schedulerMessage = Substitute.For<ScheduledMessage<PaymentTimeout>>();
        var time = fixture.Create<DateTime>();
        schedulerContext.ScheduleSendAsync(time, message)
            .Returns(Task.FromResult(schedulerMessage));
            
        var activity = new ScheduleActivity<OrderState, OriginalMessage, PaymentTimeout>(_ => Task.FromResult(message), scheduler, _ => time);
        await activity.Execute(context, next);

        await schedulerContext
            .Received(1)
            .ScheduleSendAsync(time, message);
            
        scheduler
            .Received(1)
            .SetTokenId(Arg.Any<OrderState>(), Arg.Any<Guid>());

        await schedulerContext
            .Received(1)
            .CancelScheduledSendAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    public class OriginalMessage : IMessage
    {
            
    }
        
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