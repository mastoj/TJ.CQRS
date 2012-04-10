namespace TJ.CQRS.MongoEvent
{
    public interface IMongoConfiguration
    {
        string DatabaseName { get; set; }
        string Url { get; set; }
    }
}