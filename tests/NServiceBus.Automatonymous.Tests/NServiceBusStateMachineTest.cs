using System;
using System.Linq.Expressions;
using Automatonymous;
using FluentAssertions;
using Xunit;

namespace NServiceBus.Automatonymous.Tests;

public class NServiceBusStateMachineTest
{
    [Fact]
    public void Create()
    {
        _ = new SimpleStateMachine();
    }
    
    [Fact]
    public void Correlations()
    {
        var machine = new SimpleStateMachine();
        foreach (var correlation in machine.Correlations)
        {
            correlation.Should().NotBeNull();
        }
    }
    
    public class SimpleSagaData : ContainSagaData
    {
        public Guid OrderId { get; set; }
    }
    
    public sealed class SimpleStateMachine : NServiceBusStateMachine<SimpleSagaData>
    {
#nullable disable
        public SimpleStateMachine()
        {
            Event(() => Explicit);
            Event(() => CorrelatedBy!);
            
            Event(() => CorrelatedByOrderId, opt => opt
                .CorrelateBy(x => x.OrderId)
                .HowToFindSagaData(x => x.OrderId)
                .OnMissingSaga(x => x.Fault()));
            
            Event(() => CorrelatedByOrderId, opt => opt
                .CorrelateByHeader("header")
                .OnMissingSaga(x => x.Discard()));
        }
        

        public State SampleState { get; private set; } = null;
        public State<string> SampleGenericState { get; private set; } = null;
        public Event<IRegisterEventExplicit> Explicit { get; private set; } = null;
        public Event<IRegisterEventImplicit> Implict { get; private set; } = null;
        public Event<IMessageWithCorrelatedBy> CorrelatedBy { get; private set; } = null;
        public Event<IMessageWithGenericCorrelatedBy> GenericCorrelatedBy { get; private set; } = null;
        public Event<ICorrelatedByOrderId> CorrelatedByOrderId { get; private set; } = null;
        public Event<ICorrelatedByHeader> CorrelatedByHeader { get; private set; } = null;
#nullable restore
        public override Expression<Func<SimpleSagaData, object>> CorrelationByProperty()
            => x => x.Id;

        protected override string DefaultCorrelationMessageByPropertyName => "Id";
    }
    
    public interface IRegisterEventExplicit : IMessage { }

    public interface IRegisterEventImplicit : IMessage
    {
        Guid Id { get; }
    }
    
    public interface ICorrelatedByOrderId : IMessage
    {
        Guid OrderId { get; }
    }
    
    public interface ICorrelatedByHeader : IMessage
    { 
    }

    
    public interface IMessageWithCorrelatedBy : ICorrelatedBy { }
    
    public interface IMessageWithGenericCorrelatedBy : ICorrelatedBy<Guid> { }
}