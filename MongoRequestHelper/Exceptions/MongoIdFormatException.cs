using System;
using System.Runtime.Serialization;

namespace MongoRequestHelper.Exceptions
{
    /// <summary>
    /// Исключение формата идентификатора
    /// </summary>
    [Serializable]
    public class MongoIdFormatException : Exception
    {
        public MongoIdFormatException()
        {
            
        }

        public MongoIdFormatException(string message) : base(message)
        {
        }

        public MongoIdFormatException(string message, Exception inner) : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        protected MongoIdFormatException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}