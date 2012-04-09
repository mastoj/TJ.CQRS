using System;

namespace TJ.CQRS.Infrastructure.Messaging
{
    public class Command : ICommand
    {
        public Guid AggregateId { get; private set; }

        public Command(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}