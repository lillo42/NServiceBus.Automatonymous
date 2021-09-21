using System;
using System.Threading.Tasks;
using Automatonymous.Binders;
using NServiceBus;
using NServiceBus.Automatonymous;
using NServiceBus.Automatonymous.Activities;

namespace Automatonymous
{
    
    public static partial class AutomatonymousExtensions
    {
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, Func<BehaviorContext<TInstance>, DateTime> timeProvider)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(_ => message, schedule, timeProvider));
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="schedulerTime">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, DateTime schedulerTime)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(_ => message, schedule, _ => schedulerTime));
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="delay">The delay time.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, TimeSpan delay)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(_ => message, schedule, _ => DateTime.UtcNow.Add(delay)));

        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, Func<BehaviorContext<TInstance>, DateTime> timeProvider)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(_ => message, schedule, timeProvider));
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="schedulerTime">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, DateTime schedulerTime)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(_ => message, schedule, _ => schedulerTime));
        
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="delay">The delay time.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, TimeSpan delay)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(_ => message, schedule, _ => DateTime.UtcNow.Add(delay)));

        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance>, TMessage> messageFactory, 
            Func<BehaviorContext<TInstance>, DateTime> timeProvider)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(messageFactory, schedule, timeProvider));

        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="schedulerTime">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance>, TMessage> messageFactory, DateTime schedulerTime)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(messageFactory, schedule, _ => schedulerTime));
        
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="delay">The delay time.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance>, TMessage> messageFactory, TimeSpan delay)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(messageFactory, schedule, _ => DateTime.UtcNow.Add(delay)));
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance>, Task<TMessage>> messageFactory, 
            Func<BehaviorContext<TInstance>, DateTime> timeProvider)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(messageFactory, schedule, timeProvider));
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="schedulerTime">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance>, Task<TMessage>> messageFactory, DateTime schedulerTime)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(messageFactory, schedule, _ => schedulerTime));
        
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="delay">The delay time.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Schedule<TInstance, TMessage>(this EventActivityBinder<TInstance> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance>, Task<TMessage>> messageFactory, TimeSpan delay)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance,TMessage>(messageFactory, schedule, _ => DateTime.UtcNow.Add(delay)));
        
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, Func<BehaviorContext<TInstance, TData>, DateTime> timeProvider)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(_ => message, schedule, timeProvider));
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="schedulerTime">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, DateTime schedulerTime)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(_ => message, schedule, _ => schedulerTime));
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="delay">The delay time.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, TMessage message, TimeSpan delay)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(_ => message, schedule, _ => DateTime.UtcNow.Add(delay)));

        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, Func<BehaviorContext<TInstance, TData>, DateTime> timeProvider)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(_ => message, schedule, timeProvider));
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="schedulerTime">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, DateTime schedulerTime)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage=>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(_ => message, schedule, _ => schedulerTime));
        
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="message">The message.</param>
        /// <param name="delay">The delay time.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Task<TMessage> message, TimeSpan delay)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(_ => message, schedule, _ => DateTime.UtcNow.Add(delay)));

        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory, 
            Func<BehaviorContext<TInstance, TData>, DateTime> timeProvider)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(messageFactory, schedule, timeProvider));

        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="schedulerTime">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory, DateTime schedulerTime)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(messageFactory, schedule, _ => schedulerTime));
        
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="delay">The delay time.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance, TData>, TMessage> messageFactory, TimeSpan delay)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(messageFactory, schedule, _ => DateTime.UtcNow.Add(delay)));
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="timeProvider">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance, TData>, Task<TMessage>> messageFactory, 
            Func<BehaviorContext<TInstance, TData>, DateTime> timeProvider)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(messageFactory, schedule, timeProvider));
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="schedulerTime">The time provider.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance, TData>, Task<TMessage>> messageFactory, DateTime schedulerTime)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(messageFactory, schedule, _ => schedulerTime));
        
        
        /// <summary>
        /// Scheduler a message.
        /// </summary>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The <see cref="NServiceBus.Automatonymous.Schedule{TInstance,TMessage}"/> message.</param>
        /// <param name="messageFactory">The message factory.</param>
        /// <param name="delay">The delay time.</param>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <typeparam name="TMessage">The message type.</typeparam>
        /// <typeparam name="TData">The event data.</typeparam>
        /// <returns>The <see cref="EventActivityBinder{TInstance,TData}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Schedule<TInstance, TData, TMessage>(this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance, TMessage> schedule, Func<BehaviorContext<TInstance, TData>, Task<TMessage>> messageFactory, TimeSpan delay)
            where TInstance : class, IContainSagaData
            where TMessage : class, IMessage =>
            source.Add(new ScheduleActivity<TInstance, TData,TMessage>(messageFactory, schedule, _ => DateTime.UtcNow.Add(delay)));

        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The scheduler config.</param>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance> Unschedule<TInstance>(
            this EventActivityBinder<TInstance> source,
            Schedule<TInstance> schedule)
            where TInstance : class, IContainSagaData =>
            source.Add(new UnscheduleActivity<TInstance>(schedule));
        
        /// <summary>
        /// Unschedule a message, if the message was scheduled.
        /// </summary>
        /// <typeparam name="TInstance">The state machine data.</typeparam>
        /// <param name="source">The <see cref="EventActivityBinder{TInstance}"/>.</param>
        /// <param name="schedule">The scheduler config.</param>
        /// <returns>The <see cref="EventActivityBinder{TInstance}"/>.</returns>
        public static EventActivityBinder<TInstance, TData> Unschedule<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source,
            Schedule<TInstance> schedule)
            where TInstance : class, IContainSagaData =>
            source.Add(new UnscheduleActivity<TInstance>(schedule));

    }
}