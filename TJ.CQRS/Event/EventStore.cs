using System;
using System.Collections.Generic;
using System.Linq;
using TJ.CQRS.Messaging;

namespace TJ.CQRS.Event
{
    public abstract class EventStore : IEventStore, IUnitOfWork
    {
        private readonly IBus _bus;
        private Dictionary<Guid, AggregateRoot> _aggregateDictionary;

        protected abstract void InsertBatch(IEnumerable<IDomainEvent> eventBatch);
        protected abstract IEnumerable<IDomainEvent> GetEvents(Guid aggregateId);

        public EventStore(IBus bus)
        {
            _bus = bus;
            _bus.Commit += Commit;
            _aggregateDictionary = new Dictionary<Guid, AggregateRoot>();
        }

        public T Get<T>(Guid aggregateId) where T : AggregateRoot, new()
        {
            if (_aggregateDictionary.ContainsKey(aggregateId))
            {
                return _aggregateDictionary[aggregateId] as T;
            }
            var events = GetEvents(aggregateId).ToList();
            if (events.Count == 0)
            {
                return null;
            }
            T aggregate = new T();
            aggregate.LoadAggregate(events);
            _aggregateDictionary.Add(aggregateId, aggregate);
            return aggregate;
        }

        public void Insert<TAggregate>(TAggregate aggregate) where TAggregate : AggregateRoot
        {
            _aggregateDictionary.Add(aggregate.AggregateId, aggregate);
        }

        public void Rollback()
        {
            ClearEvents();
        }

        public void Commit()
        {
            var uncommitedEvents = GetUncommitedEvents();
            InsertBatch(uncommitedEvents);
            _bus.PublishEvents(uncommitedEvents);
            ClearEvents();
        }

        private void ClearEvents()
        {
            foreach (var aggregateRoot in _aggregateDictionary)
            {
                aggregateRoot.Value.ClearChanges();
            }
            _aggregateDictionary = new Dictionary<Guid, AggregateRoot>();
        }

        private List<IDomainEvent> GetUncommitedEvents()
        {
            List<IDomainEvent> uncommitedEvents = new List<IDomainEvent>();
            foreach (var aggregateRoot in _aggregateDictionary)
            {
                uncommitedEvents.AddRange(aggregateRoot.Value.GetChanges());
            }
            return uncommitedEvents;
        }
    }
}