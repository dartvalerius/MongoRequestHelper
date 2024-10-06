using System;

namespace MongoRequestHelper.EventArgs
{
    /// <summary>
    /// Аргументы события возникновения исключения работы с файлом
    /// </summary>
    public class FileCrashEventArgs : BaseEventArgs
    {
        /// <summary>
        /// Экземпляр возникшего исключения
        /// </summary>
        public Exception InnerException { get; }

        public FileCrashEventArgs(Exception innerException)
        {
            InnerException = innerException;
        }
    }
}