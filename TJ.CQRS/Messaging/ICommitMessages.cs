namespace TJ.CQRS.Messaging
{
    public delegate void CommitMessageHandler();
    public interface ICommitMessages
    {
        event CommitMessageHandler Commit;
    }
}