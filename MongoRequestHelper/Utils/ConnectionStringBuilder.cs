using System;
using System.Linq;
using MongoDB.Driver;
using MongoRequestHelper.Exceptions;
using MongoRequestHelper.Resources;

namespace MongoRequestHelper.Utils
{
    public class ConnectionStringBuilder
    {
        private readonly MongoUrlBuilder _urlBuilder;

        public ConnectionStringBuilder()
        {
            _urlBuilder = new MongoUrlBuilder();
        }

        public string ApplicationName
        {
            get => _urlBuilder.ApplicationName;
            set => _urlBuilder.ApplicationName = value;
        }

        /// <summary>
        /// Механизм аутентификации
        /// </summary>
        public string AuthenticationMechanism
        {
            get => _urlBuilder.AuthenticationMechanism;
            set => _urlBuilder.AuthenticationMechanism = value;
        }

        /// <summary>
        /// Источник аутентификации
        /// </summary>
        public string AuthenticationSource
        {
            get => _urlBuilder.AuthenticationSource;
            set => _urlBuilder.AuthenticationSource = value;
        }

        /// <summary>
        /// Время ожидания подключения
        /// </summary>
        public TimeSpan ConnectTimeout
        {
            get => _urlBuilder.ConnectTimeout;
            set => _urlBuilder.ConnectTimeout = value;
        }

        /// <summary>
        /// Имя базы данных (необязательный параметр)
        /// </summary>
        public string DatabaseName
        {
            get => _urlBuilder.DatabaseName;
            set => _urlBuilder.DatabaseName = value;
        }

        /// <summary>
        /// Прямое подключение
        /// </summary>
        public bool DirectConnection
        {
            get => _urlBuilder.DirectConnection ?? false;
            set => _urlBuilder.DirectConnection = value;
        }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password
        {
            get => _urlBuilder.Password;
            set => _urlBuilder.Password = value;
        }

        /// <summary>
        /// Наименование реплики
        /// </summary>
        public string ReplicaSetName
        {
            get => _urlBuilder.ReplicaSetName;
            set => _urlBuilder.ReplicaSetName = value;
        }

        /// <summary>
        /// Сервер
        /// <example>host:port</example>
        /// </summary>
        public string Server
        {
            get => $"{_urlBuilder.Server.Host}:{_urlBuilder.Server.Port}";
            set => _urlBuilder.Server = GetMongoServerAddress(value);
        }

        /// <summary>
        /// Серверы
        /// <example>host1:port1,host2:port2,...,hostN:portN</example>
        /// </summary>
        public string Servers
        {
            get => string.Join(",", _urlBuilder.Servers.Select(s => $"{s.Host}:{s.Port}"));
            set => _urlBuilder.Servers = value.Split(',').Select(GetMongoServerAddress);
        }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName
        {
            get => _urlBuilder.Username;
            set => _urlBuilder.Username = value;
        }

        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString => Url.ToString();

        /// <summary>
        /// Url строки подключения
        /// </summary>
        internal MongoUrl Url => _urlBuilder.ToMongoUrl();

        /// <summary>
        /// Получение объекта сервера
        /// </summary>
        /// <param name="serverAddress">Адрес сервера</param>
        /// <returns></returns>
        /// <exception cref="MongoServerAddressInvalidException">Неверный формат адреса</exception>
        /// <exception cref="MongoServerPortInvalidException">Неверный формат порта</exception>
        private MongoServerAddress GetMongoServerAddress(string serverAddress)
        {
            var addressParts = serverAddress.Split(':');

            if (addressParts.Length != 2)
                throw new MongoServerAddressInvalidException(ExceptionMessages.ServerAddressInvalid + $" {serverAddress}");

            if (!int.TryParse(addressParts[1], out var port))
                throw new MongoServerPortInvalidException(ExceptionMessages.ServerPortInvalid + $" {addressParts[1]}");

            return new MongoServerAddress(addressParts[0], port);
        }
    }
}