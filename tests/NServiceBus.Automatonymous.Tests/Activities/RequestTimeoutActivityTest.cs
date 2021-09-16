using System;
using System.Threading.Tasks;
using AutoFixture;
using Automatonymous;
using GreenPipes;
using NServiceBus.Automatonymous.Activities;
using NSubstitute;
using Xunit;

namespace NServiceBus.Automatonymous.Tests.Activities
{
    public class RequestTimeoutActivityTest
    {
        [Fact]
        public void Ctor_Should_Throw_When_SyncFactoryIsNull()
        {
            object Create() => new RequestTimeoutActivity<OrderState, SubmitOrder>((Func<BehaviorContext<OrderState>, SubmitOrder>)null!, DateTime.UtcNow);
            Assert.Throws<ArgumentNullException>((Func<object>)Create);
            
            object Create2() => new RequestTimeoutActivity<OrderState, SubmitOrder>((Func<BehaviorContext<OrderState>, SubmitOrder>)null!, TimeSpan.Zero);
            Assert.Throws<ArgumentNullException>((Func<object>)Create2);
        }
        
        [Fact]
        public void Ctor_Should_Throw_When_AsyncFactoryIsNull()
        {
            object Create() => new RequestTimeoutActivity<OrderState, SubmitOrder>((Func<BehaviorContext<OrderState>, Task<SubmitOrder>>)null!, DateTime.UtcNow);
            Assert.Throws<ArgumentNullException>((Func<object>)Create);
            
            object Create2() => new RequestTimeoutActivity<OrderState, SubmitOrder>((Func<BehaviorContext<OrderState>, Task<SubmitOrder>>)null!, TimeSpan.Zero);
            Assert.Throws<ArgumentNullException>((Func<object>)Create2);
        }

        [Fact]
        public void Probe_Should_CreateScope()
        {
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => new SubmitOrder(), DateTime.UtcNow);
            
            var context = Substitute.For<ProbeContext>();
            
            activity.Probe(context);

            context.Received(1).CreateScope("request-timeout");
        }
        
        [Fact]
        public void Accept_Should_Visit()
        {
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => new SubmitOrder(), DateTime.UtcNow);
            
            var visitor = Substitute.For<StateMachineVisitor>();
            
            activity.Accept(visitor);

            visitor.Received(1).Visit(activity);
        }

        [Fact]
        public async Task Faulted_Should_Faulted()
        {
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => new SubmitOrder(), DateTime.UtcNow);
            
            var context = Substitute.For<BehaviorExceptionContext<OrderState, Exception>>();
            var next = Substitute.For<Behavior<OrderState>>();
            
            await activity.Faulted(context, next);
            
            await next.Received(1).Faulted(context);
        }
        
        [Fact]
        public async Task FaultedGeneric_Should_Faulted()
        {
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => new SubmitOrder(), DateTime.UtcNow);
            
            var context = Substitute.For<BehaviorExceptionContext<OrderState, PayOrder, Exception>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();
            
            await activity.Faulted(context, next);
            
            await next.Received(1).Faulted(context);
        }
        
        [Fact]
        public async Task Execute_Should_RequestTimeoutMessageWithSyncFactoryAndWithDateTime()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = DateTime.UtcNow;
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => message, timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }
        
        [Fact]
        public async Task Execute_Should_RequestTimeoutMessageWithSyncFactoryAndWitTimeSpan()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = fixture.Create<TimeSpan>();
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => message, timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }
        
        [Fact]
        public async Task Execute_Should_RequestTimeoutMessageWithAsyncFactoryAndWithDateTime()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = DateTime.UtcNow;
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => Task.FromResult(message), timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }
        
        [Fact]
        public async Task Execute_Should_RequestTimeoutMessageWithAsyncFactoryAndWitTimeSpan()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = fixture.Create<TimeSpan>();
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => Task.FromResult(message), timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }

        [Fact]
        public async Task ExecuteWithOriginMessage_Should_RequestTimeoutMessageWithSyncFactoryAndWithDateTime()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = DateTime.UtcNow;
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => message, timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_RequestTimeoutMessageWithSyncFactoryAndWitTimeSpan()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = fixture.Create<TimeSpan>();
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => message, timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_RequestTimeoutMessageWithAsyncFactoryAndWithDateTime()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = DateTime.UtcNow;
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => Task.FromResult(message), timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_RequestTimeoutMessageWithAsyncFactoryAndWitTimeSpan()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = fixture.Create<TimeSpan>();
            var activity = new RequestTimeoutActivity<OrderState, SubmitOrder>(_ => Task.FromResult(message), timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }

        public class SubmitOrder : IMessage
        {
            public Guid OrderId { get; set; }
        }
        
        public class PayOrder : IMessage
        {
            public Guid OrderId { get; set; }
        }
        
        public class OrderState : ContainSagaData
        {
            public Guid CorrelationId { get; set; }
        }
    }

    public class RequestTimeoutActivityWithOriginMessageTest
    {
        [Fact]
        public void Ctor_Should_Throw_When_SyncFactoryIsNull()
        {
            object Create() => new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>((Func<BehaviorContext<OrderState>, SubmitOrder>)null!, DateTime.UtcNow);
            Assert.Throws<ArgumentNullException>((Func<object>)Create);
            
            object Create2() => new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>((Func<BehaviorContext<OrderState>, SubmitOrder>)null!, TimeSpan.Zero);
            Assert.Throws<ArgumentNullException>((Func<object>)Create2);
        }
        
        [Fact]
        public void Ctor_Should_Throw_When_AsyncFactoryIsNull()
        {
            object Create() => new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>((Func<BehaviorContext<OrderState>, Task<SubmitOrder>>)null!, DateTime.UtcNow);
            Assert.Throws<ArgumentNullException>((Func<object>)Create);
            
            object Create2() => new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>((Func<BehaviorContext<OrderState>, Task<SubmitOrder>>)null!, TimeSpan.Zero);
            Assert.Throws<ArgumentNullException>((Func<object>)Create2);
        }

        [Fact]
        public void Probe_Should_CreateScope()
        {
            var activity = new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>(_ => new SubmitOrder(), DateTime.UtcNow);
            
            var context = Substitute.For<ProbeContext>();
            
            activity.Probe(context);

            context.Received(1).CreateScope("request-timeout");
        }
        
        [Fact]
        public void Accept_Should_Visit()
        {
            var activity = new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>(_ => new SubmitOrder(), DateTime.UtcNow);
            
            var visitor = Substitute.For<StateMachineVisitor>();
            
            activity.Accept(visitor);

            visitor.Received(1).Visit(activity);
        }

        [Fact]
        public async Task Faulted_Should_Faulted()
        {
            var activity = new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>(_ => new SubmitOrder(), DateTime.UtcNow);
            
            var context = Substitute.For<BehaviorExceptionContext<OrderState, PayOrder, Exception>>();
            var next = Substitute.For<Behavior<OrderState, PayOrder>>();
            
            await activity.Faulted(context, next);
            
            await next.Received(1).Faulted(context);
        }

        [Fact]
        public async Task ExecuteWithOriginMessage_Should_RequestTimeoutMessageWithSyncFactoryAndWithDateTime()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = DateTime.UtcNow;
            var activity = new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>(_ => message, timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_RequestTimeoutMessageWithSyncFactoryAndWitTimeSpan()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = fixture.Create<TimeSpan>();
            var activity = new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>(_ => message, timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_RequestTimeoutMessageWithAsyncFactoryAndWithDateTime()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = DateTime.UtcNow;
            var activity = new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>(_ => Task.FromResult(message), timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }
        
        [Fact]
        public async Task ExecuteWithOriginMessage_Should_RequestTimeoutMessageWithAsyncFactoryAndWitTimeSpan()
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
            
            context.TryGetPayload(out Arg.Any<SagaType>())
                .Returns(x =>
                {
                    x[0] = new SagaType(GetType());
                    return true;
                });

            var orderState = fixture.Create<OrderState>();
            context.Instance.Returns(orderState);
            

            var timeout = fixture.Create<TimeSpan>();
            var activity = new RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>(_ => Task.FromResult(message), timeout);
            await activity.Execute(context, next);
            await messageHandlerContext.Received(1).Send(message, Arg.Is<SendOptions>(opt => 
                opt.GetHeaders()[Headers.SagaId] == orderState.Id.ToString() 
                && opt.GetHeaders()[Headers.IsSagaTimeoutMessage] == bool.TrueString
                && opt.GetHeaders()[Headers.SagaType] == GetType().AssemblyQualifiedName
            ));
        }

        public class SubmitOrder : IMessage
        {
            public Guid OrderId { get; set; }
        }
        
        public class PayOrder : IMessage
        {
            public Guid OrderId { get; set; }
        }
        
        public class OrderState : ContainSagaData
        {
            public Guid CorrelationId { get; set; }
        }
    }
}