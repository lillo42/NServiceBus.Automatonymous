using System;

namespace NServiceBus.Automatonymous
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StartSagaAttribute : Attribute
    {
        
    }
}