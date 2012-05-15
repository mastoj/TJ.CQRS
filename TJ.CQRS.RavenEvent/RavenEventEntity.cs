using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TJ.CQRS.Messaging;

namespace TJ.CQRS.RavenEvent
{
    internal class RavenEventEntity
    {
        internal List<IDomainEvent> AggregateEvents { get; set; }
        internal Guid AggregateId { get; set; }
    }
}
