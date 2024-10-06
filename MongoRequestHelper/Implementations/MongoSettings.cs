using MongoRequestHelper.Interfaces;

namespace MongoRequestHelper.Implementations
{
    internal class MongoSettings : IMongoSettings
    {
        public string DatabaseName { get; set; } = null!;
        public string ConnectionString { get; set; } = null!;
    }
}