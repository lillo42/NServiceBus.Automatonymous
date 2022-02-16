using System;
using NServiceBus;

namespace SimpleStateMachine;

public class OrderState : ContainSagaData
{
    public string CurrentState { get; set; } = null!;
    public Guid OrderId { get; set; }
    public Guid? CancelOrderId { get; set; }
}