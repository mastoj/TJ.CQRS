using System;

namespace TJ.CQRS.Infrastructure.Messaging
{
    public interface IDomainEvent : IMessage
    {
        Guid Id { get; }
        DateTime TimeStamp { get; }
        Guid AggregateId { get; set; }
        int EventNumber { get; set; }
    }
}