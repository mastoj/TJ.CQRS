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
    public class RavenEventStoreTests
    {
        private Guid _aggregateId;
        private StubAggregate _aggregate;
        private IEventBus _eventPublisher;

        [TestFixtureSetUp]
        public void Setup()
        {
            // Arrange
            _eventPublisher = new InMemoryEventBus(new MessageRouter());
            RavenConfiguration configuration = new RavenConfiguration()
                                                   {
                                                       Url = "http://localhost:8090/"
                                                   };
            var eventStore = new RavenEventStore(_eventPublisher, configuration);
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
            RavenConfiguration configuration = new RavenConfiguration()
            {
                Url = "http://localhost:8090/"
            };
            var eventStore = new RavenEventStore(_eventPublisher, configuration);
            var loadedAggregate = eventStore.Get<StubAggregate>(_aggregateId);
            var appliedEvents = loadedAggregate.EventsTriggered;
            appliedEvents.Count.Should().Be(4);
            for (int i = 0; i < appliedEvents.Count; i++)
            {
                appliedEvents[i].EventNumber.Should().Be(i);
                if (i % 2 == 0)
                    appliedEvents[i].GetType().Should().Be(typeof(ValidEvent));
                else
                    appliedEvents[i].GetType().Should().Be(typeof(AnotherValidEvent));
            }
        }
    }
}
