using System;
using System.Runtime.Serialization;

namespace MongoRequestHelper.Exceptions
{
    /// <summary>
    /// Исключение неправильного типа элемента класса
    /// </summary>
    [Serializable]
    public class MongoInvalidMemberTypeException : Exception
    {
        public MongoInvalidMemberTypeException()
        {
        }

        public MongoInvalidMemberTypeException(string message) : base(message)
        {
        }

        public MongoInvalidMemberTypeException(string message, Exception inner) : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        protected MongoInvalidMemberTypeException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}