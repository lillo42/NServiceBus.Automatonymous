using System;
using System.Linq.Expressions;
using Automatonymous;
using GreenPipes.Internals.Extensions;
using GreenPipes.Internals.Reflection;

namespace NServiceBus.Automatonymous.Schedules
{
    public class StateMachineSchedule<TInstance, TMessage> : Schedule<TInstance, TMessage>
        where TInstance : class, IContainSagaData
        where TMessage : class, IMessage
    {
        private readonly IScheduleSettings<TInstance, TMessage> _settings;
        private readonly ReadWriteProperty<TInstance, Guid?> _tokenIdProperty;
        
        public StateMachineSchedule(string name, Expression<Func<TInstance, Guid?>> tokenIdExpression, IScheduleSettings<TInstance, TMessage> settings)
        {
            Name = name;
            _settings = settings;
            _tokenIdProperty = new ReadWriteProperty<TInstance, Guid?>(tokenIdExpression.GetPropertyInfo());
        }

        /// <inheritdoc />
        public string Name { get; }
        
        /// <inheritdoc />
        public Event<TMessage> Received { get; set; }

        /// <inheritdoc />
        public Event<TMessage> AnyReceived { get; set; }

        /// <inheritdoc />
        public TimeSpan GetDelay(BehaviorContext<TInstance> context) => _settings.DelayProvider(context);

        /// <inheritdoc />
        public Guid? GetTokenId(TInstance data) => _tokenIdProperty.Get(data);

        /// <inheritdoc />
        public void SetTokenId(TInstance data, Guid? tokenId) => _tokenIdProperty.SetProperty(data, tokenId);
    }
}