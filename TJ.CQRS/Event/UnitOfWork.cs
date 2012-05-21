namespace TJ.CQRS.Event
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IEventStore _eventStore;

        public UnitOfWork(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public void Rollback()
        {
            _eventStore.Rollback();
        }

        public void Commit()
        {
            _eventStore.Commit();
        }
    }
}