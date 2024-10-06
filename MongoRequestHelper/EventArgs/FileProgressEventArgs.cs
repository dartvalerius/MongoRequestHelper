namespace MongoRequestHelper.EventArgs
{
    /// <summary>
    /// Аргументы события изменения прогресса скачивания или загрузки файла
    /// </summary>
    public class FileProgressEventArgs : BaseEventArgs
    {
        /// <summary>
        /// Процент скачивания или загрузки
        /// </summary>
        public double Percent { get; }

        /// <summary>
        /// Размер скаченных или загруженных данных
        /// </summary>
        public long Seek { get; }

        public FileProgressEventArgs(double percent, long seek)
        {
            Percent = percent;
            Seek = seek;
        }
    }
}