namespace TJ.CQRS
{
    public interface IUnitOfWork
    {
        void Rollback();
        void Commit();
    }
}