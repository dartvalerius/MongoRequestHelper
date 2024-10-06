using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoRequestHelper.EventArgs;
using MongoRequestHelper.Exceptions;
using MongoRequestHelper.Models;
using MongoRequestHelper.Resources;
using MongoRequestHelper.Utils;
using FileInfo = MongoRequestHelper.Models.FileInfo;

namespace MongoRequestHelper.Managers
{
    public class FileManager
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="context">Экземпляр класса унаследованного от MongoContext</param>
        public FileManager(MongoContext context)
        {
            _context = context;

            _cancellationToken = new CancellationTokenSource();
        }

        #region PRIVATE FIELDS

        /// <summary>
        /// Контекст данных
        /// </summary>
        private readonly MongoContext _context;

        /// <summary>
        /// Токен отмены
        /// </summary>
        private readonly CancellationTokenSource _cancellationToken;

        /// <summary>
        /// Поток работы с файлом
        /// </summary>
        private Stream? _stream;

        #endregion

        #region PRIVATE METHODS

        /// <summary>
        /// Вызов события начала работы с фалом
        /// </summary>
        private void OnStart(FileStartEventArgs e) => Start?.Invoke(this, e);

        /// <summary>
        /// Вызов события окончания работы с файлом
        /// </summary>
        private void OnCompleted(FileCompletedEventArgs e) => Completed?.Invoke(this, e);

        /// <summary>
        /// Вызов события возникновения исключения при работе с файлом
        /// </summary>
        private void OnCrash(FileCrashEventArgs e) => Crash?.Invoke(this, e);

        /// <summary>
        /// Событие отмены работы с файлом
        /// </summary>
        private void OnCancel(FileCancelEventArgs e) => Cancel?.Invoke(this, e);

        /// <summary>
        /// Вызов события изменения прогресса скачивания или загрузки
        /// </summary>
        private void OnProgress(FileProgressEventArgs e) => Progress?.Invoke(this, e);

        /// <summary>
        /// Получить объект GridFS для работы с файлом
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns>Объект GridFS</returns>
        private GridFSBucket GetBucked(string collectionName)
        {
            try
            {
                var bucketOptions = new GridFSBucketOptions { BucketName = collectionName };

                var gridFsBucket = new GridFSBucket(_context.Database, bucketOptions);

                return gridFsBucket;
            }
            catch (Exception e)
            {
                throw new MongoDatabaseException(string.Join("\n", Helper.GetAllErrorMessage(e)));
            }
        }

        /// <summary>
        /// Наблюдатель изменения прогресса операции
        /// </summary>
        /// <param name="ct">Сигнал отмены</param>
        /// <param name="length">Размер файла</param>
        private async Task ProgressWatcher(CancellationToken ct, long length)
        {
            await Task.Run(() =>
            {
                try
                {
                    var progress = 0;
                    while (progress < 100)
                    {
                        ct.ThrowIfCancellationRequested();

                        progress = (int)(100 * _stream?.Position ?? 0 / length);

                        OnProgress(new FileProgressEventArgs(Math.Round(100 * (double)(_stream?.Position ?? 0) / length, 10),
                            _stream?.Position ?? 0));
                    }
                }
                catch (OperationCanceledException)
                {
                    //ignore
                }
            }, ct);
        }

        /// <summary>
        /// Получение информации всех файлов
        /// </summary>
        /// <param name="collectionName">Название коллекции файлового хранилища</param>
        /// <returns>Коллекция с информацией о файлах</returns>
        private async Task<List<GridFSFileInfo>> ListAsync(string collectionName)
        {
            var gridFsBucket = GetBucked(collectionName);

            var files = await (await gridFsBucket.FindAsync(Builders<GridFSFileInfo>.Filter.Empty,
                cancellationToken: _cancellationToken.Token)).ToListAsync();

            return files;
        }

        /// <summary>
        /// Получение файлов по имени
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="collectionName">Название коллекции файлового хранилища</param>
        /// <returns>Коллекция с информацией о найденных файлах</returns>
        private async Task<List<GridFSFileInfo>> GetByNameAsync(string fileName, string collectionName)
        {
            var gridFsBucket = GetBucked(collectionName);

            var files = await (await gridFsBucket.FindAsync(
                    Builders<GridFSFileInfo>.Filter.Where(f => f.Filename == fileName || f.Filename.Contains(fileName)),
                    cancellationToken: _cancellationToken.Token))
                .ToListAsync();

            return files;
        }

        /// <summary>
        /// Получить информацию о файле по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="collectionName">Название коллекции файлового хранилища</param>
        /// <returns>Информация о файле</returns>
        private async Task<GridFSFileInfo> GetByIdAsync(string id, string collectionName)
        {
            var gridFsBucket = GetBucked(collectionName);

            if (!ObjectId.TryParse(id, out var objectId))
                throw new MongoIdFormatException(ExceptionMessages.IdFormatInvalid);

            var file = await (await gridFsBucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", objectId),
                    cancellationToken: _cancellationToken.Token))
                .SingleOrDefaultAsync();

            return file;
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Отмена выполнения процесса работы с файлом
        /// </summary>
        public void CancelOperation()
        {
            _cancellationToken.Cancel();
            _stream?.Close();
        }

        /// <summary>
        /// Переименовать файл в БД
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="newName">Новое имя</param>
        /// <param name="option">Настройки</param>
        public async Task RenameAsync(string id, string newName, FileManagerOption? option = null)
        {
            option ??= new FileManagerOption();

            try
            {
                if (!ObjectId.TryParse(id, out var fileId))
                    throw new MongoIdFormatException(ExceptionMessages.IdFormatInvalid);

                OnStart(new FileStartEventArgs());

                var gridFsBucket = GetBucked(option.CollectionName);

                await gridFsBucket.RenameAsync(fileId, newName, _cancellationToken.Token);

                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    OnCancel(new FileCancelEventArgs());
                }
                else
                {
                    OnCompleted(new FileCompletedEventArgs());
                }
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));
            }
        }

        /// <summary>
        /// Скачивание файла из базы
        /// </summary>
        /// <param name="id">Идентификатор файла в базе</param>
        /// <param name="destination">Поток скачивания файла</param>
        /// <param name="option">Настройки</param>
        public async Task DownloadAsync(string id, Stream destination, FileManagerOption? option = null)
        {
            option ??= new FileManagerOption();
            try
            {
                if (!ObjectId.TryParse(id, out var fileId))
                    throw new MongoIdFormatException(ExceptionMessages.IdFormatInvalid);

                OnStart(new FileStartEventArgs());

                _stream = destination;

                var gridFsBucket = GetBucked(option.CollectionName);

                var fileLength = (await gridFsBucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", fileId)))
                    .SingleOrDefault()?.Length ?? throw new MongoFileLengthException(ExceptionMessages.FileLengthUndefined);

                await Task.WhenAll(ProgressWatcher(_cancellationToken.Token, fileLength),
                    gridFsBucket.DownloadToStreamAsync(fileId, _stream,
                        cancellationToken: _cancellationToken.Token));

                OnCompleted(new FileCompletedEventArgs());
            }
            catch (OperationCanceledException)
            {
                OnCancel(new FileCancelEventArgs());
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));
                _stream?.Close();
            }
        }

        /// <summary>
        /// Скачивание файла из базы
        /// </summary>
        /// <param name="id">Идентификатор файла в базе</param>
        /// <param name="filePath">Путь сохранения скачанного файла</param>
        /// <param name="option">Настройки</param>
        public async Task DownloadAsync(string id, string filePath, FileManagerOption? option = null)
        {
            using var downloadFileStream = new FileStream(filePath, FileMode.Create,
                FileAccess.ReadWrite, FileShare.Read);

            await DownloadAsync(id, downloadFileStream, option);
        }

        /// <summary>
        /// Скачивание файла в массив байт
        /// </summary>
        /// <param name="id">Идентификатор файла в базе</param>
        /// <param name="option">Настройки</param>
        /// <returns>Массив байтов</returns>
        public async Task<byte[]> DownloadAsync(string id, FileManagerOption? option = null)
        {
            option ??= new FileManagerOption();

            try
            {
                if (!ObjectId.TryParse(id, out var fileId))
                    throw new MongoIdFormatException(ExceptionMessages.IdFormatInvalid);


                var gridFsBucket = GetBucked(option.CollectionName);

                var fileLength = (await gridFsBucket.FindAsync(Builders<GridFSFileInfo>.Filter.Eq("_id", fileId)))
                    .SingleOrDefault()?.Length ?? throw new MongoFileLengthException(ExceptionMessages.FileLengthUndefined);

                if (fileLength > int.MaxValue) throw new MongoFileLengthException(ExceptionMessages.FileLengthExceed);

                return await gridFsBucket.DownloadAsBytesAsync(fileId, cancellationToken: _cancellationToken.Token);
            }
            catch (OperationCanceledException)
            {
                OnCancel(new FileCancelEventArgs());
                return Array.Empty<byte>();
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Открыть поток скачивания файла
        /// </summary>
        /// <param name="id">Идентификатор файла в базе</param>
        /// <param name="option">Настройки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Файловый поток на чтение</returns>
        public async Task<Stream?> OpenStreamDownloadAsync(string id, FileManagerOption? option = null, CancellationToken cancellationToken = default)
        {
            option ??= new FileManagerOption();

            try
            {
                if (!ObjectId.TryParse(id, out var fileId))
                    throw new MongoIdFormatException(ExceptionMessages.IdFormatInvalid);

                var gridFsBucket = GetBucked(option.CollectionName);

                return await gridFsBucket.OpenDownloadStreamAsync(fileId, cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));
                return null;
            }
        }

        /// <summary>
        /// Загрузка файла в базу
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="source">Поток для загрузки в базу</param>
        /// <param name="option">Настройки</param>
        public async Task<string?> UploadAsync(string fileName,
            Stream source, FileManagerUploadOption? option = null)
        {
            option ??= new FileManagerUploadOption();

            if(!ObjectId.TryParse(option.PresetId, out var id))
                id = ObjectId.GenerateNewId(DateTime.Now);

            try
            {
                OnStart(new FileStartEventArgs());

                _stream = source;

                var gridFsBucket = GetBucked(option.CollectionName);

                await Task.WhenAll(ProgressWatcher(_cancellationToken.Token, _stream.Length),
                    gridFsBucket.UploadFromStreamAsync(id, fileName, _stream,
                        cancellationToken: _cancellationToken.Token));

                OnCompleted(new FileCompletedEventArgs());

                return id.ToString();
            }
            catch (OperationCanceledException)
            {
                OnCancel(new FileCancelEventArgs());

                return null;
            }
            catch (Exception e)
            {
                _stream?.Close();
                OnCrash(new FileCrashEventArgs(e));

                return null;
            }
        }

        /// <summary>
        /// Загрузка файла в базу
        /// </summary>
        /// <typeparam name="TMetaData">Тип метаданных</typeparam>
        /// <param name="fileName">Имя файла</param>
        /// <param name="source">Поток для загрузки в базу</param>
        /// <param name="metadata">Метаданные</param>
        /// <param name="option">Настройки</param>
        public async Task<string?> UploadAsync<TMetaData>(string fileName,
            Stream source, TMetaData metadata, FileManagerUploadOption? option = null)
        {
            option ??= new FileManagerUploadOption();

            if (!ObjectId.TryParse(option.PresetId, out var id))
                id = ObjectId.GenerateNewId(DateTime.Now);

            try
            {
                OnStart(new FileStartEventArgs());

                _stream = source;

                var gridFsBucket = GetBucked(option.CollectionName);

                var gridFsUploadOptions = new GridFSUploadOptions
                {
                    Metadata = metadata?.ToBsonDocument()
                };

                await Task.WhenAll(ProgressWatcher(_cancellationToken.Token, _stream.Length),
                    gridFsBucket.UploadFromStreamAsync(id, fileName, _stream, gridFsUploadOptions,
                        _cancellationToken.Token));

                OnCompleted(new FileCompletedEventArgs());

                return id.ToString();
            }
            catch (OperationCanceledException)
            {
                OnCancel(new FileCancelEventArgs());

                return null;
            }
            catch (Exception e)
            {
                _stream?.Close();
                OnCrash(new FileCrashEventArgs(e));

                return null;
            }
        }

        /// <summary>
        /// Загрузка файла в базу
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="option">Настройки</param>
        public async Task<string?> UploadAsync(string filePath, FileManagerUploadOption? option = null)
        {
            using var uploadFileStream = new FileStream(filePath, FileMode.Open,
                FileAccess.Read, FileShare.Read);

            return await UploadAsync(Path.GetFileName(filePath), source: uploadFileStream, option: option);
        }

        /// <summary>
        /// Загрузка файла в базу
        /// </summary>
        /// <typeparam name="TMetaData">Тип метаданных</typeparam>
        /// <param name="filePath">Путь к файлу</param>
        /// <param name="metadata">Тип метаданных</param>
        /// <param name="option">Настройки</param>
        public async Task<string?> UploadAsync<TMetaData>(string filePath, TMetaData metadata, FileManagerUploadOption? option = null)
        {
            using var uploadFileStream = new FileStream(filePath, FileMode.Open,
                FileAccess.Read, FileShare.Read);

            return await UploadAsync(Path.GetFileName(filePath), uploadFileStream, metadata, option);
        }

        /// <summary>
        /// Загрузка файла в базу
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="option">Настройки</param>
        /// <param name="cancellationToken">Токен отмены</param>
        public async Task<UploadStream?> OpenStreamUploadAsync(string fileName, FileManagerUploadOption? option = null, CancellationToken cancellationToken = default)
        {
            option ??= new FileManagerUploadOption();

            if (!ObjectId.TryParse(option.PresetId, out var id))
                id = ObjectId.GenerateNewId(DateTime.Now);

            try
            {
                var gridFsBucket = GetBucked(option.CollectionName);

                var uploadStream = await gridFsBucket.OpenUploadStreamAsync(id, fileName, cancellationToken: cancellationToken);

                return new UploadStream(uploadStream);
            }
            catch (Exception e)
            {
                _stream?.Close();
                OnCrash(new FileCrashEventArgs(e));

                return null;
            }
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        /// <param name="id">Идентификатор удаляемого файла</param>
        /// <param name="option">Настройки</param>
        public async Task DeleteAsync(string id, FileManagerOption? option = null)
        {
            option ??= new FileManagerUploadOption();

            try
            {
                if (!ObjectId.TryParse(id, out var fileId))
                    throw new MongoIdFormatException(ExceptionMessages.IdFormatInvalid);

                OnStart(new FileStartEventArgs());

                var gridFsBucket = GetBucked(option.CollectionName);

                await gridFsBucket.DeleteAsync(fileId, _cancellationToken.Token);

                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    OnCancel(new FileCancelEventArgs());
                }
                else
                {
                    OnCompleted(new FileCompletedEventArgs());
                }
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));
            }
        }

        /// <summary>
        /// Обновление метаданных файла
        /// </summary>
        /// <typeparam name="TMetaData">Тип метаданных</typeparam>
        /// <param name="id">Идентификатор файла в базе</param>
        /// <param name="metadata">Метаданные</param>
        /// <param name="option">настройки</param>
        /// <param name="session">Экземпляр сессии</param>
        public async Task UpdateAsync<TMetaData>(string id, TMetaData metadata, FileManagerOption? option = null, Session? session = null)
        {
            option ??= new FileManagerUploadOption();

            try
            {
                option.CollectionName = $"{option.CollectionName}.files";

                if (!ObjectId.TryParse(id, out var fileId))
                    throw new MongoIdFormatException(ExceptionMessages.IdFormatInvalid);

                OnStart(new FileStartEventArgs());

                var collection = _context.Database.GetCollection<BsonDocument>(option.CollectionName);

                var filter = Builders<BsonDocument>.Filter.Eq("_id", fileId);

                var update = Builders<BsonDocument>.Update.Set("metadata", metadata);

                if(session == null) await collection.UpdateOneAsync(filter, update, cancellationToken: _cancellationToken.Token);
                else await collection.UpdateOneAsync(session.SessionHandle, filter, update, cancellationToken: _cancellationToken.Token);

                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    OnCancel(new FileCancelEventArgs());
                }
                else
                {
                    OnCompleted(new FileCompletedEventArgs());
                }
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));
            }
        }

        /// <summary>
        /// Получить информацию о всех файлах в базе данных (включая дополнительные данные)
        /// </summary>
        /// <param name="option">Настройки</param>
        /// <returns>Коллекция с информацией о файлах (включая дополнительные данные)</returns>
        public async Task<List<FileInfoBase>?> GetAllFilesInfoAsync(FileManagerOption? option = null)
        {
            option ??= new FileManagerOption();

            try
            {
                OnStart(new FileStartEventArgs());

                var files = await ListAsync(option.CollectionName);

                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    OnCancel(new FileCancelEventArgs());
                    return null;
                }

                var list = files
                    .Select(fileInfo => new FileInfoBase(fileInfo))
                    .ToList();

                OnCompleted(new FileCompletedEventArgs());

                return list;
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));

                return null;
            }
        }

        /// <summary>
        /// Получить информацию о файлах по имени (включая дополнительные данные)
        /// </summary>
        /// <param name="fileName">Имя фала</param>
        /// <param name="option">Настройки</param>
        /// <returns>Коллекция с информацией о найденных файлах (включая дополнительные данные)</returns>
        public async Task<List<FileInfoBase>?> GetFilesInfoByFileNameAsync(string fileName, FileManagerOption? option = null)
        {
            option ??= new FileManagerOption();

            try
            {
                OnStart(new FileStartEventArgs());

                var files = await GetByNameAsync(fileName, option.CollectionName);

                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    OnCancel(new FileCancelEventArgs());
                    return null;
                }

                var list = files
                    .Select(fileInfo => new FileInfoBase(fileInfo)).ToList();

                OnCompleted(new FileCompletedEventArgs());

                return list;
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));

                return null;
            }
        }

        /// <summary>
        /// Получить информацию о файле по идентификатору (включая дополнительные данные)
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="option">Настройки</param>
        /// <returns>Информация о файле (включая дополнительные данные)</returns>
        public async Task<FileInfoBase?> GetFileInfoByIdAsync(string id, FileManagerOption? option = null)
        {
            option ??= new FileManagerOption();

            try
            {
                OnStart(new FileStartEventArgs());

                var file = await GetByIdAsync(id, option.CollectionName);

                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    OnCancel(new FileCancelEventArgs());
                    return null;
                }

                var fileInfo = new FileInfoBase(file);

                OnCompleted(new FileCompletedEventArgs());

                return fileInfo;
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));

                return null;
            }
        }

        /// <summary>
        /// Получить информацию о всех файлах в базе данных (включая дополнительные данные)
        /// </summary>
        /// <typeparam name="TMetaData">Тип метаданных</typeparam>
        /// <param name="option">Настройки</param>
        /// <returns>Коллекция с информацией о файлах (включая дополнительные данные)</returns>
        public async Task<List<FileInfo<TMetaData>>?> GetAllFilesInfoAsync<TMetaData>(FileManagerOption? option = null)
        {
            option ??= new FileManagerOption();

            try
            {
                OnStart(new FileStartEventArgs());

                var files = await ListAsync(option.CollectionName);

                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    OnCancel(new FileCancelEventArgs());
                    return null;
                }

                var list = files
                    .Select(fileInfo => new FileInfo(fileInfo))
                    .Select(fileInfo => new FileInfo<TMetaData>(fileInfo)).ToList();

                OnCompleted(new FileCompletedEventArgs());

                return list;
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));

                return null;
            }
        }

        /// <summary>
        /// Получить информацию о файлах по имени (включая дополнительные данные)
        /// </summary>
        /// <typeparam name="TMetaData">Тип метаданных</typeparam>
        /// <param name="fileName">Имя фала</param>
        /// <param name="option">Настройки</param>
        /// <returns>Коллекция с информацией о найденных файлах (включая дополнительные данные)</returns>
        public async Task<List<FileInfo<TMetaData>>?> GetFilesInfoByFileNameAsync<TMetaData>(string fileName, FileManagerOption? option = null)
        {
            option ??= new FileManagerOption();

            try
            {
                OnStart(new FileStartEventArgs());

                var files = await GetByNameAsync(fileName, option.CollectionName);

                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    OnCancel(new FileCancelEventArgs());
                    return null;
                }

                var list = files
                    .Select(fileInfo => new FileInfo(fileInfo))
                    .Select(fileInfo => new FileInfo<TMetaData>(fileInfo)).ToList();

                OnCompleted(new FileCompletedEventArgs());

                return list;
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));

                return null;
            }
        }

        /// <summary>
        /// Получить информацию о файле по идентификатору (включая дополнительные данные)
        /// </summary>
        /// <typeparam name="TMetaData">Тип дополнительных данных</typeparam>
        /// <param name="id">Идентификатор файла</param>
        /// <param name="option">Настройки</param>
        /// <returns>Информация о файле (включая дополнительные данные)</returns>
        public async Task<FileInfo<TMetaData>?> GetFileInfoByIdAsync<TMetaData>(string id, FileManagerOption? option = null)
        {
            option ??= new FileManagerOption();

            try
            {
                OnStart(new FileStartEventArgs());

                var file = await GetByIdAsync(id, option.CollectionName);

                if (_cancellationToken.Token.IsCancellationRequested)
                {
                    OnCancel(new FileCancelEventArgs());
                    return null;
                }

                var fileInfo = new FileInfo(file);

                OnCompleted(new FileCompletedEventArgs());

                return new FileInfo<TMetaData>(fileInfo);
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));

                return null;
            }
        }

        /// <summary>
        /// Получить информацию о файлах по дополнительным данным (включая дополнительные данные)
        /// </summary>
        /// <typeparam name="TMetaData">Тип дополнительных данных</typeparam>
        /// <typeparam name="TFieldMetaData">Тип поля дополнительных данных, по которому производится фильтрация</typeparam>
        /// <param name="field">Название поля по которому производится фильтрация.
        /// По полям вложенных документов поиск не производится</param>
        /// <param name="value">Значение поля по которому производится фильтрация.</param>
        /// <param name="option">Настройки</param>
        /// <returns>Список с информацией о файлах (включая дополнительные данных)</returns>
        public async Task<List<FileInfo<TMetaData>>?> ListByMetaDataAsync<TMetaData, TFieldMetaData>(string field, TFieldMetaData value, FileManagerOption? option = null)
        {
            option ??= new FileManagerOption();

            try
            {
                option.CollectionName = $"{option.CollectionName}.files";

                OnStart(new FileStartEventArgs());

                var collection = _context.Database.GetCollection<BsonDocument>(option.CollectionName);

                var filter = Builders<BsonDocument>.Filter.Eq($"metadata.{(field.Equals("Id") ? "_id" : field)}", value);

                var fileInfo = collection.Find(filter).ToList();

                var list = new List<FileInfo<TMetaData>>();

                foreach (var doc in fileInfo)
                {
                    if (_cancellationToken.Token.IsCancellationRequested)
                    {
                        OnCancel(new FileCancelEventArgs());
                        return null;
                    }

                    var file = await GetByIdAsync(doc["_id"].AsObjectId.ToString(), option.CollectionName);
                    var info = new FileInfo(file);

                    list.Add(new FileInfo<TMetaData>(info));
                }

                OnCompleted(new FileCompletedEventArgs());

                return list;
            }
            catch (Exception e)
            {
                OnCrash(new FileCrashEventArgs(e));

                return null;
            }
        }

        #endregion

        #region DELEGATES

        public delegate void StartHandler(FileManager sender, FileStartEventArgs e);
        public delegate void CompletedHandler(FileManager sender, FileCompletedEventArgs e);
        public delegate void CrashHandler(FileManager sender, FileCrashEventArgs e);
        public delegate void CancelHandler(FileManager sender, FileCancelEventArgs e);
        public delegate void ProgressHandler(FileManager sender, FileProgressEventArgs e);

        #endregion

        #region EVENTS

        /// <summary>
        /// Событие начала работы с файлом
        /// </summary>
        public event StartHandler? Start;

        /// <summary>
        /// Событие завершения работы с файлом
        /// </summary>
        public event CompletedHandler? Completed;

        /// <summary>
        /// Событие возникновения исключения при работе с файлом
        /// </summary>
        public event CrashHandler? Crash;

        /// <summary>
        /// Событие отмены работы с файлом
        /// </summary>
        public event CancelHandler? Cancel;

        /// <summary>
        /// Событие изменения прогресса скачивания или загрузки
        /// </summary>
        public event ProgressHandler? Progress;

        #endregion
    }

    /// <summary>
    /// Настройки методов
    /// </summary>
    public class FileManagerOption
    {
        /// <summary>
        /// Название коллекции файлового хранилища
        /// </summary>
        public string CollectionName
        {
            get => _collectionName;
            set
            {
                if(string.IsNullOrWhiteSpace(value)) return;
                _collectionName = value;
            }
        }
        private string _collectionName = "Files";
    }

    /// <summary>
    /// Настройки методов загрузки в базу данных
    /// </summary>
    public class FileManagerUploadOption : FileManagerOption
    {
        /// <summary>
        /// Предустановленный идентификатор
        /// </summary>
        public string? PresetId
        {
            get => _presetId;
            set => _presetId = string.IsNullOrWhiteSpace(value) ? null : value;
        }
        private string? _presetId;
    }

    /// <summary>
    /// Объект потока загрузки в БД
    /// </summary>
    public class UploadStream
    {
        private readonly GridFSUploadStream<ObjectId>? _uploadStream;

        public UploadStream(Stream uploadStream)
        {
            _uploadStream = uploadStream as GridFSUploadStream<ObjectId>;
        }

        /// <summary>
        /// Поток записи
        /// </summary>
        public Stream? WriteStream => _uploadStream;

        /// <summary>
        /// Идентификатор загружаемого файла
        /// </summary>
        public string? Id => _uploadStream?.Id.ToString();

        /// <summary>
        /// Подтвердить окончания загрузки и закрыть поток записи
        /// </summary>
        public async Task CommitAndCloseAsync()
        {
            if (_uploadStream == null) return;

            await _uploadStream.CloseAsync();
        }

        /// <summary>
        /// Откатить загрузку и закрыть поток записи
        /// </summary>
        public async Task AbortAndCloseAsync()
        {
            if (_uploadStream == null) return;

            await _uploadStream.AbortAsync();
            _uploadStream.Close();
        }
    }
}