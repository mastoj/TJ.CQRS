using System;

namespace TJ.CQRS.Messaging
{
    public interface IDomainEvent : IMessage
    {
        Guid Id { get; set; }
        DateTime TimeStamp { get; }
        Guid AggregateId { get; set; }
        int EventNumber { get; set; }
    }
}