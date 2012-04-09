using System;

namespace TJ.CQRS.Infrastructure.Messaging
{
    public interface ICommand : IMessage
    {
        Guid AggregateId { get; }
    }
}
