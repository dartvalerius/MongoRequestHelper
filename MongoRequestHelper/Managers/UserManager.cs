using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoRequestHelper.Exceptions;
using MongoRequestHelper.Models;
using MongoRequestHelper.Utils;

namespace MongoRequestHelper.Managers
{
    /// <summary>
    /// Работа с пользователями
    /// </summary>
    public class UserManager
    {
        private readonly MongoContext _context;

        public UserManager(MongoContext context)
        {
            _context = context;
        }

        private BsonDocument ConvertRoleToBsonDocument(Role role)
        {
            return new BsonDocument
            {
                new BsonElement("role", role.Name),
                new BsonElement("db",
                    string.IsNullOrEmpty(role.Database)
                        ? _context.Database.DatabaseNamespace.DatabaseName
                        : role.Database)
            };
        }

        private BsonDocument CreateUser(string username, string password, IEnumerable<Role> roles)
        {
            var document = new BsonDocument
            {
                { "createUser", username },
                { "pwd", password },
                {
                    "roles",
                    new BsonArray(roles.Select(ConvertRoleToBsonDocument))
                }
            };

            return document;
        }

        private BsonDocument CreateUser<TCustomData>(string username, string password, IEnumerable<Role> roles, TCustomData customData)
        {
            var document = CreateUser(username, password, roles);

            document.Add(new BsonElement("customData", customData.ToBsonDocument()));

            return document;
        }

        private BsonDocument DropUser(string username)
        {
            var document = new BsonDocument
            {
                { "dropUser", username }
            };

            return document;
        }

        private BsonDocument GrantRolesToUser(string username, IEnumerable<Role> roles)
        {
            var document = new BsonDocument
            {
                { "grantRolesToUser", username },
                {
                    "roles",
                    new BsonArray(roles.Select(ConvertRoleToBsonDocument))
                }
            };

            return document;
        }

        private BsonDocument RevokeRolesFromUser(string username, IEnumerable<Role> roles)
        {
            var document = new BsonDocument
            {
                { "revokeRolesFromUser", username },
                {
                    "roles",
                    new BsonArray(roles.Select(ConvertRoleToBsonDocument))
                }
            };

            return document;
        }

        private BsonDocument UpdateUserPassword(string username, string password)
        {
            var document = new BsonDocument
            {
                { "updateUser", username },
                { "pwd", password }
            };

            return document;
        }

        private BsonDocument UpdateUserCustomData<T>(string username, T customData)
        {
            var document = new BsonDocument
            {
                { "updateUser", username },
                { "customData", customData.ToBsonDocument() }
            };

            return document;
        }

        private BsonDocument UsersInfo(string? userName = null)
        {
            var document = new BsonDocument
            {
                string.IsNullOrEmpty(userName)
                    ? new BsonElement("usersInfo", 1)
                    : new BsonElement("usersInfo", userName)
            };

            return document;
        }

        private UserInfo GetUserInfo(BsonDocument user)
        {
            var userInfo = new UserInfo
            {
                Name = user.GetValue("user").AsString,
                Id = user.GetValue("userId").AsGuid.ToString(),
                CustomData = user.GetValue("customData", new BsonDocument()).AsBsonDocument,
                Database = user.GetValue("db").AsString,
                Roles = user.GetValue("roles").AsBsonArray
                    .Select(doc => new Role(doc["role"].AsString, doc["db"].AsString))
            };

            return userInfo;
        }

        private UserInfo<TCustomData> GetUserInfo<TCustomData>(BsonDocument user) where TCustomData : class
        {
            var ui = GetUserInfo(user);

            return new UserInfo<TCustomData>(ui);
        }


        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <param name="roles">Роли пользователя</param>
        /// <returns><c>true</c> - пользователь добавлен успешно</returns>
        public void Create(string username, string password, IEnumerable<Role> roles)
        {
            try
            {
                _context.Database.RunCommand<BsonDocument>(CreateUser(username, password, roles));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <typeparam name="TCustomData">Тип пользовательских данных</typeparam>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <param name="roles">Роли пользователя</param>
        /// <param name="customData">Пользовательские данные</param>
        /// <returns><c>true</c> - пользователь добавлен успешно</returns>
        public void Create<TCustomData>(string username, string password, IEnumerable<Role> roles, TCustomData customData)
        {
            try
            {
                _context.Database.RunCommand<BsonDocument>(CreateUser(username, password, roles, customData));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <returns><c>true</c> - пользователь удалён успешно</returns>
        public void Drop(string username)
        {
            try
            {
                _context.Database.RunCommand<BsonDocument>(DropUser(username));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Добавление ролей пользователю
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="roles">Словарь, где key - название роли из статического класса DbUserRoles,
        /// value - название базы данных, где находится выбранная роль, либо null или пустая строка,
        /// если роль существует в той же базе данных</param>
        /// <returns><c>true</c> - роли добавлены успешно</returns>
        public void GrantRoles(string username, IEnumerable<Role> roles)
        {
            try
            {
                _context.Database.RunCommand<BsonDocument>(GrantRolesToUser(username, roles));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Удаление ролей пользователя
        /// </summary>
        /// <param name="username">ИМя пользователя</param>
        /// <param name="roles">Удаляемые роли</param>
        /// <returns><c>true</c> - роли удалены успешно</returns>
        public void RevokeRoles(string username, IEnumerable<Role> roles)
        {
            try
            {
                _context.Database.RunCommand<BsonDocument>(RevokeRolesFromUser(username, roles));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="newPassword">Пароль пользователя</param>
        /// <returns><c>true</c> - пароль изменён успешно</returns>
        public void ChangePassword(string username, string newPassword)
        {
            try
            {
                _context.Database.RunCommand<BsonDocument>(UpdateUserPassword(username, newPassword));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Изменение пользовательских данных
        /// </summary>
        /// <typeparam name="T">Тип пользовательских данных</typeparam>
        /// <param name="username">Имя пользователя</param>
        /// <param name="customData">Пользовательские данные</param>
        /// <returns><c>true</c> - пользовательские данные изменены успешно</returns>
        public void UpdateCustomData<T>(string username, T customData)
        {
            try
            {
                _context.Database.RunCommand<BsonDocument>(UpdateUserCustomData(username, customData));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить информацию о пользователях текущей базы данных
        /// </summary>
        /// <typeparam name="TCustomData">Тип пользовательских данных</typeparam>
        /// <returns>Список с информацией о пользователях</returns>
        public IEnumerable<UserInfo<TCustomData>> List<TCustomData>() where TCustomData : class
        {
            try
            {
                var result = _context.Database.RunCommand<BsonDocument>(UsersInfo());

                return result["users"].AsBsonArray.Select(user => GetUserInfo<TCustomData>(user.AsBsonDocument));
            }
            catch (Exception)
            {
                return new List<UserInfo<TCustomData>>();
            }
        }

        /// <summary>
        /// Получить информацию о пользователе
        /// </summary>
        /// <typeparam name="TCustomData">Тип пользовательских данных</typeparam>
        /// <param name="username">Имя пользователя</param>
        /// <returns>Информация о пользователе</returns>
        public UserInfo<TCustomData>? GetUser<TCustomData>(string username) where TCustomData : class
        {
            try
            {
                var result = _context.Database.RunCommand<BsonDocument>(UsersInfo(username));

                var user = result["users"].AsBsonArray[0];

                var userInfo = GetUserInfo<TCustomData>(user.AsBsonDocument);

                return userInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Получить информацию о текущем пользователе
        /// </summary>
        /// <typeparam name="TCustomData">Тип пользовательских данных</typeparam>
        /// <returns>Информация о текущем пользователе</returns>
        public UserInfo<TCustomData>? GetCurrentUser<TCustomData>() where TCustomData : class
        {
            var username = _context.Database.Client.Settings.Credential.Username;

            return GetUser<TCustomData>(username);
        }


        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <param name="roles">Роли пользователя</param>
        /// <returns><c>true</c> - пользователь добавлен успешно</returns>
        public async Task CreateAsync(string username, string password, IEnumerable<Role> roles)
        {
            try
            {
                await _context.Database.RunCommandAsync<BsonDocument>(CreateUser(username, password, roles));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <typeparam name="TCustomData">Тип пользовательских данных</typeparam>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <param name="roles">Роли пользователя</param>
        /// <param name="customData">Пользовательские данные</param>
        /// <returns><c>true</c> - пользователь добавлен успешно</returns>
        public async Task CreateAsync<TCustomData>(string username, string password, IEnumerable<Role> roles, TCustomData customData)
        {
            try
            {
                await _context.Database.RunCommandAsync<BsonDocument>(CreateUser(username, password, roles, customData));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Удаление пользователя
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <returns><c>true</c> - пользователь удалён успешно</returns>
        public async Task DropAsync(string username)
        {
            try
            {
                await _context.Database.RunCommandAsync<BsonDocument>(DropUser(username));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Добавление ролей пользователю
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="roles">Словарь, где key - название роли из статического класса DbUserRoles,
        /// value - название базы данных, где находится выбранная роль, либо null или пустая строка,
        /// если роль существует в той же базе данных</param>
        /// <returns><c>true</c> - роли добавлены успешно</returns>
        public async Task GrantRolesAsync(string username, IEnumerable<Role> roles)
        {
            try
            {
                await _context.Database.RunCommandAsync<BsonDocument>(GrantRolesToUser(username, roles));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Удаление ролей пользователя
        /// </summary>
        /// <param name="username">ИМя пользователя</param>
        /// <param name="roles">Удаляемые роли</param>
        /// <returns><c>true</c> - роли удалены успешно</returns>
        public async Task RevokeRolesAsync(string username, IEnumerable<Role> roles)
        {
            try
            {
                await _context.Database.RunCommandAsync<BsonDocument>(RevokeRolesFromUser(username, roles));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="username">Имя пользователя</param>
        /// <param name="newPassword">Пароль пользователя</param>
        /// <returns><c>true</c> - пароль изменён успешно</returns>
        public async Task ChangePasswordAsync(string username, string newPassword)
        {
            try
            {
                await _context.Database.RunCommandAsync<BsonDocument>(UpdateUserPassword(username, newPassword));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Изменение пользовательских данных
        /// </summary>
        /// <typeparam name="TCustomData">Тип пользовательских данных</typeparam>
        /// <param name="username">Имя пользователя</param>
        /// <param name="customData">Пользовательские данные</param>
        /// <returns><c>true</c> - пользовательские данные изменены успешно</returns>
        public async Task UpdateCustomDataAsync<TCustomData>(string username, TCustomData customData)
        {
            try
            {
                await _context.Database.RunCommandAsync<BsonDocument>(UpdateUserCustomData(username, customData));
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить информацию о пользователях текущей базы данных
        /// </summary>
        /// <typeparam name="TCustomData">Тип пользовательских данных</typeparam>
        /// <returns>Список с информацией о пользователях</returns>
        public async Task<IEnumerable<UserInfo<TCustomData>>> ListAsync<TCustomData>() where TCustomData : class
        {
            try
            {
                var result = await _context.Database.RunCommandAsync<BsonDocument>(UsersInfo());

                return result["users"].AsBsonArray.Select(user => GetUserInfo<TCustomData>(user.AsBsonDocument));
            }
            catch (Exception)
            {
                return new List<UserInfo<TCustomData>>();
            }
        }

        /// <summary>
        /// Получить информацию о пользователе
        /// </summary>
        /// <typeparam name="TCustomData">Тип пользовательских данных</typeparam>
        /// <param name="username">Имя пользователя</param>
        /// <returns>Информация о пользователе</returns>
        public async Task<UserInfo<TCustomData>?> GetUserAsync<TCustomData>(string username) where TCustomData : class
        {
            try
            {
                var result = await _context.Database.RunCommandAsync<BsonDocument>(UsersInfo(username));

                var user = result["users"].AsBsonArray[0];

                var userInfo = GetUserInfo<TCustomData>(user.AsBsonDocument);

                return userInfo;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Получить информацию о текущем пользователе
        /// </summary>
        /// <typeparam name="TCustomData">Тип пользовательских данных</typeparam>
        /// <returns>Информация о текущем пользователе</returns>
        public async Task<UserInfo<TCustomData>?> GetCurrentUserAsync<TCustomData>() where TCustomData : class
        {
            var username = _context.Database.Client.Settings.Credential.Username;

            return await GetUserAsync<TCustomData>(username);
        }
    }
}