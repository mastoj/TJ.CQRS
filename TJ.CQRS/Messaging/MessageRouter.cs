using System;
using System.Collections.Generic;
using TJ.Extensions;

namespace TJ.CQRS.Messaging
{
    public class MessageRouter : IMessageRouter
    {
        private Dictionary<Type, List<Action<IMessage>>> _messageRoutes;

        public MessageRouter()
        {
            _messageRoutes = new Dictionary<Type, List<Action<IMessage>>>();
        }

        public void Register<TMessage>(Action<TMessage> route) where TMessage : class, IMessage
        {
            List<Action<IMessage>> routes;
            var type = typeof(TMessage);
            if (_messageRoutes.TryGetValue(type, out routes).IsFalse())
            {
                routes = new List<Action<IMessage>>();
                _messageRoutes.Add(type, routes);
            }
            routes.Add((y) => route(y as TMessage));
        }

        public bool TryGetValue(Type commandType, out List<Action<IMessage>> handlers)
        {
            return _messageRoutes.TryGetValue(commandType, out handlers);
        }
    }
}