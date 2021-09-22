using NServiceBus.Features;

namespace NServiceBus.Automatonymous.Hangfire
{
    /// <summary>
    /// The <see cref="Feature"/> to add Hangfire.
    /// </summary>
    public class AutomatonymousHangfireFeature : Feature
    {
        /// <summary>
        /// 
        /// </summary>
        public AutomatonymousHangfireFeature()
        {
            DependsOn<AutomatonymousFeature>();
            EnableByDefault();
        }
        
        /// <inheritdoc />
        protected override void Setup(FeatureConfigurationContext context)
        {
            context.Container.RegisterSingleton(new HangfireMessageSchedulerContext());
            context.Container.ConfigureComponent<IMessageScheduler>(provider => provider.Build<HangfireMessageSchedulerContext>(), DependencyLifecycle.SingleInstance);
            context.Container.ConfigureComponent<MessageSchedulerContext>(provider => provider.Build<HangfireMessageSchedulerContext>(), DependencyLifecycle.SingleInstance);
        }
    }
}