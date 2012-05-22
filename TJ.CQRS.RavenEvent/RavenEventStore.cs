using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using TJ.CQRS.Event;
using TJ.CQRS.Messaging;

namespace TJ.CQRS.RavenEvent
{
    public class RavenEventStore : EventStore
    {
        private DocumentStore _documentStore;

        public RavenEventStore(IEventBus eventBus, string connectionStringName) : base(eventBus)
        {
//            var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(connectionString);
//            parser.Parse();
            _documentStore = new DocumentStore
                                 {
                                     ConnectionStringName = connectionStringName
                                     //Url = parser.ConnectionStringOptions.Url,
                                     //ApiKey = parser.ConnectionStringOptions.ApiKey,
                                     //Credentials = parser.ConnectionStringOptions.Credentials,
                                     //DefaultDatabase = parser.ConnectionStringOptions.DefaultDatabase,
                                     //ResourceManagerId = parser.ConnectionStringOptions.ResourceManagerId,
                                     //EnlistInDistributedTransactions = parser.ConnectionStringOptions.EnlistInDistributedTransactions
                                 };
            _documentStore.Initialize();
        }

        protected override void InsertBatch(IEnumerable<IDomainEvent> eventBatch)
        {
            using(var session = _documentStore.OpenSession())
            {
                var aggregateId = eventBatch.First().AggregateId;
                var _ravenAggregate =
                    session.Query<RavenEventEntity>().SingleOrDefault(y => y.AggregateId == aggregateId);
                if(_ravenAggregate == null)
                {
                    _ravenAggregate = new RavenEventEntity()
                                          {
                                              AggregateId = aggregateId,
                                              AggregateEvents = eventBatch.ToList()
                                          };
                    session.Store(_ravenAggregate);
                }
                else
                {
                    _ravenAggregate.AggregateEvents.AddRange(eventBatch);
                }
                session.SaveChanges();
            }
        }

        protected override IEnumerable<IDomainEvent> GetEvents(Guid aggregateId)
        {
            using(var session = _documentStore.OpenSession())
            {
                var events = session.Query<RavenEventEntity>().Single(y => y.AggregateId == aggregateId).AggregateEvents;
                return events;
            }
        }

        internal void DeleteCollection()
        {
            using (var session = _documentStore.OpenSession())
            {
                var events = session.Load<RavenEventEntity>();;
                foreach (var ravenEventEntity in events)
                {
                    session.Delete(ravenEventEntity);                    
                }
                session.SaveChanges();
            }

        }
    }
}