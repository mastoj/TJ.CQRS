namespace TJ.CQRS.Infrastructure.Messaging
{
    public delegate void CommitMessageHandler();
    public interface ICommitMessages
    {
        event CommitMessageHandler Commit;
    }
}