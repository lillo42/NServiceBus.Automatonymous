using System;

namespace NServiceBus.Automatonymous
{
    internal class SagaType
    {
        public SagaType(Type saga)
        {
            Saga = saga;
        }

        public Type Saga { get; }
    }
}