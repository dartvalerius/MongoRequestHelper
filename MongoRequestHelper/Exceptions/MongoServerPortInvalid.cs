using System;
using System.Runtime.Serialization;

namespace MongoRequestHelper.Exceptions
{
    /// <summary>
    /// Исключение формата порта сервера
    /// </summary>
    [Serializable]
    public class MongoServerPortInvalidException : Exception
    {
        public MongoServerPortInvalidException()
        {
        }

        public MongoServerPortInvalidException(string message) : base(message)
        {
        }

        public MongoServerPortInvalidException(string message, Exception inner) : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        protected MongoServerPortInvalidException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}