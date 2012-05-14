using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TJ.CQRS.Event;

namespace TJ.CQRS.Respositories
{
    public class DomainRepositoryFactory : IDomainRepositoryFactory
    {
        private IEventStore _eventStore;

        public DomainRepositoryFactory(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public DomainRepository<TAggregate> GetDomainRepository<TAggregate>() where TAggregate : AggregateRoot, new()
        {
            return new DomainRepository<TAggregate>(_eventStore);
        }
    }

    public interface IDomainRepositoryFactory
    {
        DomainRepository<TAggregate> GetDomainRepository<TAggregate>() where TAggregate : AggregateRoot, new();
    }

    public class DomainRepository<TAggregate> : IDomainRepository<TAggregate> where TAggregate : AggregateRoot, new()
    {
        private readonly IEventStore _eventStore;

        public DomainRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public TAggregate Get(Guid aggregateId)
        {
            var aggregate = _eventStore.Get<TAggregate>(aggregateId);
            return aggregate;
        }

        public void Insert(TAggregate aggregate)
        {
            _eventStore.Insert(aggregate);
        }
    }
}
