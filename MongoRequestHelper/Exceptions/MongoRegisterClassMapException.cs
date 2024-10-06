using System;
using System.Runtime.Serialization;

namespace MongoRequestHelper.Exceptions
{
    /// <summary>
    /// Исключение регистрации карты класса
    /// </summary>
    [Serializable]
    public class MongoRegisterClassMapException : Exception
    {
        public MongoRegisterClassMapException()
        {
        }

        public MongoRegisterClassMapException(string message) : base(message)
        {
        }

        public MongoRegisterClassMapException(string message, Exception inner) : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        protected MongoRegisterClassMapException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}