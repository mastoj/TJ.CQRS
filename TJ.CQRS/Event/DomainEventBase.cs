using System;
using TJ.CQRS.Messaging;
using TJ.Extensions;

namespace TJ.CQRS.Event
{
    public abstract class DomainEventBase : IDomainEvent
    {
        public DateTime TimeStamp { get; set; }

        public DomainEventBase()
        {
            TimeStamp = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public int EventNumber { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if ((obj is DomainEventBase).IsFalse()) return false;
            return Equals((DomainEventBase) obj);
        }

        public bool Equals(DomainEventBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.AggregateId.Equals(AggregateId);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash*23 + AggregateId.GetHashCode();
                hash = hash*23 + EventNumber.GetHashCode();
                return hash;
            }
        }
    }
}