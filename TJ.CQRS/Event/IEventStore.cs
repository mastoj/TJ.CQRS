using System;
using System.Collections.Generic;

namespace TJ.CQRS.Event
{
    public interface IEventStore
    {
        TAggregate Get<TAggregate>(Guid aggregateId) where TAggregate : AggregateRoot, new();
        void Insert<TAggregate>(TAggregate aggregate) where TAggregate : AggregateRoot;
    }
}