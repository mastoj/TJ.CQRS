using System;
using System.Collections.Generic;
using TJ.CQRS.Exceptions;
using TJ.Extensions;

namespace TJ.CQRS.Messaging
{
    public interface ICommandBus
    {
        void Send<TCommand>(TCommand command) where TCommand : class, ICommand;
    }

    public interface IEventBus
    {
        void PublishEvent<TEvent>(TEvent @event) where TEvent : class, IDomainEvent;
        void PublishEvents<TEvent>(IEnumerable<TEvent> events) where TEvent : class, IDomainEvent;
    }

    public class InMemoryCommandBus : ICommandBus
    {
        private readonly ICommandRouter _messageRouter;
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryCommandBus(ICommandRouter messageRouter, IUnitOfWork unitOfWork)
        {
            _messageRouter = messageRouter;
            _unitOfWork = unitOfWork;
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
                _unitOfWork.Commit();
            }
            else
            {
                throw new UnregisteredCommandException("No command handler registered for command type: " + commandType);
            }
        }
    }

    public class InMemoryEventBus : IEventBus
    {
        private readonly IEventRouter _messageRouter;
        public Action<IDomainEvent> _eventPublished;
        public Action<IDomainEvent> EventPublished
        {
            get
            {
                _eventPublished = _eventPublished ?? ((y) => { });
                return _eventPublished;
            }
            set { _eventPublished = value; }
        }

        public InMemoryEventBus(IEventRouter messageRouter)
        {
            _messageRouter = messageRouter;
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
            EventPublished(@event);
        }

        public void PublishEvents<TEvent>(IEnumerable<TEvent> events) where TEvent : class, IDomainEvent
        {
            foreach (var @event in events)
            {
                PublishEvent(@event);
            }
        }
    }
}