using System;
using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TJ.CQRS.Event;
using TJ.CQRS.Messaging;
namespace TJ.CQRS.MongoEvent
{
    public class MongoEventStore : EventStore
    {
        private MongoDatabase _database;
        private MongoServer _server;
        private string _collectionName = "Events";

        public MongoEventStore(IMongoConfiguration mongoConfiguration, IEventBus eventBus)
            : base(eventBus)
        {
            _server = MongoServer.Create(mongoConfiguration.Url);
            var mongoDatabaseSettings = _server.CreateDatabaseSettings(mongoConfiguration.DatabaseName);
            _database = _server.GetDatabase(mongoDatabaseSettings);
        }

        protected override IEnumerable<IDomainEvent> GetEvents(Guid aggregateId)
        {
            MongoCollection<IDomainEvent> events = _database.GetCollection<IDomainEvent>(_collectionName);
            var query = Query.EQ("AggregateId", aggregateId);
            var cursor = events.FindAs<IDomainEvent>(query).SetSortOrder(SortBy.Ascending("EventNumber"));
            foreach (var domainEvent in cursor)
            {
                yield return domainEvent;
            }
        }

        public override IEnumerable<IDomainEvent> GetAllEvents()
        {
            throw new NotImplementedException();
        }

        protected override void InsertBatch(IEnumerable<IDomainEvent> eventBatch)
        {
            MongoCollection<IDomainEvent> events = _database.GetCollection<IDomainEvent>(_collectionName);
            events.InsertBatch(eventBatch);
        }

        public void DeleteCollection()
        {
            _database.DropCollection(_collectionName);
        }
    }
}