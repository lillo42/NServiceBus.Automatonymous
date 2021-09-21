using System;
using Automatonymous;
using NServiceBus.Automatonymous.Events;

namespace NServiceBus.Automatonymous.Schedules
{
    public interface IScheduleConfigurator<TState, TMessage>
        where TState : class, IContainSagaData
        where TMessage : IMessage
    {
        /// <summary>
        /// Set a fixed message delay, which is applied to all scheduled messages unless
        /// overriden by the .Schedule method.
        /// </summary>
        TimeSpan Delay { set; }

        /// <summary>
        /// Set a dynamic message delay provider, which uses the instance to determine the delay
        /// unless overriden by the .Schedule method.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=Automatonymous.IScheduleConfigurator%602.DelayProvider">`IScheduleConfigurator.DelayProvider` on google.com</a></footer>
        Func<BehaviorContext<TState>, TimeSpan> DelayProvider { set; }

        /// <summary>
        /// Configure the behavior of the Received event, the same was Events are configured on
        /// the state machine.
        /// </summary>
        /// <footer><a href="https://www.google.com/search?q=Automatonymous.IScheduleConfigurator%602.Received">`IScheduleConfigurator.Received` on google.com</a></footer>
        Action<IEventCorrelationConfigurator<TState, TMessage>> Received { set; } 
    }
}