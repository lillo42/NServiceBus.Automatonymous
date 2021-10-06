using System;
using NServiceBus;

namespace SimpleStateMachine;

public class OrderState : ContainSagaData
{
    public string CurrentState { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
}