using TJ.CQRS.Infrastructure;
using TJ.CQRS.Infrastructure.Event;

namespace TJ.CQRS.MongoEvent.Tests
{
    public class MyEvent : DomainEventBase
    {
        public string SomeText { get; set; }
    }

    public class MyEvent2 : DomainEventBase
    {
        public string SomeText2 { get; set; }
    }
}
