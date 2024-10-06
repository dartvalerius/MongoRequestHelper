using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoRequestHelper.Managers
{
    /// <summary>
    /// Менеджер сессий
    /// </summary>
    public class SessionManager : IDisposable
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="session">Экземпляр сессии</param>
        internal SessionManager(Session session)
        {
            Session = session;
        }

        /// <summary>
        /// Сессия
        /// </summary>
        public Session Session { get; }

        /// <summary>
        /// Начать транзакцию
        /// </summary>
        public void StartTransaction() => Session.SessionHandle.StartTransaction();

        /// <summary>
        /// Подтвердить транзакцию
        /// </summary>
        public void CommitTransaction() => Session.SessionHandle.CommitTransaction();

        /// <summary>
        /// Откатить транзакцию
        /// </summary>
        public void AbortTransaction() => Session.SessionHandle.AbortTransaction();

        /// <summary>
        /// Подтвердить транзакцию
        /// </summary>
        public async Task CommitTransactionAsync() => await Session.SessionHandle.CommitTransactionAsync();

        /// <summary>
        /// Откатить транзакцию
        /// </summary>
        public async Task AbortTransactionAsync() => await Session.SessionHandle.AbortTransactionAsync();

        public void Dispose()
        {
            Session.Dispose();
        }
    }

    /// <summary>
    /// Сессия
    /// </summary>
    public class Session : IDisposable
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sessionHandle">Экземпляр дескриптора сессии</param>
        internal Session(IClientSessionHandle sessionHandle)
        {
            SessionHandle = sessionHandle;
        }

        /// <summary>
        /// Дескриптор сессии
        /// </summary>
        internal IClientSessionHandle SessionHandle { get; set; }

        /// <summary>
        /// Идентификатор сессии
        /// </summary>
        public string SessionId => SessionHandle.ServerSession.Id.AsString;

        /// <summary>
        /// Флаг старта транзакции
        /// </summary>
        public bool IsStartTransaction => SessionHandle.IsInTransaction;

        public void Dispose()
        {
            SessionHandle.Dispose();
        }
    }
}