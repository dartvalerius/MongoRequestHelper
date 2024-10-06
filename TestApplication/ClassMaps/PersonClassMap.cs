using MongoRequestHelper.Serialization;
using TestApplication.Models;

namespace TestApplication.ClassMaps;

public static class PersonClassMap
{
    public static void Register()
    {
        var classMap = new MongoClassMap<Person>();

        classMap.AutoMap();

        classMap.SetIgnoreExtraElements();

        classMap
            .MapIdMember(p => p.Id)
            .SetIdGenerator(IdGeneratorType.StringObjectId)
            .SetStringToObjectId()
            .SetOrder(1);

        classMap
            .MapMember(p => p.FirstName)
            .SetIsRequired()
            .SetOrder(2);

        classMap
            .MapMember(p => p.MiddleName)
            .SetIgnoreIfDefault()
            .SetOrder(3);

        classMap
            .MapMember(p => p.LastName)
            .SetIsRequired()
            .SetOrder(4);

        classMap
            .MapMember(p => p.BirthDay)
            .SetIsRequired()
            .SetDateTimeLocal()
            .SetOrder(5);

        classMap
            .MapMember(p => p.Sex)
            .SetIsRequired()
            .SetEnumToString<SexType>()
            .SetOrder(6);

        classMap
            .MapMember(p => p.Citizenship)
            .SetIsRequired()
            .SetOrder(7);

        classMap
            .MapMember(p => p.Family)
            .SetListStringToListObjectId()
            .SetIgnoreIfNull()
            .SetOrder(8);

        classMap
            .MapMember(p => p.DateCreateUtc)
            .SetIsRequired()
            .SetElementName("DateCreate")
            .SetDefaultValue(() => DateTime.Now)
            .SetDateTimeUtc()
            .SetOrder(9);

        classMap
            .MapMember(p => p.UserCreate)
            .SetIsRequired()
            .SetDefaultValue("dartvalerius")
            .SetOrder(10);

        classMap
            .MapMember(p => p.DateUpdateUtc)
            .SetElementName("DateUpdate")
            .SetIgnoreIfDefault()
            .SetDateTimeUtc()
            .SetOrder(11);

        classMap
            .MapMember(p => p.UserUpdate)
            .SetIgnoreIfNull()
            .SetOrder(12);

        classMap.Registration();
    }
}