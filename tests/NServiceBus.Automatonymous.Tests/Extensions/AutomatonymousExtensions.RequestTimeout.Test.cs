using System;
using System.Threading.Tasks;
using AutoFixture;
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
        public void RequestTimeoutSyncWithMessageAndDateTime()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeout(new SubmitOrder(), DateTime.UtcNow).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutSyncWithMessageAndTimeSpan()
        {
            var fixture = new Fixture();
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeout(new SubmitOrder(), fixture.Create<TimeSpan>()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutSyncWithMessageFactoryAndDateTime()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeout(_ => new SubmitOrder(), DateTime.UtcNow).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>());
        }
        
        
        [Fact]
        public void RequestTimeoutSyncWithMessageFactoryAndTimeSpan()
        {
            var fixture = new Fixture();
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeout(_ => new SubmitOrder(), fixture.Create<TimeSpan>()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutAsyncWithMessageAndDateTime()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeoutAsync(Task.FromResult(new SubmitOrder()), DateTime.UtcNow).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutAsyncWithMessageAndTimeSpan()
        {
            var fixture = new Fixture();
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeoutAsync(Task.FromResult(new SubmitOrder()), fixture.Create<TimeSpan>()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutAsyncWithMessageFactoryAndDateTime()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeoutAsync(_ => Task.FromResult(new SubmitOrder()), DateTime.UtcNow).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutAsyncWithMessageFactoryAndTimeSpan()
        {
            var fixture = new Fixture();
            var binder = Substitute.For<EventActivityBinder<OrderState>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeoutAsync(_ => Task.FromResult(new SubmitOrder()), fixture.Create<TimeSpan>()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutWithOriginEventSyncWithMessageAndDateTime()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeout(new SubmitOrder(), DateTime.UtcNow).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeouthWithOriginEventSyncWithMessageAndTimeSpan()
        {
            var fixture = new Fixture();
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeout(new SubmitOrder(), fixture.Create<TimeSpan>()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutWithOriginEventSyncWithMessageFactoryAndDateTime()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeout(_ => new SubmitOrder(), DateTime.UtcNow).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutWithOriginEventSyncWithMessageFactoryAndTimeSpan()
        {
            var fixture = new Fixture();
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeout(_ => new SubmitOrder(), fixture.Create<TimeSpan>()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutWithOriginEventAsyncWithMessageAndDateTime()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeoutAsync(Task.FromResult(new SubmitOrder()), DateTime.UtcNow).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutWithOriginEventAsyncWithMessageAndTimeSpan()
        {
            var fixture = new Fixture();
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeoutAsync(Task.FromResult(new SubmitOrder()), fixture.Create<TimeSpan>()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>());
        }

        [Fact]
        public void RequestTimeoutWithOriginEventAsyncWithMessageFactoryAndDateTime()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeoutAsync(_ => Task.FromResult(new SubmitOrder()), DateTime.UtcNow).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void RequestTimeoutWithOriginEventAsyncWithMessageFactoryAndTimeSpan()
        {
            var fixture = new Fixture();
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.RequestTimeoutAsync(_ => Task.FromResult(new SubmitOrder()), fixture.Create<TimeSpan>()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<RequestTimeoutActivity<OrderState, PayOrder, SubmitOrder>>());
        }
    }
}