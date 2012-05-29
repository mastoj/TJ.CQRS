using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client.Document;
using Raven.Client.Indexes;
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

        public RavenEventStore(IEventBus eventBus, string connectionStringName) : base(eventBus)
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
                                                           if(typeof(IDomainEvent).IsAssignableFrom(type))
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

        protected override void InsertBatch(IEnumerable<IDomainEvent> eventBatch)
        {
            using(var session = _documentStore.OpenSession())
            {
                using (var transaction = new TransactionScope())
                {
                    foreach (var domainEvent in eventBatch)
                    {
                        session.Store(domainEvent);
                    }
                    session.SaveChanges();
                    transaction.Complete();
                }
            }
        }

        protected override IEnumerable<IDomainEvent> GetEvents(Guid aggregateId)
        {
            using(var session = _documentStore.OpenSession())
            {
                var events = session.Query<IDomainEvent>().Where(y => y.AggregateId == aggregateId);
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