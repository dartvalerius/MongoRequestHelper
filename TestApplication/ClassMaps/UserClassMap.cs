using MongoRequestHelper.Serialization;
using TestApplication.Models;

namespace TestApplication.ClassMaps;

public static class UserClassMap
{
    public static void Register()
    {
        var classMap = new MongoClassMap<User>();

        classMap.AutoMap();

        classMap.SetIgnoreExtraElements();

        classMap
            .MapIdMember(p => p.Id)
            .SetIdGenerator(IdGeneratorType.GuidId)
            .SetOrder(1);

        classMap
            .MapMember(p => p.Name)
            .SetIsRequired()
            .SetOrder(2);

        classMap
            .MapMember(p => p.ListGuid)
            .SetIgnoreIfDefault()
            .SetListGuidToListString()
            .SetOrder(3);

        classMap.Registration();
    }
}