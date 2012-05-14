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

        public IDomainRepository<TAggregate> GetDomainRepository<TAggregate>() where TAggregate : AggregateRoot, new()
        {
            return new DomainRepository<TAggregate>(_eventStore);
        }
    }
}