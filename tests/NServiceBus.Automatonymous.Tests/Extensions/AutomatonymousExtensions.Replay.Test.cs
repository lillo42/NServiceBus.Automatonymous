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
        public void ReplayWithOriginEventSyncWithMessage()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Replay(new SubmitOrder()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void ReplayWithOriginEventSyncWithMessageAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Replay(new SubmitOrder(), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void ReplayWithOriginEventSyncWithMessageFactory()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Replay(_ => new SubmitOrder()).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void ReplayWithOriginEventSyncWithMessageFactoryAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.Replay(_ => new SubmitOrder(), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void ReplayWithOriginEventAsyncWithMessage()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.ReplayAsync(Task.FromResult(new SubmitOrder())).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void ReplayWithOriginEventAsyncWithMessageAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.ReplayAsync(Task.FromResult(new SubmitOrder()), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void ReplayWithOriginEventAsyncWithMessageFactory()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.ReplayAsync(_ => Task.FromResult(new SubmitOrder())).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>());
        }
        
        [Fact]
        public void ReplayWithOriginEventAsyncWithMessageFactoryAndConfigurationOption()
        {
            var binder = Substitute.For<EventActivityBinder<OrderState, PayOrder>>();
            binder.Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>()).Returns(binder);
            binder.ReplayAsync(_ => Task.FromResult(new SubmitOrder()), (_, _) => { }).Should().Be(binder);
            binder.Received(1).Add(Arg.Any<ReplayActivity<OrderState, PayOrder, SubmitOrder>>());
        }
    }
}