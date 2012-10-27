using System;
using System.Collections.Generic;
using System.Linq;
using TJ.CQRS.Event;
using TJ.CQRS.Messaging;

namespace TJ.CQRS.Tests.Stub
{
    public class StubEventStore : EventStore
    {
        private List<IDomainEvent> _insertedEvents;
        private Dictionary<Guid, IEnumerable<IDomainEvent>> _aggregateEventDictionary;

        public StubEventStore(IEventBus eventBus) : base(eventBus)
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

        public override IEnumerable<IDomainEvent> GetAllEvents()
        {
            return _insertedEvents;
        }

        public void InsertEvents(IEnumerable<IDomainEvent> eventBatch)
        {
            _insertedEvents.AddRange(eventBatch);
        }
    }
}