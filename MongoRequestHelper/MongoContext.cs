using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Linq;
using MongoRequestHelper.EventArgs;
using MongoRequestHelper.Exceptions;
using MongoRequestHelper.Implementations;
using MongoRequestHelper.Interfaces;
using MongoRequestHelper.Managers;
using MongoRequestHelper.Utils;

namespace MongoRequestHelper
{
    /// <summary>
    /// Контекст подключения к БД
    /// </summary>
    public abstract class MongoContext
    {
        protected MongoContext(IOptions<IMongoSettings> dbOptions)
            : this(dbOptions.Value)
        {

        }

        protected MongoContext(IMongoSettings settings)
        {
            try
            {
                var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);

                clientSettings.LinqProvider = LinqProvider.V3;

                _client = new MongoClient(clientSettings);

                Database = _client.GetDatabase(settings.DatabaseName);

                CurrentUserName = _client.Settings.Credential?.Username;

                SetConnectionState();

                OnRegisterClassMap();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        protected MongoContext(ConnectionStringBuilder connectionStringBuilder)
            : this(new MongoSettings
            {
                ConnectionString = connectionStringBuilder.Url.ToString(),
                DatabaseName = connectionStringBuilder.DatabaseName
            })
        { }

        #region PRIVATE FIELDS

        /// <summary>
        /// Клиент подключения к серверу
        /// </summary>
        private readonly IMongoClient _client;
        
        #endregion

        #region PUBLIC PROPERTIES

        /// <summary>
        /// Текущая база данных
        /// </summary>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Состояние подключения
        /// </summary>
        public bool IsConnectionOpen { get; private set; }

        /// <summary>
        /// Имя текущего пользователя
        /// По умолчанию устанавливается из параметров аутентификации
        /// </summary>
        public string? CurrentUserName { get; set; }

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Установка состояния подключения
        /// </summary>
        private void SetConnectionState()
        {
            IsConnectionOpen = Ping();

            _client.Cluster.DescriptionChanged += Cluster_DescriptionChanged;
        }

        /// <summary>
        /// Обработчик события изменений в кластере
        /// </summary>
        private void Cluster_DescriptionChanged(object? sender, ClusterDescriptionChangedEventArgs e)
        {
            //Если вызывается конструктор ConnectionManager несколько раз до закрытия приложения, то лучше убрать,
            //т.к. подключение остаётся в драйвере
            if (e.NewClusterDescription.State == e.OldClusterDescription.State) return;

            IsConnectionOpen = e.NewClusterDescription.State != ClusterState.Disconnected;

            OnConnectionStateChanged();
        }

        /// <summary>
        /// Вызов события изменения состояния подключения
        /// </summary>
        private void OnConnectionStateChanged() => ConnectionStateChanged?.Invoke(this, new ConnectionStateChangeEventArgs(IsConnectionOpen));

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Проверка доступности сервера
        /// </summary>
        /// <returns><c>true</c> - если подключение установлено</returns>
        public bool Ping()
        {
            return Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);
        }

        /// <summary>
        /// Получить новую сессию
        /// </summary>
        /// <returns>Менеджер сессии</returns>
        public SessionManager GetSession()
        {
            return new SessionManager(new Session(_client.StartSession().Fork()));
        }

        /// <summary>
        /// Получить новую сессию
        /// </summary>
        /// <returns>Менеджер сессии</returns>
        public async Task<SessionManager> GetSessionAsync()
        {
            return new SessionManager(new Session(await _client.StartSessionAsync()));
        }

        #endregion

        #region VIRTUAL METHODS

        /// <summary>
        /// Регистрация карты классов
        /// </summary>
        protected virtual void OnRegisterClassMap()
        {

        }

        #endregion

        #region DELEGATES

        public delegate void ConnectionStateChangeHandler(object sender, ConnectionStateChangeEventArgs e);

        #endregion

        #region EVENTS

        /// <summary>
        /// Событие изменения состояния подключения
        /// </summary>
        public event ConnectionStateChangeHandler? ConnectionStateChanged;

        #endregion
    }
}