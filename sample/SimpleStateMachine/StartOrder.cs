using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SimpleStateMachine;

public class StartOrder : IMessage
{
    public Guid OrderId { get; set; }
}