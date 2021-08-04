using System;
using System.Collections.Generic;
using System.Linq;
using Automatonymous;
using NServiceBus.Automatonymous.Generators;
using NServiceBus.Features;
using NServiceBus.ObjectBuilder;
using NServiceBus.Sagas;
using NServiceBus.Transport;

namespace NServiceBus.Automatonymous
{
    public class AutomatonymousFeature : Feature
    {
        public AutomatonymousFeature()
        {
            EnableByDefault();
            Prerequisite(config => config.Settings.GetAvailableTypes().Any(IsNServiceBusStateMachine), "No state machine were found in the scanned types");
        }
        
        private static bool IsNServiceBusStateMachine(Type type)
            => IsCompatible(type, typeof(StateMachine));

        private static bool IsCompatible(Type type, Type source)
            => source.IsAssignableFrom(type) && type != source && !type.IsAbstract && !type.IsInterface && !type.IsGenericType;

        protected override void Setup(FeatureConfigurationContext context)
        {
            foreach (var stateMachineType in context.Settings.GetAvailableTypes().Where(IsNServiceBusStateMachine))
            {
                context.Container.RegisterSingleton(stateMachineType);
            }
        }
    }
}