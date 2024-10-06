using System;
using MongoDB.Bson;

namespace MongoRequestHelper.Utils
{
    public static class MongoId
    {
        public static DateTime? ObjectIdToDateTime(string objectId)
        {
            if (ObjectId.TryParse(objectId, out var id))
                return id.CreationTime;

            return null;
        }

        public static string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public static bool IsObjectId(string id)
        {
            return !string.IsNullOrEmpty(id) && ObjectId.TryParse(id, out _);
        }
    }
}