using System;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Binders;
using FluentAssertions;
using NServiceBus.Automatonymous.Activities;
using NSubstitute;
using Xunit;

namespace NServiceBus.Automatonymous.Tests.Extensions
{
    public class AutomatonymousSendExtensionsTest
    {
        [Fact]
        public void SendSyncWithMessage()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.Send(new SubmitOrder()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void SendSyncWithMessageAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.Send(new SubmitOrder(), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void SendSyncWithMessageFactory()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.Send(_ => new SubmitOrder()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void SendSyncWithMessageFactoryAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.Send(_ => new SubmitOrder(), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void SendAsyncWithMessage()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.SendAsync(Task.FromResult(new SubmitOrder())).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void SendAsyncWithMessageAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.SendAsync(Task.FromResult(new SubmitOrder()), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void SendAsyncWithMessageFactory()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.SendAsync(_ => Task.FromResult(new SubmitOrder())).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void SendAsyncWithMessageFactoryAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.SendAsync(_ =>Task.FromResult(new SubmitOrder()), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void SendWithOriginEventSyncWithMessage()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Send(new SubmitOrder()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void SendWithOriginEventSyncWithMessageAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Send(new SubmitOrder(), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void SendWithOriginEventSyncWithMessageFactory()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Send(_ => new SubmitOrder()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void SendWithOriginEventSyncWithMessageFactoryAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Send(_ => new SubmitOrder(), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void SendWithOriginEventAsyncWithMessage()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.SendAsync(Task.FromResult(new SubmitOrder())).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void SendWithOriginEventAsyncWithMessageAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.SendAsync(Task.FromResult(new SubmitOrder()), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void SendWithOriginEventAsyncWithMessageFactory()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.SendAsync(_ => Task.FromResult(new SubmitOrder())).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void SendWithOriginEventAsyncWithMessageFactoryAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.SendAsync(_ =>Task.FromResult(new SubmitOrder()), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<SendActivity<OrderState, PayOrder, SubmitOrder>>());
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