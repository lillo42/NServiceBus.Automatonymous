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
        private Conventions _conventions = null!;
        
        public AutomatonymousFeature()
        {
            EnableByDefault();
            
            Defaults(settings =>
            {
                _conventions = settings.Get<Conventions>();
            });
            
            Prerequisite(context => !context.Settings.GetOrDefault<bool>("Endpoint.SendOnly"), "Sagas are only relevant for endpoints receiving messages.");
            Prerequisite(config => config.Settings.GetAvailableTypes().Any(IsNServiceBusStateMachine), "No sagas were found in the scanned types");
        }
        
        private static bool IsNServiceBusStateMachine(Type type)
            => IsCompatible(type, typeof(StateMachine));

        private static bool IsCompatible(Type type, Type source)
            => source.IsAssignableFrom(type) && type != source && !type.IsAbstract && !type.IsInterface && !type.IsGenericType;

        protected override void Setup(FeatureConfigurationContext context)
        {
            var sagaTypes = new List<Type>();
            foreach (var stateMachineType in context.Settings.GetAvailableTypes().Where(IsNServiceBusStateMachine))
            {
                var generator = new SagaGenerator(stateMachineType);
                context.Container.RegisterSingleton(stateMachineType);

                var sagaType = generator.Generate();
                sagaTypes.Add(sagaType);
            }

            foreach (var sagaType in sagaTypes)
            {
                context.Settings.GetAvailableTypes().Add(sagaType);
            }
        }
        
        private static void RegisterCustomFindersInContainer(IConfigureComponents container, IEnumerable<SagaMetadata> sagaMetaModel)
        {
            foreach (var finder in sagaMetaModel.SelectMany(m => m.Finders))
            {
                container.ConfigureComponent(finder.Type, DependencyLifecycle.InstancePerCall);

                if (finder.Properties.TryGetValue("custom-finder-clr-type", out var customFinderType))
                {
                    container.ConfigureComponent((Type)customFinderType, DependencyLifecycle.InstancePerCall);
                }
            }
        }
    }
    
}