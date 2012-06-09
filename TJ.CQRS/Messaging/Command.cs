using System;

namespace TJ.CQRS.Messaging
{
    public class Command : ICommand
    {
        public Guid AggregateId { get; set; }

        public Command(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}