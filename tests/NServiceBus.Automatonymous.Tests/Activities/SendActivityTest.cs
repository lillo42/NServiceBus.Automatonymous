using System;
using System.Threading.Tasks;
using AutoFixture;
using Automatonymous;
using FluentAssertions;
using GreenPipes;
using NServiceBus.Automatonymous.Activities;
using NSubstitute;
using Xunit;

namespace NServiceBus.Automatonymous.Tests.Activities
{
    public class SendActivityTest
    {
        [Fact]
        public void Ctor_Should_Throw_When_SyncFactoryIsNull()
        {
            object Create() => new SendActivity<OrderState, SubmitOrder>((Func<BehaviorContext<OrderState>, SubmitOrder>)null!, null);
            Assert.Throws<ArgumentNullException>((Func<object>)Create);
        }
        
        [Fact]
        public void Ctor_Should_Throw_When_AsyncFactoryIsNull()
        {
            object Create() => new SendActivity<OrderState, SubmitOrder>((Func<BehaviorContext<OrderState>, Task<SubmitOrder>>)null!, null);
            Assert.Throws<ArgumentNullException>((Func<object>)Create);
        }

        [Fact]
        public void Probe_Should_CreateScope()
        {
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => new SubmitOrder(), null);
            
            var context = Substitute.For<ProbeContext>();
            
            activity.Probe(context);

            context.Received(1).CreateScope("send");
        }
        
        [Fact]
        public void Accept_Should_Visit()
        {
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => new SubmitOrder(), null);
            
            var visitor = Substitute.For<StateMachineVisitor>();
            
            activity.Accept(visitor);

            visitor.Received(1).Visit(activity);
        }

        [Fact]
        public async Task Faulted_Should_Faulted()
        {
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => new SubmitOrder(), null);
            
            var context = Substitute.For<BehaviorExceptionContext<OrderState, Exception>>();
            var next = Substitute.For<Behavior<OrderState>>();
            
            await activity.Faulted(context, next);
            
            await next.Received(1).Faulted(context);
        }
        
        [Fact]
        public async Task FaultedGeneric_Should_Faulted()
        {
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => new SubmitOrder(), null);
            
            var context = Substitute.For<BehaviorExceptionContext<OrderState, PayOrder, Exception>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();
            
            await activity.Faulted(context, next);
            
            await next.Received(1).Faulted(context);
        }
        
        [Fact]
        public async Task Execute_Should_SendMessageWithSyncFactoryAndConfigSendOptionIsNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState>>();
            var next = Substitute.For<Behavior<OrderState>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });
            
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => message, null);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
        }
        
        [Fact]
        public async Task Execute_Should_SendMessageWithSyncFactoryAndConfigSendOptionIsNotNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState>>();
            var next = Substitute.For<Behavior<OrderState>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });

            var called = false;
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => message, (ctx, opt) =>
            {
                ctx.Should().NotBeNull();
                opt.Should().NotBeNull();
                called = true;
            });
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
            called.Should().BeTrue();
        }
        
        [Fact]
        public async Task Execute_Should_SendMessageWithAsyncFactoryAndConfigSendOptionIsNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState>>();
            var next = Substitute.For<Behavior<OrderState>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });
            
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => Task.FromResult(message), null);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
        }
        
        [Fact]
        public async Task Execute_Should_SendMessageWithAsyncFactoryAndConfigSendOptionIsNotNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState>>();
            var next = Substitute.For<Behavior<OrderState>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });
            
            var called = false;
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => Task.FromResult(message), (ctx, opt) =>
            {
                ctx.Should().NotBeNull();
                opt.Should().NotBeNull();
                called = true;
            });
            
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
            called.Should().BeTrue();
        }
        
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_SendMessageWithSyncFactoryAndConfigSendOptionIsNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState, PayOrder>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });
            
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => message, null);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_SendMessageWithSyncFactoryAndConfigSendOptionIsNotNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState, PayOrder>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });

            var called = false;
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => message, (ctx, opt) =>
            {
                ctx.Should().NotBeNull();
                opt.Should().NotBeNull();
                called = true;
            });
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
            called.Should().BeTrue();
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_SendMessageWithAsyncFactoryAndConfigSendOptionIsNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState, PayOrder>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });
            
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => Task.FromResult(message), null);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_SendMessageWithAsyncFactoryAndConfigSendOptionIsNotNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState, PayOrder>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });
            
            var called = false;
            var activity = new SendActivity<OrderState, SubmitOrder>(_ => Task.FromResult(message), (ctx, opt) =>
            {
                ctx.Should().NotBeNull();
                opt.Should().NotBeNull();
                called = true;
            });
            
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
            called.Should().BeTrue();
        }

        public class SubmitOrder
        {
            public Guid OrderId { get; set; }
        }
        
        public class PayOrder
        {
            public Guid OrderId { get; set; }
        }
        
        public class OrderState : ContainSagaData
        {
            public Guid CorrelationId { get; set; }
        }
    }

    public class SendActivityWithOriginMessageTest
    {
        [Fact]
        public void Ctor_Should_Throw_When_SyncFactoryIsNull()
        {
            object Create() => new SendActivity<OrderState, PayOrder, SubmitOrder>((Func<BehaviorContext<OrderState, PayOrder>, SubmitOrder>)null!, null);
            Assert.Throws<ArgumentNullException>((Func<object>)Create);
        }
        
        [Fact]
        public void Ctor_Should_Throw_When_AsyncFactoryIsNull()
        {
            object Create() => new SendActivity<OrderState, PayOrder, SubmitOrder>((Func<BehaviorContext<OrderState, PayOrder>, Task<SubmitOrder>>)null!, null);
            Assert.Throws<ArgumentNullException>((Func<object>)Create);
        }

        [Fact]
        public void Probe_Should_CreateScope()
        {
            var activity = new SendActivity<OrderState, PayOrder, SubmitOrder>(_ => new SubmitOrder(), null);
            
            var context = Substitute.For<ProbeContext>();
            
            activity.Probe(context);

            context.Received(1).CreateScope("send");
        }
        
        [Fact]
        public void Accept_Should_Visit()
        {
            var activity = new SendActivity<OrderState, PayOrder, SubmitOrder>(_ => new SubmitOrder(), null);
            
            var visitor = Substitute.For<StateMachineVisitor>();
            
            activity.Accept(visitor);

            visitor.Received(1).Visit(activity);
        }

        [Fact]
        public async Task Faulted_Should_Faulted()
        {
            var activity = new SendActivity<OrderState, PayOrder, SubmitOrder>(_ => new SubmitOrder(), null);
            
            var context = Substitute.For<BehaviorExceptionContext<OrderState, PayOrder, Exception>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();
            
            await activity.Faulted(context, next);
            
            await next.Received(1).Faulted(context);
        }

        [Fact]
        public async Task ExecuteWithOriginMessage_Should_SendMessageWithSyncFactoryAndConfigSendOptionIsNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState, PayOrder>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });
            
            var activity = new SendActivity<OrderState, PayOrder, SubmitOrder>(_ => message, null);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_SendMessageWithSyncFactoryAndConfigSendOptionIsNotNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState, PayOrder>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });

            var called = false;
            var activity = new SendActivity<OrderState, PayOrder,SubmitOrder>(_ => message, (ctx, opt) =>
            {
                ctx.Should().NotBeNull();
                opt.Should().NotBeNull();
                called = true;
            });
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
            called.Should().BeTrue();
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_SendMessageWithAsyncFactoryAndConfigSendOptionIsNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState, PayOrder>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });
            
            var activity = new SendActivity<OrderState, PayOrder, SubmitOrder>(_ => Task.FromResult(message), null);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_SendMessageWithAsyncFactoryAndConfigSendOptionIsNotNull()
        {
            var fixture = new Fixture();
            var message = fixture.Create<SubmitOrder>();
            
            var context = Substitute.For<BehaviorContext<OrderState, PayOrder>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();

            var messageHandlerContext = Substitute.For<IMessageHandlerContext>();
            context.TryGetPayload(out Arg.Any<IMessageHandlerContext>())
                .Returns(x =>
                {
                    x[0] = messageHandlerContext;
                    return true;
                });
            
            var called = false;
            var activity = new SendActivity<OrderState, PayOrder, SubmitOrder>(_ => Task.FromResult(message), (ctx, opt) =>
            {
                ctx.Should().NotBeNull();
                opt.Should().NotBeNull();
                called = true;
            });
            
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Any<SendOptions>());
            called.Should().BeTrue();
        }

        public class SubmitOrder
        {
            public Guid OrderId { get; set; }
        }
        
        public class PayOrder
        {
            public Guid OrderId { get; set; }
        }
        
        public class OrderState : ContainSagaData
        {
            public Guid CorrelationId { get; set; }
        }
    }
}