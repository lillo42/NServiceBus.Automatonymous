using System;
using NServiceBus;

namespace SimpleStateMachine
{
    public class CancelOrder : IMessage
    {
        public Guid OrderId { get; set; }
    }
}