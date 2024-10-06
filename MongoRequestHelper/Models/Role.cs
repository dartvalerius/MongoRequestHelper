namespace MongoRequestHelper.Models
{
    /// <summary>
    /// Класс описания роли
    /// </summary>
    public sealed class Role
    {
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// База данных к которой относится роль
        /// </summary>
        public string Database { get; set; }

        public Role(string name, string database)
        {
            Name = name;
            Database = database;
        }
    }
}