using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Automatonymous;
using FluentAssertions;
using GreenPipes.Payloads;
using Xunit;

namespace NServiceBus.Automatonymous.Tests
{
    public class NServiceBusStateMachineEventContextTest
    {
        private readonly Fixture _fixture;
        private readonly RelationshipStateMachine _stateMachine;

        public NServiceBusStateMachineEventContextTest()
        {
            _fixture = new Fixture();
            _stateMachine = new RelationshipStateMachine();
        }
        
        [Fact]
        public async Task Raise_Should_ThrowNotImplementedException()
        {
            var eventContext = new NServiceBusStateMachineEventContext<Relationship, Person>(_stateMachine, new Relationship(),
                _stateMachine.Introduce, _fixture.Create<Person>(), new ListPayloadCache(), CancellationToken.None);
            await Assert.ThrowsAsync<NotImplementedException>(() => eventContext.Raise(_stateMachine.Introduce));
        }
        
        [Fact]
        public async Task Raise()
        {
            var state = new Relationship();
            var @event = _fixture.Create<Person>();
            var eventContext = new NServiceBusStateMachineEventContext<Relationship, Person>(_stateMachine, state,
                _stateMachine.Introduce, @event, new ListPayloadCache(), CancellationToken.None);
            
            await ((StateMachine<Relationship>) _stateMachine).RaiseEvent(eventContext);
            var currentState = await _stateMachine.GetState(state);
            currentState.Should().Be(_stateMachine.Friend);
            state.Name.Should().Be(@event.Name);
        }

        private sealed class RelationshipStateMachine : NServiceBusStateMachine<Relationship>
        {
            public RelationshipStateMachine()
            {
                Event(() => Hello);
                Event(() => PissOff);
                Event(() => Introduce);

                State(() => Friend);
                State(() => Enemy);

                Initially(
                    When(Hello)
                        .TransitionTo(Friend),
                    When(PissOff)
                        .TransitionTo(Enemy),
                    When(Introduce)
                        .Schedule(OrderCompletionTimeout, context => new OrderCompleted(), DateTime.UtcNow)
                        .Then(ctx => ctx.Instance.Name = ctx.Data.Name)
                        .TransitionTo(Friend)
                );
            }

            public State Friend { get; private set; } = null!;
            public State Enemy { get; private set; } = null!;

            public Event Hello { get; private set; } = null!;
            public Event PissOff { get; private set; } = null!;
            public Event<Person> Introduce { get; private set; } = null!;

            public Schedule<Relationship, OrderCompleted> OrderCompletionTimeout { get; private set; } = null!; 

            public override Expression<Func<Relationship, object>> CorrelationByProperty() => x => x.Id;
        }
        
        private class Relationship : ContainSagaData
        {
            public State CurrentState { get; set; } = null!;
            public string Name { get; set; } = string.Empty;
        }
        
        private class Person : IMessage
        {
            public string Name { get; set; } = string.Empty;
        }
        
        public class OrderCompleted : IMessage
        {
            public Guid OrderId { get; set; }
        }
    }
}