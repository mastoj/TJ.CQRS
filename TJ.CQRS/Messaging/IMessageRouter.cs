using System;
using System.Collections.Generic;

namespace TJ.CQRS.Messaging
{
    public interface IEventRouter : IMessageRouter {}

    public interface ICommandRouter : IMessageRouter {}

    public interface IMessageRouter
    {
        void Register<TMessage>(Action<TMessage> route) where TMessage : class, IMessage;
        bool TryGetValue(Type commandType, out List<Action<IMessage>> handlers);
    }
}