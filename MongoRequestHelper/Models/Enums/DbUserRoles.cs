namespace MongoRequestHelper.Models.Enums
{
    /// <summary>
    /// Стандартные роли для всех баз данных
    /// </summary>
    public static class DbUserRoles
    {
        public static string Read { get; } = "read";

        public static string ReadWrite { get; } = "readWrite";

        public static string DbAdmin { get; } = "dbAdmin";

        public static string DbOwner { get; } = "dbOwner";

        public static string UserAdmin { get; } = "userAdmin";
    }
}