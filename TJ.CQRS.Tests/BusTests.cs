using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TJ.CQRS.Event;
using TJ.CQRS.Messaging;

namespace TJ.CQRS.Tests
{
    public class When_Publishing_An_Event : BaseTestSetup
    {
        private InMemoryEventBus _eventBus;
        private EventHandler<ValidEvent> _validEventHandler1;
        private EventHandler<ValidEvent> _validEventHandler2;
        private EventHandler<AnotherValidEvent> _anotherValidEventHandler;

        protected override void Given()
        {
            _validEventHandler1 = new EventHandler<ValidEvent>();
            _validEventHandler2 = new EventHandler<ValidEvent>();
            _anotherValidEventHandler = new EventHandler<AnotherValidEvent>();
            var messageRouter = new MessageRouter();
            messageRouter.Register<ValidEvent>(_validEventHandler1.Handle);
            messageRouter.Register<ValidEvent>(_validEventHandler2.Handle);
            messageRouter.Register<AnotherValidEvent>(_anotherValidEventHandler.Handle);
            _eventBus = new InMemoryEventBus(messageRouter);

            _eventBus.PublishEvent(new ValidEvent(Guid.Empty));
        }

        [Test]
        public void All_Event_Handlers_For_The_Event_Should_Be_Executed()
        {
            _validEventHandler1.ExecutionCount.Should().Be(1);
            _validEventHandler2.ExecutionCount.Should().Be(1);
        }

        [Test]
        public void And_Event_Other_Event_Handlers_Should_Not_Be_Executed()
        {
            _anotherValidEventHandler.ExecutionCount.Should().Be(0);
        }
    }

    internal class EventHandler<T> : IHandle<T> where T : IDomainEvent
    {
        private int _executionCount;

        public int ExecutionCount
        {
            get { return _executionCount; }
        }

        public void Handle(T thingToHandle)
        {
            _executionCount = ExecutionCount + 1;
        }
    }
}
