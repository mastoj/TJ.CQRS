using System;

namespace TJ.CQRS.Respositories
{
    public interface IDomainRepository<TAggregate> where TAggregate : AggregateRoot
    {
        TAggregate Get(Guid aggregateId);
        void Insert(TAggregate aggregate);
    }
}