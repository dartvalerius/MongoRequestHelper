using System;
using System.Runtime.Serialization;

namespace MongoRequestHelper.Exceptions
{
    /// <summary>
    /// Исключение базы данных
    /// </summary>
    [Serializable]
    public class MongoDatabaseException : Exception
    {
        public MongoDatabaseException()
        {
        }

        public MongoDatabaseException(string message) : base(message)
        {
        }

        public MongoDatabaseException(string message, Exception inner) : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        protected MongoDatabaseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}