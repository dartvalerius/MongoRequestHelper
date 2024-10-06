using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoRequestHelper.Exceptions;
using MongoRequestHelper.Resources;

namespace MongoRequestHelper.Serialization
{
    /// <summary>
    /// Карта элемента класса
    /// </summary>
    public class MongoMemberMap
    {
        private readonly BsonMemberMap _memberMap;


        /// <summary>
        /// Значение по умолчанию
        /// </summary>
        public object DefaultValue => _memberMap.DefaultValue;

        /// <summary>
        /// Имя элемента
        /// </summary>
        public string ElementName => _memberMap.ElementName;

        /// <summary>
        /// Тип генератора идентификатора
        /// </summary>
        public IdGeneratorType IdGenerator
        {
            get
            {
                switch (_memberMap.IdGenerator)
                {
                    case GuidGenerator:
                        return IdGeneratorType.GuidId;
                    case ObjectIdGenerator:
                        return IdGeneratorType.ObjectId;
                    case StringObjectIdGenerator:
                        return IdGeneratorType.StringObjectId;
                    default:
                        return IdGeneratorType.ObjectId;
                }
            }
        }

        /// <summary>
        /// Игнорирование элемента если имеет значение по умолчанию
        /// </summary>
        public bool IgnoreIfDefault => _memberMap.IgnoreIfDefault;
        
        /// <summary>
        /// Игнорирование элемента если имеет значение null
        /// </summary>
        public bool IgnoreIfNull => _memberMap.IgnoreIfNull;

        /// <summary>
        /// Обязательный элемент
        /// </summary>
        public bool IsRequired => _memberMap.IsRequired;

        /// <summary>
        /// Информация об элементе
        /// </summary>
        public MemberInfo MemberInfo => _memberMap.MemberInfo;

        /// <summary>
        /// Имя элемента класса
        /// </summary>
        public string MemberName => _memberMap.MemberName;

        /// <summary>
        /// Тип элемента класса
        /// </summary>
        public Type MemberType => _memberMap.MemberType;

        /// <summary>
        /// Номер по порядку в документе
        /// </summary>
        public int Order => _memberMap.Order;


        internal MongoMemberMap(BsonMemberMap memberMap)
        {
            _memberMap = memberMap;
        }


        /// <summary>
        /// Установить значение по умолчанию с помощью переданного генератора значений.
        /// Применяется только при чтении документа из базы данных, в котором нет значения
        /// для соответствующего поля. Если вы хотите, чтобы новые объекты, которые вы
        /// создаете в памяти, имели значение, отличное от null, вам необходимо установить
        /// это значение в своем конструкторе. Кроме того, когда вы сохраняете объект в базе
        /// данных, если поле имеет значение null, то это то, что сохраняется и считывается обратно.
        /// </summary>
        /// <param name="defaultValueCreator">Метод генерирующий значение по умолчанию.</param>
        /// <returns>Карта элемента класса</returns>
        public MongoMemberMap SetDefaultValue(Func<object> defaultValueCreator)
        {
            _memberMap.SetDefaultValue(defaultValueCreator);

            return this;
        }

        /// <summary>
        /// Установить значение по умолчанию
        /// Применяется только при чтении документа из базы данных, в котором нет значения
        /// для соответствующего поля. Если вы хотите, чтобы новые объекты, которые вы
        /// создаете в памяти, имели значение, отличное от null, вам необходимо установить
        /// это значение в своем конструкторе. Кроме того, когда вы сохраняете объект в базе
        /// данных, если поле имеет значение null, то это то, что сохраняется и считывается обратно.
        /// </summary>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Карта элемента класса</returns>
        public MongoMemberMap SetDefaultValue(object defaultValue)
        {
            _memberMap.SetDefaultValue(defaultValue);

            return this;
        }

        /// <summary>
        /// Установить имя элемента класса
        /// </summary>
        /// <param name="elementName">Имя элемента</param>
        /// <returns>Карта элемента класса</returns>
        public MongoMemberMap SetElementName(string elementName)
        {
            _memberMap.SetElementName(elementName);

            return this;
        }

        /// <summary>
        /// Установить генератор идентификатора
        /// </summary>
        /// <param name="idGeneratorType">Тип генератора</param>
        /// <returns>Карта элемента класса</returns>
        public MongoMemberMap SetIdGenerator(IdGeneratorType idGeneratorType)
        {
            switch (idGeneratorType)
            {
                case IdGeneratorType.GuidId:
                    {
                        _memberMap.SetIdGenerator(GuidGenerator.Instance);
                        break;
                    }
                case IdGeneratorType.ObjectId:
                    {
                        _memberMap.SetIdGenerator(ObjectIdGenerator.Instance);
                        break;
                    }
                case IdGeneratorType.StringObjectId:
                    {
                        _memberMap.SetIdGenerator(StringObjectIdGenerator.Instance);
                        break;
                    }
                default:
                    {
                        _memberMap.SetIdGenerator(ObjectIdGenerator.Instance);
                        break;
                    }
            }

            return this;
        }

        /// <summary>
        /// Не отображать элемент в документе БД если элемент имеет значение по умолчанию
        /// </summary>
        /// <returns>Карта элемента класса</returns>
        public MongoMemberMap SetIgnoreIfDefault()
        {
            _memberMap.SetIgnoreIfDefault(true);

            return this;
        }

        /// <summary>
        /// Не отображать элемент в документе БД если элемент имеет значение null
        /// </summary>
        /// <returns>Карта элемента класса</returns>
        public MongoMemberMap SetIgnoreIfNull()
        {
            _memberMap.SetIgnoreIfNull(true);

            return this;
        }

        /// <summary>
        /// Установить, что элемент класса является обязательным
        /// </summary>
        /// <returns>Карта элемента класса</returns>
        public MongoMemberMap SetIsRequired()
        {
            _memberMap.SetIsRequired(true);

            return this;
        }

        /// <summary>
        /// Задать порядок элемента в документе
        /// </summary>
        /// <param name="order">Порядковый номер</param>
        /// <returns>Карта элемента класса</returns>
        public MongoMemberMap SetOrder(int order)
        {
            _memberMap.SetOrder(order);

            return this;
        }

        /// <summary>
        /// Установка даты в формате UTC
        /// </summary>
        /// <returns>Карта элемента класса</returns>
        /// <exception cref="MongoInvalidMemberTypeException">Исключение неверного типа выбранного элемента</exception>
        public MongoMemberMap SetDateTimeUtc()
        {
            if (_memberMap.MemberType == typeof(DateTime))
                _memberMap.SetSerializer(new DateTimeSerializer(DateTimeKind.Utc));
            else throw new MongoInvalidMemberTypeException(ExceptionMessages.InvalidMemberType + " DateTime");

            return this;
        }

        /// <summary>
        /// Установка даты в локализированном формате
        /// </summary>
        /// <returns>Карта элемента класса</returns>
        /// <exception cref="MongoInvalidMemberTypeException">Исключение неверного типа выбранного элемента</exception>
        public MongoMemberMap SetDateTimeLocal()
        {
            if (_memberMap.MemberType == typeof(DateTime))
                _memberMap.SetSerializer(new DateTimeSerializer(DateTimeKind.Local));
            else throw new MongoInvalidMemberTypeException(ExceptionMessages.InvalidMemberType + " DateTime");

            return this;
        }

        /// <summary>
        /// Установить строку в тип идентификатора базы данных
        /// </summary>
        /// <returns>Карта элемента класса</returns>
        /// <exception cref="MongoInvalidMemberTypeException">Исключение неверного типа выбранного элемента</exception>
        public MongoMemberMap SetStringToObjectId()
        {
            if (_memberMap.MemberType == typeof(string))
                _memberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));
            else throw new MongoInvalidMemberTypeException(ExceptionMessages.InvalidMemberType + " string");

            return this;
        }

        /// <summary>
        /// Преобразует тип GUID в строковый тип базы
        /// </summary>
        /// <returns>Карта элемента класса</returns>
        /// <exception cref="MongoInvalidMemberTypeException">Исключение неверного типа выбранного элемента</exception>
        public MongoMemberMap SetGuidToString()
        {
            if (_memberMap.MemberType == typeof(Guid))
                _memberMap.SetSerializer(new GuidSerializer(BsonType.String));
            else throw new MongoInvalidMemberTypeException(ExceptionMessages.InvalidMemberType + " Guid");

            return this;
        }

        /// <summary>
        /// Установить элемент перечисления в строковый тип базы
        /// </summary>
        /// <typeparam name="TEnum">Тип перечисления</typeparam>
        /// <returns>Карта элемента класса</returns>
        /// <exception cref="MongoInvalidMemberTypeException">Исключение неверного типа выбранного элемента</exception>
        public MongoMemberMap SetEnumToString<TEnum>() where TEnum: struct, Enum
        {
            if (_memberMap.MemberType.IsEnum)
                _memberMap.SetSerializer(new EnumSerializer<TEnum>(BsonType.String));
            else throw new MongoInvalidMemberTypeException(ExceptionMessages.InvalidMemberType + " Enum");

            return this;
        }

        /// <summary>
        /// Установить список строк в список идентификаторов базы
        /// </summary>
        /// <returns>Карта элемента класса</returns>
        /// <exception cref="MongoInvalidMemberTypeException">Исключение неверного типа выбранного элемента</exception>
        public MongoMemberMap SetListStringToListObjectId()
        {
            if (_memberMap.MemberType == typeof(List<string>))
                _memberMap.SetSerializer(
                    new EnumerableInterfaceImplementerSerializer<List<string>>(
                        new StringSerializer(BsonType.ObjectId)));
            else throw new MongoInvalidMemberTypeException(ExceptionMessages.InvalidMemberType + " List<string>");

            return this;
        }

        /// <summary>
        /// Установить список Guid в список сток базы
        /// </summary>
        /// <returns>Карта элемента класса</returns>
        /// <exception cref="MongoInvalidMemberTypeException">Исключение неверного типа выбранного элемента</exception>
        public MongoMemberMap SetListGuidToListString()
        {
            if (_memberMap.MemberType == typeof(List<Guid>))
                _memberMap.SetSerializer(
                    new EnumerableInterfaceImplementerSerializer<List<Guid>>(
                        new GuidSerializer(BsonType.String)));
            else throw new MongoInvalidMemberTypeException(ExceptionMessages.InvalidMemberType + " List<Guid>");

            return this;
        }
    }
}