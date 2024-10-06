using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MongoRequestHelper.Models
{
    /// <summary>
    /// Базовая информация о пользователе
    /// </summary>
    public class UserInfoBase
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Название базы данных
        /// </summary>
        public string Database { get; set; } = null!;

        /// <summary>
        /// Роли
        /// </summary>
        public IEnumerable<Role> Roles { get; set; } = null!;
    }

    /// <summary>
    /// Информация о пользователе
    /// </summary>
    internal class UserInfo : UserInfoBase
    {
        /// <summary>
        /// Пользовательские данные
        /// </summary>
        public BsonDocument? CustomData { get; set; }
    }

    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public sealed class UserInfo<TCustomData> : UserInfoBase where TCustomData : class
    {
        /// <summary>
        /// Пользовательские данные
        /// </summary>
        public TCustomData CustomData { get; }

        internal UserInfo(UserInfo userInfo)
        {
            Name = userInfo.Name;
            Id = userInfo.Id;
            Database = userInfo.Database;
            Roles = userInfo.Roles;
            CustomData = BsonSerializer.Deserialize<TCustomData>(userInfo.CustomData);
        }
    }
}