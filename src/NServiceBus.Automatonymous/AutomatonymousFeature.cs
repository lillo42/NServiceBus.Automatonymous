using System;
using System.Linq;
using Automatonymous;
using NServiceBus.Automatonymous.Schedules;
using NServiceBus.Features;
using NServiceBus.ObjectBuilder;

namespace NServiceBus.Automatonymous
{
    /// <summary>
    /// The Automatonymous <see cref="Feature"/>.
    /// </summary>
    public class AutomatonymousFeature : Feature
    {
        /// <summary>
        /// Initialize a new <see cref="AutomatonymousFeature" />.
        /// </summary>
        public AutomatonymousFeature()
        {
            EnableByDefault();
            Prerequisite(config => config.Settings.GetAvailableTypes().Any(IsNServiceBusStateMachine), "No state machine were found in the scanned types");
        }
        
        private static bool IsNServiceBusStateMachine(Type type)
            => IsCompatible(type, typeof(StateMachine));

        private static bool IsCompatible(Type type, Type source)
            => source.IsAssignableFrom(type) && type != source && !type.IsAbstract && !type.IsInterface && !type.IsGenericType;

        /// <inheritdoc />
        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Container.ConfigureComponent<DefaultMessageSchedulerContext>(DependencyLifecycle.SingleInstance);
            context.Container.ConfigureComponent<IMessageScheduler>(provider => provider.Build<DefaultMessageSchedulerContext>(), DependencyLifecycle.SingleInstance);
            context.Container.ConfigureComponent<MessageSchedulerContext>(provider => provider.Build<DefaultMessageSchedulerContext>(), DependencyLifecycle.SingleInstance);
            
            foreach (var stateMachineType in context.Settings.GetAvailableTypes().Where(IsNServiceBusStateMachine))
            {
                context.Container.RegisterSingleton(stateMachineType, Activator.CreateInstance(stateMachineType));
            }

            if (context.Settings.TryGet<string>(ErrorQueueSettings.SettingsKey, out var deadQueue))
            {
                DeadQueue = deadQueue;
            }

            Container = context.Container;
        }

        internal static string DeadQueue { get; set; } = string.Empty;
        internal static IConfigureComponents Container { get; private set; } = null!;
    }
}