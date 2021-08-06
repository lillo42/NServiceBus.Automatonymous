using System;

namespace NServiceBus.Automatonymous
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StartSagaAttribute : Attribute
    {
        
    }
}