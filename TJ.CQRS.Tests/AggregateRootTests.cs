using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using TJ.CQRS.Event;
using TJ.CQRS.Exceptions;
using TJ.CQRS.Messaging;

namespace TJ.CQRS.Tests
{
    [TestFixture]
    public class When_I_Do_Something_Illegal
    {
        private StubAggregate _aggregate;

        [TestFixtureSetUp]
        public void Setup()
        {
            _aggregate = new StubAggregate();
        }

        [Test]
        public void Then_I_Get_An_Unregistered_Event_Exception()
        {
            // Assert
            Action act = () => { _aggregate.SomethingIShouldNotDo(); };
            act.ShouldThrow<UnregisteredEventException>();
        }
    }

    public class When_I_Load_Aggregate_From_Events
    {
        private StubAggregate _aggregate;

        [TestFixtureSetUp]
        public void Setup()
        {
            _aggregate = new StubAggregate();
            var aggregateId = Guid.NewGuid();
            var events = new List<IDomainEvent>();
            for (int i = 0; i < 5; i++)
            {
                var validEvent = new ValidEvent(aggregateId);
                validEvent.EventNumber = i;
                events.Add(validEvent);
            }
            _aggregate.LoadAggregate(events);
        }

        [Test]
        public void The_Next_Event_Should_Get_Next_Version_Number()
        {
            _aggregate.DoThis();
            _aggregate.GetChanges().First().EventNumber.Should().Be(4);
        }
    }

    public class StubAggregate : AggregateRoot
    {
        private List<IDomainEvent> _eventsTriggered;

        public List<IDomainEvent> EventsTriggered { get { return _eventsTriggered; } } 

        public StubAggregate()
        {
            RegisterEventHandler<ValidEvent>(OkAction);
            RegisterEventHandler<AnotherValidEvent>(AnotherOkAction);
            _eventsTriggered = new List<IDomainEvent>();
        }

        public void DoThis()
        {
            Apply(new ValidEvent(AggregateId));
        }

        public void DoSomethingElse()
        {
            Apply(new AnotherValidEvent(AggregateId));
        }

        private void OkAction(ValidEvent obj)
        {
            _eventsTriggered.Add(obj);
        }

        private void AnotherOkAction(AnotherValidEvent obj)
        {
            _eventsTriggered.Add(obj);
        }

        public void SomethingIShouldNotDo()
        {
            Apply(new ShouldNotEvent(AggregateId));
        }
    }

    public class ValidEvent : DomainEventBase
    {
        private static int propCount = 0;
        public ValidEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
            AProp = propCount++;
        }

        public int AProp { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            var otherObj = obj as ValidEvent;
            if (otherObj == null) return false;
            return AggregateId == otherObj.AggregateId && AProp == otherObj.AProp;
        }

        public override int GetHashCode()
        {
            return AggregateId.GetHashCode() * 27 + EventNumber.GetHashCode();
        }
    }

    public class AnotherValidEvent : DomainEventBase
    {
        private static int propCount2 = 20;
        public AnotherValidEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
            BProp = propCount2++;
        }

        public int BProp { get; set; }
        // override object.Equals
        public override bool Equals(object obj)
        {
            var otherObj = obj as AnotherValidEvent;
            if (otherObj == null) return false;
            return AggregateId == otherObj.AggregateId && BProp == otherObj.BProp;
        }

        public override int GetHashCode()
        {
            return AggregateId.GetHashCode()*27 + EventNumber.GetHashCode();
        }

    }

    public class ShouldNotEvent : DomainEventBase
    {
        public ShouldNotEvent(Guid aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}
