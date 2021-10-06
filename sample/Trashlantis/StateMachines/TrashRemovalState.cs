using System;
using NServiceBus;

namespace Trashlantis.StateMachines;

public class TrashRemovalState : ContainSagaData
{
    public int CurrentState { get; set; }
    public string BinNumber { get; set; } = string.Empty;
    public DateTime RequestTimestamp { get; set; }
}