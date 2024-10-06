using System;
using System.Linq.Expressions;
using MongoDB.Bson.Serialization;
using MongoRequestHelper.Exceptions;
using MongoRequestHelper.Utils;

namespace MongoRequestHelper.Serialization
{
    /// <summary>
    /// Составления карты класса на базу данных
    /// </summary>
    public class MongoClassMap<TDocument>
    {
        private readonly BsonClassMap<TDocument> _classMap;


        /// <summary>
        /// Получить тип класса
        /// </summary>
        public Type ClassType => _classMap.ClassType;

        /// <summary>
        /// Получить идентификатор карты класса (null если нет)
        /// </summary>
        public MongoMemberMap? IdMemberMap =>
            _classMap.IdMemberMap == null ? null : new MongoMemberMap(_classMap.IdMemberMap);

        /// <summary>
        /// Игнорирование дополнительных элементов
        /// </summary>
        public bool IgnoreExtraElements => _classMap.IgnoreExtraElements;


        public MongoClassMap()
        {
            _classMap = new BsonClassMap<TDocument>();
        }


        /// <summary>
        /// Автоматическое сопоставление
        /// </summary>
        public void AutoMap() => _classMap.AutoMap();

        /// <summary>
        /// Игнорировать дополнительные элементы в БД
        /// </summary>
        public void SetIgnoreExtraElements() => _classMap.SetIgnoreExtraElements(true);

        /// <summary>
        /// Создание карты для элемента-идентификатора класса
        /// </summary>
        /// <typeparam name="TMember">Тип элемента-идентификатора класса</typeparam>
        /// <param name="member">Предикат получения элемента-идентификатора</param>
        /// <returns>Карта элемента-идентификатора</returns>
        public MongoMemberMap MapIdMember<TMember>(Expression<Func<TDocument, TMember>> member) =>
            new(_classMap.MapIdMember(member));

        /// <summary>
        /// Создание карты для элемента класса
        /// </summary>
        /// <typeparam name="TMember">Тип элемента класса</typeparam>
        /// <param name="member">Предикат получения элемента класса</param>
        /// <returns>Карта элемента класса</returns>
        public MongoMemberMap MapMember<TMember>(Expression<Func<TDocument, TMember>> member) =>
            new(_classMap.MapMember(member));

        /// <summary>
        /// Получить элемент карты класса
        /// </summary>
        /// <typeparam name="TMember">Тип элемента класса</typeparam>
        /// <param name="member">Предикат получения элемента класса</param>
        /// <returns>Карта элемента класса</returns>
        public MongoMemberMap GetMemberMap<TMember>(Expression<Func<TDocument, TMember>> member)
        {
            return new MongoMemberMap(_classMap.GetMemberMap(member));
        }


        /// <summary>
        /// Регистрация карты класса
        /// </summary>
        /// <exception cref="MongoRegisterClassMapException"/>
        public void Registration()
        {
            try
            {
                if (!BsonClassMap.IsClassMapRegistered(typeof(TDocument)))
                    BsonClassMap.RegisterClassMap(_classMap);
            }
            catch (Exception e)
            {
                throw new MongoRegisterClassMapException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }
    }
}