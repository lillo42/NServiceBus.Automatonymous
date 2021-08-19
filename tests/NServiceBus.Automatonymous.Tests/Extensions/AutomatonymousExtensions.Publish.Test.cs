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
    public partial class AutomatonymousExtensionsTest
    {
        [Fact]
        public void PublishSyncWithMessage()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.Publish(new SubmitOrder()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishSyncWithMessageAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.Publish(new SubmitOrder(), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishSyncWithMessageFactory()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.Publish(_ => new SubmitOrder()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishSyncWithMessageFactoryAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.Publish(_ => new SubmitOrder(), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishAsyncWithMessage()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.PublishAsync(Task.FromResult(new SubmitOrder())).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishAsyncWithMessageAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.PublishAsync(Task.FromResult(new SubmitOrder()), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishAsyncWithMessageFactory()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.PublishAsync(_ => Task.FromResult(new SubmitOrder())).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishAsyncWithMessageFactoryAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.PublishAsync(_ =>Task.FromResult(new SubmitOrder()), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishWithOriginEventSyncWithMessage()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Publish(new SubmitOrder()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishWithOriginEventSyncWithMessageAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Publish(new SubmitOrder(), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishWithOriginEventSyncWithMessageFactory()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Publish(_ => new SubmitOrder()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishWithOriginEventSyncWithMessageFactoryAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Publish(_ => new SubmitOrder(), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishWithOriginEventAsyncWithMessage()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.PublishAsync(Task.FromResult(new SubmitOrder())).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishWithOriginEventAsyncWithMessageAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.PublishAsync(Task.FromResult(new SubmitOrder()), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishWithOriginEventAsyncWithMessageFactory()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.PublishAsync(_ => Task.FromResult(new SubmitOrder())).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void PublishWithOriginEventAsyncWithMessageFactoryAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.PublishAsync(_ =>Task.FromResult(new SubmitOrder()), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<PublishActivity<OrderState, PayOrder, SubmitOrder>>());
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