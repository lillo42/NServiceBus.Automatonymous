using System;
using System.Linq;
using Automatonymous;
using NServiceBus.Features;
using NServiceBus.ObjectBuilder;

namespace NServiceBus.Automatonymous;

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
        context.Container.ConfigureComponent<MessageHandlerContextWrapper>(DependencyLifecycle.InstancePerUnitOfWork);

        foreach (var stateMachineType in context.Settings.GetAvailableTypes().Where(IsNServiceBusStateMachine))
        {
            context.Container.RegisterSingleton(stateMachineType, Activator.CreateInstance(stateMachineType));
        }
        
        Container = context.Container;
    }

    internal static IConfigureComponents Container { get; set; } = null!;
}