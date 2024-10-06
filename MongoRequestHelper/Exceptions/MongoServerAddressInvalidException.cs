using System;
using System.Runtime.Serialization;

namespace MongoRequestHelper.Exceptions
{
    /// <summary>
    /// Исключение формата адреса сервера
    /// </summary>
    [Serializable]
    public class MongoServerAddressInvalidException : Exception
    {
        public MongoServerAddressInvalidException()
        {
        }

        public MongoServerAddressInvalidException(string message) : base(message)
        {
        }

        public MongoServerAddressInvalidException(string message, Exception inner) : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        protected MongoServerAddressInvalidException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}