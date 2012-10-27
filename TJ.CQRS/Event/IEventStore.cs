using System;
using System.Collections.Generic;
using TJ.CQRS.Messaging;

namespace TJ.CQRS.Event
{
    public interface IEventStore
    {
        void Rollback();
        void Commit();
        TAggregate Get<TAggregate>(Guid aggregateId) where TAggregate : AggregateRoot, new();
        void Insert<TAggregate>(TAggregate aggregate) where TAggregate : AggregateRoot;
        IEnumerable<IDomainEvent> GetAllEvents();
    }
}