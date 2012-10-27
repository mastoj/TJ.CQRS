using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TJ.CQRS.Messaging;
using TJ.CQRS.Tests.Stub;

namespace TJ.CQRS.Tests
{
    [TestFixture]
    public class When_Loading_Aggregate_With_Empty_Event_Store : BaseTestSetup
    {
        private StubAggregate _aggregate;

        protected override void Given()
        {
            var stubUnitOfWork = new InMemoryEventBus(new MessageRouterStub());
            var eventStore = new StubEventStore(stubUnitOfWork);
            _aggregate = eventStore.Get<StubAggregate>(Guid.Empty);
        }

        [Test]
        public void Null_Should_Be_Returned()
        {
            _aggregate.Should().BeNull();
        }
    }

    [TestFixture]
    public class When_Commiting_Changes_On_A_Inserted_Aggregate : EventStoreTestBase
    {
        private StubAggregate _aggregate;
        private InMemoryEventBus _stubEventBus;
        private StubEventStore _eventStore;
        private List<IDomainEvent> _publishedEvents;

        protected override void Given()
        {
            _stubEventBus = new InMemoryEventBus(new MessageRouterStub());
            _publishedEvents = new List<IDomainEvent>();
            _stubEventBus.EventPublished = (y) => _publishedEvents.Add(y);
            _eventStore = new StubEventStore(_stubEventBus);
            _aggregate = new StubAggregate();
            _aggregate.DoThis();
            _aggregate.DoSomethingElse();
            _aggregate.DoThis();
            _aggregate.DoSomethingElse();
            _eventStore.Insert(_aggregate);
            _eventStore.Commit();
        }

        [Test]
        public void The_List_Of_Changes_Should_Be_Empty_On_The_Aggregate()
        {
            _aggregate.GetChanges().Count().Should().Be(0);
        }

        [Test]
        public void The_Uncommited_Events_Should_Be_Published_On_The_Bus_In_The_Right_Order()
        {
            var publishedEvents = _publishedEvents.ToList();
            CheckEvents(publishedEvents);
        }

        [Test]
        public void The_Events_Should_Be_Stored()
        {
            var insertedEvents = _eventStore.InsertedEvents.ToList();
            CheckEvents(insertedEvents);
        }
    }

    [TestFixture]
    public class When_Loading_An_Aggregate_With_A_Event_History : EventStoreTestBase
    {
        private StubAggregate _aggregate;

        protected override void Given()
        {
            var eventBus = new InMemoryEventBus(new MessageRouterStub());
            var eventStore = new StubEventStore(eventBus);
            var events = new List<IDomainEvent>();
            events.Add(new ValidEvent(Guid.Empty) { EventNumber = 0 });
            events.Add(new AnotherValidEvent(Guid.Empty) { EventNumber = 1 });
            events.Add(new ValidEvent(Guid.Empty) { EventNumber = 2 });
            events.Add(new AnotherValidEvent(Guid.Empty) { EventNumber = 3 });
            eventStore.InsertEvents(events);
            _aggregate = eventStore.Get<StubAggregate>(Guid.Empty);
        }

        [Test]
        public void All_The_Events_Should_Be_Applied_To_The_Aggregate()
        {
            var eventsApplied = _aggregate.EventsTriggered;
            CheckEvents(eventsApplied);
        }
    }

    [TestFixture]
    public class When_Requesting_All_Events
    {
        [Test]
        public void All_The_Events_Should_Be_Returned_In_The_Right_Order()
        {
            var eventBus = new InMemoryEventBus(new MessageRouterStub());
            var eventStore = new StubEventStore(eventBus);
            var events = new List<IDomainEvent>();
            events.Add(new ValidEvent(Guid.Empty) { EventNumber = 0 });
            events.Add(new AnotherValidEvent(Guid.Empty) { EventNumber = 1 });
            events.Add(new ValidEvent(Guid.Empty) { EventNumber = 2 });
            events.Add(new AnotherValidEvent(Guid.Empty) { EventNumber = 3 });
            eventStore.InsertEvents(events);

            var retrievedEvents = eventStore.GetAllEvents();

            Assert.AreEqual(4, retrievedEvents.Count());
            for(var count = 0; count < 4; count++)
            {
                var @event = retrievedEvents.ElementAt(count);
                Assert.AreEqual(count, @event.EventNumber);
                if(count % 2== 0)
                    Assert.IsInstanceOf<ValidEvent>(@event);
                else
                {
                    Assert.IsInstanceOf<AnotherValidEvent>(@event);
                }
            }
        }
    }

    public abstract class EventStoreTestBase : BaseTestSetup
    {
        protected void CheckEvents(List<IDomainEvent> events)
        {
            events.Count.Should().Be(4);
            for (int i = 0; i < events.Count; i++)
            {
                events[i].EventNumber.Should().Be(i);
                if (i % 2 == 0)
                    events[i].GetType().Should().Be(typeof(ValidEvent));
                else
                    events[i].GetType().Should().Be(typeof(AnotherValidEvent));
            }
        }        
    }
}
