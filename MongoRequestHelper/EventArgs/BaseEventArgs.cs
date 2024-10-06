using System;

namespace MongoRequestHelper.EventArgs
{
    /// <summary>
    /// Базовый класс аргументов событий работы с файлами
    /// </summary>
    public abstract class BaseEventArgs
    {
        /// <summary>
        /// Время возникновения события
        /// </summary>
        public DateTime CurrentTime { get; }

        protected BaseEventArgs()
        {
            CurrentTime = DateTime.Now;
        }
    }
}