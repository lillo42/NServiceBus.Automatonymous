using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NServiceBus.Automatonymous.Events
{
    public class NServiceBusEventCorrelationConfigurator<TState, TMessage> : IEventCorrelationConfigurator<TState, TMessage>
        where TState : class, IContainSagaData, new()
    {
        public NServiceBusEventCorrelationConfigurator(
            Expression<Func<TMessage, object>>? correlateByProperty,
            Expression<Func<TState, object>> howToFindSagaWithMessage)
        {
            _howToFindSagaWithMessage = howToFindSagaWithMessage;
            _correlateByProperty = correlateByProperty;
        }

        private Expression<Func<TMessage, object>>? _correlateByProperty;
        public IEventCorrelationConfigurator<TState, TMessage> CorrelateBy(Expression<Func<TMessage, object>> propertyExpression)
        {
            _correlateByProperty = propertyExpression;
            return this;
        }

        private Expression<Func<TState, object>> _howToFindSagaWithMessage;
        public IEventCorrelationConfigurator<TState, TMessage> HowToFindSagaWithMessage(Expression<Func<TState, object>> propertyExpression)
        {
            _howToFindSagaWithMessage = propertyExpression;
            return this;
        }

        private Func<TMessage, IMessageProcessingContext, Task>? _onMissingSaga;
        public IEventCorrelationConfigurator<TState, TMessage> OnMissingSaga(Action<IMissingInstanceConfigurator<TState, TMessage>> config)
        {
            var configurator = new EventMissingInstanceConfigurator<TState, TMessage>();
            config(configurator);
            _onMissingSaga = configurator.Build();
            return this;
        }
        
        public NServiceBusEventCorrelation<TState, TMessage> Build()
            => new NServiceBusEventCorrelation<TState, TMessage>(_correlateByProperty,
                _howToFindSagaWithMessage,
                _onMissingSaga);
    }
}