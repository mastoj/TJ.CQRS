using System;
using System.Collections.Generic;
using System.Linq;
using TJ.CQRS.Infrastructure.Event;
using TJ.CQRS.Infrastructure.Messaging;

namespace TJ.CQRS.Infrastructure.Tests.Stub
{
    public class StubEventStore : EventStore
    {
        private List<IDomainEvent> _insertedEvents;
        private Dictionary<Guid, IEnumerable<IDomainEvent>> _aggregateEventDictionary;

        public StubEventStore(IBus bus) : base(bus)
        {
            _insertedEvents = new List<IDomainEvent>();
        }

        public IEnumerable<IDomainEvent> InsertedEvents
        {
            get { return _insertedEvents; }
        }

        protected override void InsertBatch(IEnumerable<IDomainEvent> eventBatch)
        {
            _insertedEvents.AddRange(eventBatch);
        }

        protected override IEnumerable<IDomainEvent> GetEvents(Guid aggregateId)
        {
            return _insertedEvents.Where(y => y.AggregateId == aggregateId);
        }

        public void InsertEvents(IEnumerable<IDomainEvent> eventBatch)
        {
            _insertedEvents.AddRange(eventBatch);
        }
    }
}