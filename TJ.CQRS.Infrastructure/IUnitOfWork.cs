namespace TJ.CQRS.Infrastructure
{
    public interface IUnitOfWork
    {
        void Rollback();
        void Commit();
    }
}