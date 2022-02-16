using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using NServiceBus.Automatonymous.Events;
using NServiceBus.Automatonymous.Exceptions;
using NServiceBus.Settings;
using NServiceBus.Testing;
using NSubstitute;
using Xunit;

namespace NServiceBus.Automatonymous.Tests.Events;

public class EventMissingInstanceConfiguratorTest
{
    private readonly EventMissingInstanceConfigurator<ISomeState, ISomeMessage> _event;

    public EventMissingInstanceConfiguratorTest()
    {
        _event = new();
    }

    [Fact]
    public async Task Discard()
    {
        _event.Discard();
        var action = _event.Build();

        var message = Substitute.For<ISomeMessage>();
        var context = new TestableMessageHandlerContext();
        action.Should().NotBeNull();
        await action!(message, context);
    }
    
    [Fact]
    public async Task Fault_Should_Throw_When_ErrorQueueIsNotSet()
    {
        _event.Fault();
        var action = _event.Build();

        var message = Substitute.For<ISomeMessage>();
        var context = new TestableMessageHandlerContext();
        
        var setting = Substitute.For<ReadOnlySettings>();
        setting.TryGet(ErrorQueueSettings.SettingsKey, out Arg.Any<string>())
            .Returns(false);
        
        context.Extensions.Set(setting);

        
        action.Should().NotBeNull();
        await Assert.ThrowsAsync<DeadQueueNotSetupException>(() => action!(message, context));
    }
    
    [Fact]
    public async Task Fault()
    {
        _event.Fault();
        var action = _event.Build();

        var message = Substitute.For<ISomeMessage>();
        var context = new TestableMessageHandlerContext();

        var queue = new Fixture().Create<string>();
        var setting = Substitute.For<ReadOnlySettings>();
        setting.TryGet(ErrorQueueSettings.SettingsKey, out Arg.Any<string>())
            .Returns(x =>
            {
                x[1] = queue;
                return true;
            });
        
        context.Extensions.Set(setting);

        action.Should().NotBeNull();
        await action!(message, context);

        context.ForwardedMessages.Should().ContainSingle();
    }
    
    [Fact]
    public async Task ExecuteAsync()
    {
        _event.ExecuteAsync((_, context) => context.Reply(Substitute.For<IOtherMessage>()));
        var action = _event.Build();

        var message = Substitute.For<ISomeMessage>();
        var context = new TestableMessageHandlerContext();
        
        action.Should().NotBeNull();
        await action!(message, context);

        context.RepliedMessages.Should().ContainSingle();
    }
    
    [Fact]
    public async Task Execute()
    {
        _event.Execute((message, _) => message.DoSomething());
        var action = _event.Build();

        var message = Substitute.For<ISomeMessage>();
        var context = new TestableMessageHandlerContext();
        
        action.Should().NotBeNull();
        await action!(message, context);

        message
            .Received(1)
            .DoSomething();
    }
    
    public interface ISomeState : IContainSagaData
    {
        
    }
    
    public interface ISomeMessage : IMessage
    {
        void DoSomething();
    }
    
    public interface IOtherMessage : IMessage
    {
        
    }
}