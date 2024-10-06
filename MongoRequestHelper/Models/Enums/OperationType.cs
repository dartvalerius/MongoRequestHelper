namespace MongoRequestHelper.Models.Enums
{
    /// <summary>
    /// Типы событий аудита
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// Вставка
        /// </summary>
        Insert,

        /// <summary>
        /// Изменение
        /// </summary>
        Update,
        
        /// <summary>
        /// Замена
        /// </summary>
        Replace,

        /// <summary>
        /// Удаление
        /// </summary>
        Delete
    }
}