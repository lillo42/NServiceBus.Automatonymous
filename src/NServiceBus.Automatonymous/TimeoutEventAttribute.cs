using System;

namespace NServiceBus.Automatonymous
{
    /// <summary>
    /// Mark event as timeout event
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TimeoutEventAttribute : Attribute
    {
        
    }
}