using System;

namespace NServiceBus.Automatonymous
{
    /// <summary>
    /// Indicate that event start the State Machine
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class StartStateMachineAttribute : Attribute
    {
        
    }
}