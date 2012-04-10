using System;
using System.Collections.Generic;
using TJ.CQRS.Exceptions;
using TJ.Extensions;

namespace TJ.CQRS.Messaging
{
    public interface IBus : ISendCommand, IPublishEvent, ICommitMessages
    { }

    public class InMemoryBus : IBus
    {
        private readonly IMessageRouter _messageRouter;
        private List<IDomainEvent> _publishedEvents;

        public InMemoryBus(IMessageRouter messageRouter)
        {
            _messageRouter = messageRouter;
#if DEBUG
            _publishedEvents = new List<IDomainEvent>();
#endif
        }

        public void Send<TCommand>(TCommand command) where TCommand : class, ICommand
        {
            var commandType = command.GetType();
            List<Action<IMessage>> handlers;
            if (_messageRouter.TryGetValue(commandType, out handlers))
            {
                foreach (var handler in handlers)
                {
                    handler(command);
                }
                Commit();
            }
            else
            {
                throw new UnregisteredCommandException("No command handler registered for command type: " + commandType);
            }
        }

        public void PublishEvent<TEvent>(TEvent @event) where TEvent : class, IDomainEvent
        {
            var eventType = @event.GetType();
            List<Action<IMessage>> eventHandlers;
            if (_messageRouter.TryGetValue(eventType, out eventHandlers).IsTrue())
            {
                foreach (var eventHandler in eventHandlers)
                {
                    eventHandler(@event);
                }
            }
#if DEBUG
            _publishedEvents.Add(@event);
#endif
        }

        public void PublishEvents<TEvent>(IEnumerable<TEvent> events) where TEvent : class, IDomainEvent
        {
            foreach (var @event in events)
            {
                PublishEvent(@event);
            }
        }

        public List<IDomainEvent> PublishedEvents
        {
            get { return _publishedEvents; }
        }

        public event CommitMessageHandler Commit = () => { };
    }
}