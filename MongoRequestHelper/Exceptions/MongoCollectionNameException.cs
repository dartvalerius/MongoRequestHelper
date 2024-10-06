using System;
using System.Runtime.Serialization;

namespace MongoRequestHelper.Exceptions
{
    /// <summary>
    /// Исключение имени коллекции
    /// </summary>
    [Serializable]
    public class MongoCollectionNameException : Exception
    {
        public MongoCollectionNameException()
        {
        }

        public MongoCollectionNameException(string message) : base(message)
        {
        }

        public MongoCollectionNameException(string message, Exception inner) : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        protected MongoCollectionNameException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}