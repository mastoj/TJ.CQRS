namespace TJ.CQRS
{
    public interface IHandle<T>
    {
        void Handle(T thingToHandle);
    }
}