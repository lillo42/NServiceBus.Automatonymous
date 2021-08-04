using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Automatonymous;
using Automatonymous.Contexts;
using NServiceBus.Logging;
using NServiceBus.Sagas;

namespace NServiceBus.Automatonymous
{
    public class NServiceBusSaga<TStateMachine, TState> : Saga<TState>, IHandleSagaNotFound
        where TStateMachine : NServiceBusStateMachine<TState>, new()
        where TState : class, IContainSagaData, new()
    {
        // ReSharper disable once StaticMemberInGenericType
        private static ILog? _log;
        protected TStateMachine StateMachine { get; private set; }

        protected NServiceBusSaga(TStateMachine stateMachine)
        {
            StateMachine = stateMachine;
            InitiallyLog();
        }

        private void InitiallyLog()
        {
            _log ??= LogManager.GetLogger(GetType());
        }

        private static readonly FieldInfo ConfigureHowToFindSagaWithMessage = typeof(SagaPropertyMapper<TState>)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default)
            .First(x => x.FieldType == typeof(IConfigureHowToFindSagaWithMessage));

        private static readonly MethodInfo ConfigureMapping = typeof(IConfigureHowToFindSagaWithMessage)
            .GetMethod(nameof(IConfigureHowToFindSagaWithMessage.ConfigureMapping))!;
        
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TState> mapper)
        {
            var configureHowToFindSagaWithMessage = ConfigureHowToFindSagaWithMessage.GetValue(mapper);
            foreach (var correlation in new TStateMachine().Correlations.Where(x => x.CorrelateByProperty != null))
            {
                var genericMethod = ConfigureMapping.MakeGenericMethod(typeof(TState), correlation.MessageType);
                genericMethod.Invoke(configureHowToFindSagaWithMessage, new[] { correlation.HowToFindSagaWithMessage, correlation.CorrelateByProperty });
            }
        }
        
        protected async Task Execute<T>(T message, IMessageHandlerContext context, Event<T> @event)
        {
            var eventContext = new StateMachineEventContext<TState, T>(StateMachine, Data, @event, message, CancellationToken.None);
            eventContext.GetOrAddPayload(() => context);
            eventContext.GetOrAddPayload(() => _log);
            eventContext.GetOrAddPayload(GetType);
            await ((StateMachine<TState>) StateMachine).RaiseEvent(eventContext);

            var state = await StateMachine.GetState(Data);
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (state == StateMachine.Final)
            {
                MarkAsComplete();
            }
        }

        public Task Handle(object message, IMessageProcessingContext context)
        {
            var correlations = StateMachine.GetCorrelations(message.GetType());
            return correlations.OnMissingSaga != null ? correlations.OnMissingSaga(message, context) : Task.CompletedTask;
        }
    }
}