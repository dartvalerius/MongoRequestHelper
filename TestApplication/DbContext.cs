using MongoRequestHelper;
using MongoRequestHelper.Utils;
using TestApplication.ClassMaps;

namespace TestApplication;

public class DbContext : MongoContext
{
    public DbContext(ConnectionStringBuilder connectionStringBuilder) : base(connectionStringBuilder)
    {
    }

    protected override void OnRegisterClassMap()
    {
        PersonClassMap.Register();
        UserClassMap.Register();
    }
}