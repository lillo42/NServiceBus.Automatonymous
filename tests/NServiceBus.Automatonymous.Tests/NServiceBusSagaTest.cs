using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Automatonymous;
using FluentAssertions;
using NServiceBus.ObjectBuilder;
using NServiceBus.Testing;
using NSubstitute;
using Xunit;

namespace NServiceBus.Automatonymous.Tests;

public class SimpleNServiceBusSagaTest
{
    private readonly FakeBuilder _builder;
    private readonly SimpleSaga _saga;

    public SimpleNServiceBusSagaTest()
    {
        _builder = new FakeBuilder();
        _builder.Register(new MessageHandlerContextWrapper());
        _saga = new(new(), _builder)
        {
            Data = new()
            {
                Id = Guid.NewGuid()
            }
        };
    }

    [Fact]
    public async Task Execute_StartBy()
    {
        AutomatonymousFeature.Container = Substitute.For<IConfigureComponents>();
        var context = new TestableMessageHandlerContext
        {
            Builder = _builder
        };
        await _saga.Handle(Substitute.For<IStartBy>(), context);
        var state = await _saga.StateMachine.GetState(_saga.Data);
        state.Should().Be(_saga.StateMachine.Started);
    }

    [Fact]
    public async Task Execute_FinalizeBy()
    {
        AutomatonymousFeature.Container = Substitute.For<IConfigureComponents>();
        var context = new TestableMessageHandlerContext
        {
            Builder = _builder
        };
        await _saga.Handle(Substitute.For<IFinalizeBy>(), context);
        var state = await _saga.StateMachine.GetState(_saga.Data);
        state.Should().Be(_saga.StateMachine.Final);
    }
    
    [Fact]
    public async Task Execute_NotFound_Interface_WithoutAction()
    {
        AutomatonymousFeature.Container = Substitute.For<IConfigureComponents>();
        var context = new TestableMessageHandlerContext
        {
            Builder = _builder
        };
        await _saga.Handle((object)new MissingWithoutAction(), context);
    }
    
    [Fact]
    public async Task Execute_NotFound_Class_WithoutAction()
    {
        AutomatonymousFeature.Container = Substitute.For<IConfigureComponents>();
        var context = new TestableMessageHandlerContext
        {
            Builder = _builder
        };
        await _saga.Handle((object)Substitute.For<IStartBy>(), context);
    }
    
    [Fact]
    public async Task Execute_NotFound_Interface_WithAction()
    {
        AutomatonymousFeature.Container = Substitute.For<IConfigureComponents>();
        var context = new TestableMessageHandlerContext
        {
            Builder = _builder
        };
        await _saga.Handle((object)Substitute.For<IFinalizeBy>(), context);
        context.SentMessages.Should().ContainSingle();
    }
    
    [Fact]
    public async Task Execute_NotFound_Class_WithAction()
    {
        AutomatonymousFeature.Container = Substitute.For<IConfigureComponents>();
        var context = new TestableMessageHandlerContext
        {
            Builder = _builder
        };
        await _saga.Handle((object)new MissingWithAction(), context);
        context.SentMessages.Should().ContainSingle();
    }
    
    [Fact]
    public async Task Execute_NotFound_WithoutCorrelation()
    {
        AutomatonymousFeature.Container = Substitute.For<IConfigureComponents>();
        var context = new TestableMessageHandlerContext
        {
            Builder = _builder
        };
        await _saga.Handle(new object(), context);
    }
    
    public class SimpleSaga : NServiceBusSaga<SimpleStateMachine, SimpleSagaData>,
        IAmStartedByMessages<IStartBy>,
        IAmStartedByMessages<IFinalizeBy>
    {
        public SimpleSaga(SimpleStateMachine stateMachine, IBuilder builder) 
            : base(stateMachine, builder)
        {
        }

        public Task Handle(IStartBy message, IMessageHandlerContext context) 
            => Execute(message, context, StateMachine.StartBy);

        public Task Handle(IFinalizeBy message, IMessageHandlerContext context) 
            => Execute(message, context, StateMachine.FinalizeBy);
    }

    public class SimpleSagaData : ContainSagaData
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = null!;
    }

    public class SimpleStateMachine : NServiceBusStateMachine<SimpleSagaData>
    {

        public SimpleStateMachine()
        {
            InstanceState(x => x.CurrentState);
            Initially(When(StartBy).TransitionTo(Started),
                When(FinalizeBy).TransitionTo(Final));

            Event(() => FinalizeBy, config => config
                .OnMissingSaga(missing => missing.Execute((_, context) => context.SendLocal<IMissing>(message => { }))));
            
            Event(() => MissingWithAction, config => config
                .OnMissingSaga(missing => missing.Execute((_, context) => context.SendLocal<IMissing>(message => { }))));
        }

        public State Started { get; private set; } = null!;
        
        public Event<IStartBy> StartBy { get; private set; } = null!;
        public Event<IFinalizeBy> FinalizeBy { get; private set; } = null!;
        
        public Event<MissingWithAction> MissingWithAction { get; private set; } = null!;
        
        public Event<MissingWithoutAction> MissingWithoutAction { get; private set; } = null!;
        
        public override Expression<Func<SimpleSagaData, object>> CorrelationByProperty() => x => x.CorrelationId;
    }

    public interface IStartBy : IMessage { }

    public interface IFinalizeBy : IMessage { }

    public interface IMissing : IMessage { }

    public class MissingWithAction : IMessage { }

    public class MissingWithoutAction : IMessage { }
}
