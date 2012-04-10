using System;

namespace TJ.CQRS.Messaging
{
    public interface ICommand : IMessage
    {
        Guid AggregateId { get; }
    }
}
