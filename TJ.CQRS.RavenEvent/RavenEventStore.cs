using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Json.Linq;
using TJ.CQRS.Event;
using TJ.CQRS.Messaging;

namespace TJ.CQRS.RavenEvent
{
    public class Events_ByAggregateId : AbstractIndexCreationTask<IDomainEvent>
    {
        public Events_ByAggregateId()
        {
            Map = domainEvents => from domainEvent in domainEvents select new { AggregateId = domainEvent.AggregateId };
        }
    }

    public class RavenEventStore : EventStore
    {
        private DocumentStore _documentStore;

        public RavenEventStore(IEventBus eventBus, string connectionStringName)
            : base(eventBus)
        {
            var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionStringName(connectionStringName);
            parser.Parse();
            _documentStore = new DocumentStore
                                 {
                                     Url = parser.ConnectionStringOptions.Url,
                                     ApiKey = parser.ConnectionStringOptions.ApiKey,
                                     Conventions =
                                         {
                                             FindTypeTagName = type =>
                                                       {
                                                           if (typeof(IDomainEvent).IsAssignableFrom(type))
                                                           {
                                                               return "Events";
                                                           }
                                                           return DocumentConvention.DefaultTypeTagName(type);
                                                       }
                                         }
                                 };
            _documentStore.Initialize();
            IndexCreation.CreateIndexes(typeof(Events_ByAggregateId).Assembly, _documentStore);
        }

        internal void InsertBatchTest(IEnumerable<IDomainEvent> eventBatch)
        {
            InsertBatch(eventBatch);
        }

        protected override void InsertBatch(IEnumerable<IDomainEvent> eventBatch)
        {
            using (var transaction = new TransactionScope())
            {
                using (var session = _documentStore.OpenSession())
                {
                    foreach (var domainEvent in eventBatch)
                    {
                        var item = domainEvent;
                        session.Store(item);
                    }
                    session.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        internal IEnumerable<IDomainEvent> GetEventsTest(Guid aggregateId)
        {
            return GetEvents(aggregateId);
        }

        protected override IEnumerable<IDomainEvent> GetEvents(Guid aggregateId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var events = session.Query<IDomainEvent>().Where(y => y.AggregateId == aggregateId);
                return events;
            }
        }

        public override IEnumerable<IDomainEvent> GetAllEvents()
        {
            using (var session = _documentStore.OpenSession())
            {
                var events = session.Query<IDomainEvent>().ToArray();
                return events;
            }
        }

        internal void DeleteCollection()
        {
            using (var session = _documentStore.OpenSession())
            {
                _documentStore.DatabaseCommands.DeleteByIndex("Events/ByAggregateId", new IndexQuery());
                session.SaveChanges();
            }
        }
    }
}