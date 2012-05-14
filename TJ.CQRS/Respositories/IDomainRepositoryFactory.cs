namespace TJ.CQRS.Respositories
{
    public interface IDomainRepositoryFactory
    {
        IDomainRepository<TAggregate> GetDomainRepository<TAggregate>() where TAggregate : AggregateRoot, new();
    }
}