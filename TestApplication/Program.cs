using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoRequestHelper.Utils;

namespace TestApplication;

internal static class Program
{
    private static DbContext? _context;
    private static void Main()
    {
        var connectionStringBuilder = new ConnectionStringBuilder
        {
            AuthenticationSource = "admin",
            DatabaseName = "TestDb",
            DirectConnection = false,
            UserName = "root",
            Password = "hSHi3jwscg",
            ReplicaSetName = "DAS",
            Servers = "localhost:38888,localhost:38889,localhost:38890",
            ApplicationName = "TestApplication",
            ConnectTimeout = TimeSpan.FromSeconds(10)
        };

        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));

        _context = new DbContext(connectionStringBuilder);

        Console.WriteLine(_context.IsConnectionOpen ? "Connect" : "Disconnect");
    }
}