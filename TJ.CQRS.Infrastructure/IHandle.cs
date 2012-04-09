namespace TJ.CQRS.Infrastructure
{
    public interface IHandle<T>
    {
        void Handle(T thingToHandle);
    }
}