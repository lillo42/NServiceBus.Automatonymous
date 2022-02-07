using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SimpleStateMachine;

public class StartOrder : IMessage
{
    public Guid OrderId { get; set; }
}

public class TestHandler : IHandleMessages<StartOrder>
{
    public Task Handle(StartOrder message, IMessageHandlerContext context)
    {
        // context.SendLocal();
        // context.DoNotContinueDispatchingCurrentMessageToHandlers();
        // context.ForwardCurrentMessageTo()
        throw new NotImplementedException();
    }
}