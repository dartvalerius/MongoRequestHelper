using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.GridFS;

namespace MongoRequestHelper.Models
{
    /// <summary>
    /// Базовый класс информации о файле
    /// </summary>
    public class FileInfoBase
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Размер файла
        /// </summary>
        public long Length { get; }

        /// <summary>
        /// Дата загрузки в БД
        /// </summary>
        public DateTime UploadDate { get; }

        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; }

        internal FileInfoBase(string id, long length, DateTime uploadDate, string fileName)
        {
            Id = id;
            Length = length;
            UploadDate = uploadDate;
            FileName = fileName;
        }

        internal FileInfoBase(GridFSFileInfo fileInfo)
        {
            Id = fileInfo.Id.ToString();
            Length = fileInfo.Length;
            UploadDate = fileInfo.UploadDateTime.ToLocalTime();
            FileName = fileInfo.Filename;
        }
    }

    /// <summary>
    /// Класс данных с информацией о файле
    /// </summary>
    internal class FileInfo : FileInfoBase
    {
        /// <summary>
        /// Дополнительные метаданные
        /// </summary>
        public BsonDocument MetaData { get; }

        internal FileInfo(GridFSFileInfo fileInfo) : base(fileInfo)
        {
            MetaData = fileInfo.BackingDocument;
        }
    }

    /// <summary>
    /// Класс данных с информацией о файле с определённым типом метаданных
    /// </summary>
    /// <typeparam name="TMetaData">Тип метаданных</typeparam>
    public class FileInfo<TMetaData> : FileInfoBase
    {
        /// <summary>
        /// Дополнительные метаданные определённого типа
        /// </summary>
        public TMetaData MetaData { get; }

        internal FileInfo(FileInfo fileInfo) : base(fileInfo.Id, fileInfo.Length, fileInfo.UploadDate, fileInfo.FileName)
        {
            MetaData = BsonSerializer.Deserialize<TMetaData>(fileInfo.MetaData);
        }
    }
}