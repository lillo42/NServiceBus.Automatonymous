using System;
using FluentAssertions;
using GreenPipes;
using NSubstitute;
using Xunit;

namespace NServiceBus.Automatonymous.Tests.Extensions;

public class PipeContextExtensionsTest
{
    private readonly IMessageCreator _messageCreator;
    private readonly PipeContext _context;

    public PipeContextExtensionsTest()
    {
        _context = Substitute.For<PipeContext>();
        _messageCreator = Substitute.For<IMessageCreator>();
        _context.TryGetPayload(out  Arg.Any<IMessageCreator>())
            .Returns(x =>
            {
                x[0] = _messageCreator;
                return true;
            });
    }
    
    [Fact]
    public void Init()
    {
        var simple = Substitute.For<ISimpleMessage>();
        _messageCreator.CreateInstance<ISimpleMessage>().Returns(simple);
        var message = _context.Init<ISimpleMessage>();
        message.Should().NotBeNull();
        message.Should().Be(simple);
    }
    
    [Fact]
    public void InitWithAction()
    {
        var simple = Substitute.For<ISimpleMessage>();
        _messageCreator.CreateInstance(Arg.Any<Action<ISimpleMessage>>()).Returns(simple);
        var message = _context.Init<ISimpleMessage>(m => { });
        message.Should().NotBeNull();
        message.Should().Be(simple);
    }
    
    public interface ISimpleMessage :IMessage
    {
        
    }
}