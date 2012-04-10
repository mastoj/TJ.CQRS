namespace TJ.CQRS.MongoEvent
{
    public class MongoConfiguration : IMongoConfiguration
    {
        private string _databaseName;
        public string DatabaseName
        {
            get
            {
                _databaseName = _databaseName ?? "MongoDB";
                return _databaseName;
            }
            set { _databaseName = value; }
        }

        private string _url;
        public string Url
        {
            get
            {
                _url = _url ?? "mongodb://localhost";
                return _url;
            }
            set { _url = value; }
        }
    }
}