using System;
using System.Linq.Expressions;
using Automatonymous;
using GreenPipes;
using NServiceBus;
using NServiceBus.Automatonymous;
using Trashlantis.Contracts;

namespace Trashlantis.StateMachines
{
    public class TrashRemovalStateMachine : NServiceBusStateMachine<TrashRemovalState>
    {

        public TrashRemovalStateMachine()
        {
            InstanceState(x => x.CurrentState, Requested);

            Event(() => TrashRemovalRequested, x =>
            {
                x.CorrelateBy(y => y.BinNumber);
            });
            
            Initially(When(TrashRemovalRequested)
                .Then(x =>
                {
                    x.Instance.BinNumber = x.Data.BinNumber;
                    x.Instance.RequestTimestamp = DateTime.UtcNow;
                })
                .PublishAsync(x => new EmptyTrashBin { BinNumber = x.Instance.BinNumber })
                .TransitionTo(Requested)
            );
            
            During(Requested, When(TrashRemovalRequested)
                .PublishAsync(x => new EmptyTrashBin { BinNumber = x.Instance.BinNumber }));
        }

        public override Expression<Func<TrashRemovalState, object>> CorrelationByProperty() => x => x.BinNumber;

        public State Requested { get; private set; } = null!;
        
        [StartStateMachine]
        public Event<TakeOutTheTrash> TrashRemovalRequested { get; private set; } = null!;
    }
}