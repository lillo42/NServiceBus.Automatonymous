using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automatonymous;
using GreenPipes.Payloads;
using NServiceBus.Automatonymous.Events;
using NServiceBus.Logging;
using NServiceBus.ObjectBuilder;
using NServiceBus.Sagas;

namespace NServiceBus.Automatonymous;

/// <summary>
/// The base <see cref="Saga{TSagaData}"/>. 
/// </summary>
/// <typeparam name="TStateMachine">The <see cref="NServiceBusStateMachine{TState}"/>.</typeparam>
/// <typeparam name="TState">The <see cref="IContainSagaData"/>.</typeparam>
public abstract class NServiceBusSaga<TStateMachine, TState> : Saga<TState>, IHandleSagaNotFound
    where TStateMachine : NServiceBusStateMachine<TState>, new()
    where TState : class, IContainSagaData, new()
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly ILog Log = LogManager.GetLogger(typeof(TStateMachine));
        
    /// <summary>
    /// The <typeparamref name="TStateMachine" />.
    /// </summary>
    protected internal TStateMachine StateMachine { get; }
    private readonly IBuilder _builder;
        
    /// <summary>
    /// Initialize <see cref="NServiceBusSaga{TStateMachine,TState}" />.
    /// </summary>
    /// <param name="stateMachine">The <see cref="NServiceBusStateMachine{TState}" />.</param>
    /// <param name="builder">The <see cref="IBuilder" />.</param>
    protected NServiceBusSaga(TStateMachine stateMachine, IBuilder builder)
    {
        StateMachine = stateMachine;
        _builder = builder;
    }

    /// <inheritdoc />
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TState> mapper)
    {
        var stateMachine = new TStateMachine();
        foreach (var correlation in stateMachine.Correlations)
        {
            correlation.Map(mapper);
        }
    }
        
    /// <summary>
    /// Execute message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="context">The <see cref="IMessageHandlerContext" />.</param>
    /// <param name="event">The <see cref="Event{TData}"/>.</param>
    /// <typeparam name="T">The message type.</typeparam>
    protected async Task Execute<T>(T message, IMessageHandlerContext context, Event<T> @event)
    {
        var eventContext = new NServiceBusStateMachineEventContext<TState, T>(StateMachine, Data, @event, message, 
            new BuilderPayloadCache(_builder,  AutomatonymousFeature.Container, new ListPayloadCache()),
            CancellationToken.None);
        _builder.Build<MessageHandlerContextWrapper>().MessageHandlerContext = context;
        eventContext.GetOrAddPayload(() => context);
        eventContext.GetOrAddPayload(() => Log);
        eventContext.GetOrAddPayload(() => new SagaType(GetType()));
        await ((StateMachine<TState>) StateMachine).RaiseEvent(eventContext);

        var state = await StateMachine.GetState(Data);
        // ReSharper disable once PossibleUnintendedReferenceComparison
        if (state == StateMachine.Final)
        {
            MarkAsComplete();
        }
    }

    /// <inheritdoc />
    public Task Handle(object message, IMessageProcessingContext context)
    {
        var messageType = message.GetType();
        if (StateMachine.TryGetCorrelations(messageType, out var correlation))
        {
            return correlation.OnMissingSaga?.Invoke(message, context) ?? Task.CompletedTask;
        }

        if (messageType.GetInterfaces().Any(@interface => StateMachine.TryGetCorrelations(@interface, out correlation)))
        {
            return correlation!.OnMissingSaga?.Invoke(message, context) ?? Task.CompletedTask;
        }
        
        return Task.CompletedTask;
    }
}