using System;
using System.Runtime.Serialization;

namespace MongoRequestHelper.Exceptions
{
    /// <summary>
    /// Исключение размера файла
    /// </summary>
    [Serializable]
    public class MongoFileLengthException : Exception
    {
        public MongoFileLengthException()
        {
        }

        public MongoFileLengthException(string message) : base(message)
        {
        }

        public MongoFileLengthException(string message, Exception inner) : base(message, inner)
        {
        }

        [Obsolete("Obsolete")]
        protected MongoFileLengthException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}