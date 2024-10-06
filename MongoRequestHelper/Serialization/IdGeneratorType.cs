namespace MongoRequestHelper.Serialization
{
    /// <summary>
    /// Тип генератора идентификатора
    /// </summary>
    public enum IdGeneratorType
    {
        /// <summary>
        /// Неизвестный формат
        /// </summary>
        Undefined,

        /// <summary>
        /// Идентификатора в формате GUID
        /// </summary>
        GuidId,

        /// <summary>
        /// Идентификатор в формате ObjectId
        /// </summary>
        ObjectId,

        /// <summary>
        /// Идентификатор в строковом формате ObjectId
        /// </summary>
        StringObjectId
    }
}