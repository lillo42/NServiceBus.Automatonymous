using System;
using NServiceBus;

namespace Trashlantis.StateMachines;

public class TrashRemovalState : ContainSagaData
{
    public int CurrentState { get; set; }
    public string BinNumber { get; set; } = null!;
    public DateTime RequestTimestamp { get; set; }
}