namespace MongoRequestHelper.Interfaces
{
    /// <summary>
    /// Данные для подключения к серверу
    /// </summary>
    public interface IMongoSettings
    {
        /// <summary>
        /// Название базы данных
        /// </summary>
        string DatabaseName { get; set; }

        /// <summary>
        /// Строка подключения
        /// </summary>
        string ConnectionString { get; set; }
    }
}