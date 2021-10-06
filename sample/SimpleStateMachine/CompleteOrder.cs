using System;
using NServiceBus;

namespace SimpleStateMachine;

public class CompleteOrder : IMessage
{
    public Guid OrderId { get; set; }
}