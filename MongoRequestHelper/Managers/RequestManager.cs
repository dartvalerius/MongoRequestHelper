using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoRequestHelper.Exceptions;
using MongoRequestHelper.Utils;

namespace MongoRequestHelper.Managers
{
    /// <summary>
    /// Менеджер запросов
    /// </summary>
    /// <typeparam name="TDocument">Тип данных требуемой коллекции</typeparam>
    public class RequestManager<TDocument> 
        where TDocument : class
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="context">Экземпляр класса унаследованного от MongoContext</param>
        public RequestManager(MongoContext context)
        {
            _collection = context.Database
                .GetCollection<TDocument>(Helper.GetCollectionName(typeof(TDocument)));
        }

        public RequestManager(MongoContext context, string collectionName) : this(context)
        {
            _collection = context.Database.GetCollection<TDocument>(collectionName);
        }

        #region PRIVATE FIELDS

        /// <summary>
        /// Экземпляр коллекции
        /// </summary>
        private readonly IMongoCollection<TDocument> _collection;

        #endregion

        #region PUBLIC METHODS SYNC

        /// <summary>
        /// Получить источник данных
        /// </summary>
        /// <returns>Источник данных</returns>
        public IQueryable<TDocument> GetQueryable() => _collection.AsQueryable();

        /// <summary>
        /// Получить список объектов
        /// </summary>
        /// <param name="predicate">Предикат для фильтрации объектов</param>
        /// <returns>Список полученных объектов</returns>
        public IEnumerable<TDocument> List(Expression<Func<TDocument, bool>> predicate)
        {
            try
            {
                return _collection
                    .Find(predicate)
                    .ToEnumerable();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить список объектов проекции
        /// </summary>
        /// <typeparam name="TProjection">Тип на который проецируется основной тип</typeparam>
        /// <param name="predicate">Предикат для фильтрации объектов</param>
        /// <param name="projection">Метод формирования объекта проекции</param>
        /// <returns>Список полученных объектов проекции</returns>
        public IEnumerable<TProjection> List<TProjection>(Expression<Func<TDocument, bool>> predicate,
            Expression<Func<TDocument, TProjection>> projection)
        {
            try
            {
                return _collection
                    .Find(predicate)
                    .Project(projection)
                    .ToEnumerable();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить первый объект соответствующий предикату
        /// </summary>
        /// <param name="predicate">Предикат фильтрации</param>
        /// <returns>Полученный объект</returns>
        public TDocument Get(Expression<Func<TDocument, bool>> predicate)
        {
            try
            {
                return _collection
                    .Find(predicate)
                    .FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить первый объект проекции соответствующий предикату
        /// </summary>
        /// <param name="predicate">Предикат фильтрации</param>
        /// <param name="projection">Метод формирования объекта проекции</param>
        /// <returns>Полученный объект проекции</returns>
        public TProjection Get<TProjection>(Expression<Func<TDocument, bool>> predicate,
            Expression<Func<TDocument, TProjection>> projection)
        {
            try
            {
                return _collection
                    .Find(predicate)
                    .Project(projection)
                    .FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Вставить новый объект в БД
        /// </summary>
        /// <param name="document">Экземпляр объекта</param>
        /// <param name="session">Экземпляр сессии</param>
        public void Insert(TDocument document, Session? session = null)
        {
            try
            {
                if (session == null) _collection.InsertOne(document);
                else _collection.InsertOne(session.SessionHandle, document);
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Вставить несколько объектов в БД
        /// </summary>
        /// <param name="documents">Список экземпляров объекта</param>
        /// <param name="session">Экземпляр сессии</param>
        public void Insert(IEnumerable<TDocument> documents, Session? session = null)
        {
            try
            {
                var mongoDocuments = documents.ToList();

                if (session == null) _collection.InsertMany(mongoDocuments);
                else _collection.InsertMany(session.SessionHandle, mongoDocuments);
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Заменить данные первого объекта который соответствует предикату
        /// </summary>
        /// <param name="predicate">Предикат для фильтрации объектов</param>
        /// <param name="document">Экземпляр объекта</param>
        /// <param name="isUpsert">Вставлять документ если он не существует</param>
        /// <param name="session">Экземпляр сессии</param>
        public TDocument Replace(Expression<Func<TDocument, bool>> predicate, TDocument document, bool isUpsert = false, Session? session = null)
        {
            try
            {
                var options = new FindOneAndReplaceOptions<TDocument>
                {
                    IsUpsert = isUpsert,
                };

                var result = session == null
                    ? _collection.FindOneAndReplace(predicate, document, options)
                    : _collection.FindOneAndReplace(session.SessionHandle, predicate, document, options);

                return result;
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Удалить данные объектов соответствующих предикату
        /// </summary>
        /// <param name="predicate">Предикат фильтрации</param>
        /// <param name="session"></param>
        public void Delete(Expression<Func<TDocument, bool>> predicate, Session? session = null)
        {
            try
            {
                if (session == null) _collection.DeleteMany(predicate);
                else _collection.DeleteMany(session.SessionHandle, predicate);
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить количество объектов соответствующие переданному предикату
        /// </summary>
        /// <param name="predicate">Предикат фильтрации объектов</param>
        /// <returns>Количество объектов</returns>
        public long Count(Expression<Func<TDocument, bool>> predicate)
        {
            try
            {
                return _collection
                    .Find(predicate)
                    .CountDocuments();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить страницу объектов
        /// </summary>
        /// <param name="predicate">Предикат фильтрации объектов</param>
        /// <param name="limit">Количество получаемых объектов</param>
        /// <param name="skip">Количество пропускаемых объектов</param>
        /// <returns>Список объектов на странице</returns>
        public IEnumerable<TDocument> Page(Expression<Func<TDocument, bool>> predicate, int limit = 0, int skip = 0)
        {
            try
            {
                return _collection
                    .Find(predicate)
                    .Limit(limit)
                    .Skip(skip)
                    .ToEnumerable();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить страницу объектов проекции
        /// </summary>
        /// <param name="predicate">Предикат фильтрации объектов</param>
        /// <param name="projection">Метод формирования объекта проекции</param>
        /// <param name="limit">Количество получаемых объектов</param>
        /// <param name="skip">Количество пропускаемых объектов</param>
        /// <returns>Список объектов проекции на странице</returns>
        public IEnumerable<TProjection> Page<TProjection>(Expression<Func<TDocument, bool>> predicate,
            Expression<Func<TDocument, TProjection>> projection, int limit = 0, int skip = 0)
        {
            try
            {
                return _collection
                    .Find(predicate)
                    .Project(projection)
                    .Limit(limit)
                    .Skip(skip)
                    .ToEnumerable();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        #endregion

        #region PUBLIC METHODS ASYNC

        /// <summary>
        /// Получить список объектов
        /// </summary>
        /// <param name="predicate">Предикат для фильтрации объектов</param>
        /// <returns>Список полученных объектов</returns>
        public async Task<IEnumerable<TDocument>> ListAsync(Expression<Func<TDocument, bool>> predicate)
        {
            try
            {
                return await _collection
                    .Find(predicate)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить список объектов проекции
        /// </summary>
        /// <param name="predicate">Предикат для фильтрации объектов</param>
        /// <param name="projection">Метод формирования объекта проекции</param>
        /// <returns>Список полученных объектов проекции</returns>
        public async Task<IEnumerable<TProjection>> ListAsync<TProjection>(Expression<Func<TDocument, bool>> predicate,
            Expression<Func<TDocument, TProjection>> projection)
        {
            try
            {
                return await _collection
                    .Find(predicate)
                    .Project(projection)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить первый объект соответствующий предикату
        /// </summary>
        /// <param name="predicate">Предикат фильтрации</param>
        /// <returns>Полученный объект</returns>
        public async Task<TDocument> GetAsync(Expression<Func<TDocument, bool>> predicate)
        {
            try
            {
                return await _collection
                    .Find(predicate)
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить первый объект проекции соответствующий предикату
        /// </summary>
        /// <param name="predicate">Предикат фильтрации</param>
        /// <param name="projection">Метод формирования объекта проекции</param>
        /// <returns>Полученный объект проекции</returns>
        public async Task<TProjection> GetAsync<TProjection>(Expression<Func<TDocument, bool>> predicate,
            Expression<Func<TDocument, TProjection>> projection)
        {
            try
            {
                return await _collection
                    .Find(predicate)
                    .Project(projection)
                    .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Вставить новый объект в БД
        /// </summary>
        /// <param name="document">Экземпляр объекта</param>
        /// <param name="session">Экземпляр сессии</param>
        public async Task InsertAsync(TDocument document, Session? session = null)
        {
            try
            {
                if (session == null) await _collection.InsertOneAsync(document);
                else await _collection.InsertOneAsync(session.SessionHandle, document);
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Вставить несколько объектов в БД
        /// </summary>
        /// <param name="documents">Список экземпляров объекта</param>
        /// <param name="session">Экземпляр сессии</param>
        public async Task InsertAsync(IEnumerable<TDocument> documents, Session? session = null)
        {
            try
            {
                var mongoDocuments = documents.ToList();

                if (session == null) await _collection.InsertManyAsync(mongoDocuments);
                else await _collection.InsertManyAsync(session.SessionHandle, mongoDocuments);
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Заменить данные первого объекта который соответствует предикату
        /// </summary>
        /// <param name="predicate">Предикат для фильтрации объектов</param>
        /// <param name="document">Экземпляр объекта</param>
        /// <param name="isUpsert">Вставлять документ если он не существует</param>
        /// <param name="session">Экземпляр сессии</param>
        public async Task<TDocument> ReplaceAsync(Expression<Func<TDocument, bool>> predicate, TDocument document, bool isUpsert = false, Session? session = null)
        {
            try
            {
                var options = new FindOneAndReplaceOptions<TDocument>
                {
                    IsUpsert = isUpsert,
                };

                var result = session == null
                    ? await _collection.FindOneAndReplaceAsync(predicate, document, options)
                    : await _collection.FindOneAndReplaceAsync(session.SessionHandle, predicate, document, options);

                return result;
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Удалить данные объектов соответствующих предикату
        /// </summary>
        /// <param name="predicate">Предикат фильтрации</param>
        /// <param name="session">Экземпляр сессии</param>
        public async Task DeleteAsync(Expression<Func<TDocument, bool>> predicate, Session? session = null)
        {
            try
            {
                if(session == null) await _collection.DeleteManyAsync(predicate);
                else await _collection.DeleteManyAsync(session.SessionHandle, predicate);
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить количество объектов соответствующие переданному предикату
        /// </summary>
        /// <param name="predicate">Предикат фильтрации объектов</param>
        /// <returns>Количество объектов</returns>
        public async Task<long> CountAsync(Expression<Func<TDocument, bool>> predicate)
        {
            try
            {
                return await _collection
                    .Find(predicate)
                    .CountDocumentsAsync();
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить страницу объектов
        /// </summary>
        /// <param name="predicate">Предикат фильтрации объектов</param>
        /// <param name="limit">Количество получаемых объектов</param>
        /// <param name="skip">Количество пропускаемых объектов</param>
        /// <returns>Список объектов на странице</returns>
        public async Task<IEnumerable<TDocument>> PageAsync(Expression<Func<TDocument, bool>> predicate, int limit = 0, int skip = 0)
        {
            try
            {
                return await Task.Run(() => _collection
                    .Find(predicate)
                    .Limit(limit)
                    .Skip(skip)
                    .ToEnumerable());
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Получить страницу объектов
        /// </summary>
        /// <param name="predicate">Предикат фильтрации объектов</param>
        /// <param name="projection">Метод формирования объекта проекции</param>
        /// <param name="limit">Количество получаемых объектов</param>
        /// <param name="skip">Количество пропускаемых объектов</param>
        /// <returns>Список объектов проекции на странице</returns>
        public async Task<IEnumerable<TProjection>> PageAsync<TProjection>(Expression<Func<TDocument, bool>> predicate,
            Expression<Func<TDocument, TProjection>> projection, int limit = 0, int skip = 0)
        {
            try
            {
                return await Task.Run(() => _collection
                    .Find(predicate)
                    .Project(projection)
                    .Limit(limit)
                    .Skip(skip)
                    .ToEnumerable());
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        #endregion
    }
}