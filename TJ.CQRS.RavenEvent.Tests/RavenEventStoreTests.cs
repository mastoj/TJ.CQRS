using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TJ.CQRS.Messaging;
using TJ.CQRS.Tests;

namespace TJ.CQRS.RavenEvent.Tests
{
    [TestFixture]
    public class ShouldNotFailTests
    {
        [Test]
        public void ShouldNotFail()
        {
            var id = Guid.NewGuid();
            var eventPublisher = new InMemoryEventBus(new MessageRouterStub());
            var eventStore = new RavenEventStore(eventPublisher, "RavenDB");
            eventStore.DeleteCollection();
            var events = new List<IDomainEvent>
                             {
                                 new ValidEvent(id),
                                 new AnotherValidEvent(id),
                                 new ValidEvent(id),
                                 new AnotherValidEvent(id)
                             };
            eventStore.InsertBatchTest(events);
            var eventsFromStore = eventStore.GetEventsTest(id);
            foreach (var domainEvent in eventsFromStore)
            {
                events.Should().Contain(domainEvent);
            }
            
        }
    }

    [TestFixture]
    public class RavenEventStoreTests
    {
        private Guid _aggregateId;
        private StubAggregate _aggregate;
        private IEventBus _eventPublisher;

        [TestFixtureSetUp]
        public void Setup()
        {
            // Arrange
            _eventPublisher = new InMemoryEventBus(new MessageRouterStub());
            var eventStore = new RavenEventStore(_eventPublisher, "RavenDB");
            eventStore.DeleteCollection();
            _aggregate = new StubAggregate();
            _aggregate.AggregateId = Guid.NewGuid();
            _aggregate.DoThis();
            _aggregate.DoSomethingElse();
            _aggregate.DoThis();
            _aggregate.DoSomethingElse();
            _aggregateId = _aggregate.AggregateId;
            eventStore.Insert(_aggregate);
            eventStore.Commit();
        }

        [Test]
        public void All_Changes_Should_Be_Cleared()
        {
            _aggregate.GetChanges().Count().Should().Be(0);
        }

        [Test]
        public void And_All_Events_Should_Be_Applied_When_Loading_From_EventStore_In_The_Right_Order()
        {
            Func<RavenEventStore, IEnumerable<IDomainEvent>> getEventsFunc =
                (y) =>
                    {
                        var loadedAggregate = y.Get<StubAggregate>(_aggregateId);
                        var appliedEvents = loadedAggregate.EventsTriggered;
                        return appliedEvents;
                    };
            VerifyEvents(getEventsFunc);
        }

        [Test]
        public void And_I_Should_Be_Able_To_Get_All_Events()
        {
            Func<RavenEventStore, IEnumerable<IDomainEvent>> getEventsFunc = (y) => y.GetAllEvents();
            VerifyEvents(getEventsFunc);
        }

        private void VerifyEvents(Func<RavenEventStore, IEnumerable<IDomainEvent>> getEventsFunc)
        {
            var eventStore = new RavenEventStore(_eventPublisher, "RavenDB");
            var @events = getEventsFunc(eventStore).ToList();
            @events.Count.Should().Be(4);
            for (int i = 0; i < @events.Count; i++)
            {
                @events[i].EventNumber.Should().Be(i);
                if (i % 2 == 0)
                    @events[i].GetType().Should().Be(typeof(ValidEvent));
                else
                    @events[i].GetType().Should().Be(typeof(AnotherValidEvent));
            }
        }
    }
}
