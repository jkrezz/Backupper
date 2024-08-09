using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.IO.Compression;

namespace Infotecs_1
{
    public class Backupper
    {
        private Directories _options;
        private Logger _log;
        /// <summary>
        /// Инициализирует новый экземпляр класса Backupper.
        /// </summary>
        /// <param name="options">Объект содержащий информацию о директориях для резервного копирования.</param>
        /// <param name="logger">Экземпляр для записи логов.</param>
        public Backupper(Directories options, Logger logger)
        {
            _options = options;
            _log = logger;
        }

        /// <summary>
        /// Запускает процесс резервного копирования, создавая временную директорию (архив) в целевой папке и копируя файлы 
        /// из источников в эту директорию.
        /// </summary>
        /// <param name="timestamp">Временной штамп, используемый для создания уникального имени 
        /// директории для резервного копирования.</param>
        public void Run(string timestamp)
        {
            _log.Information("Программа запущена");

            // Путь к архиву
            string zipFilePath = Path.Combine(_options.Target, $"Backup_{timestamp}.zip");

            _log.Information("Начало резервного копирования");
            _log.Information($"Создание ZIP-архива для резервной копии: {zipFilePath}");

            try
            {
                using (var zipToCreate = new FileStream(zipFilePath, FileMode.Create))
                using (var archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    foreach (var source in _options.Sources)
                    {
                        _log.Information($"Начата работа с директорией {source}.");
                        AddDirectoryToZip(archive, source);
                        _log.Information($"Работа с директорией {source} завершена.");
                    }
                }

                _log.Information("Резервное копирование завершено.");
            }
            catch (Exception ex)
            {
                _log.Error($"Произошла ошибка при создании архива: {ex.Message}");
            }
            _log.Information("Нажмите любую клавишу, чтобы выйти.");
            Console.ReadKey();
        }

        private void AddDirectoryToZip(ZipArchive archive, string sourceDirectory)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                _log.Error($"Директория {sourceDirectory} не найдена.");
                return;
            }

            // Копируем файлы в архив
            foreach (var file in Directory.EnumerateFiles(sourceDirectory))
            {
                try
                {
                    string fileName = Path.GetFileName(file);
                    var entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);

                    using (var entryStream = entry.Open())
                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        fileStream.CopyTo(entryStream);
                        _log.Debug($"Файл {file} успешно добавлен в архив.");
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    _log.Error($"Ошибка доступа к файлу {file}: {ex.Message}");
                }
                catch (IOException ex)
                {
                    _log.Error($"Ошибка копирования файла {file}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Произошла непредвиденная ошибка с файлом {file}: {ex.Message}");
                }
            }
        }


        /*private void CopyFilesFromDirectory(string sourceDirectory, string targetDirectory)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                _log.Error($"Директория {sourceDirectory} не найдена.");
                return;
            }

            // Создаем целевую директорию, если ее нет
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
                _log.Information($"Директория {targetDirectory} успешно создана.");
            }

            // Копируем файлы
            foreach (var file in Directory.EnumerateFiles(sourceDirectory))
            {
                try
                {
                    string fileName = Path.GetFileName(file);

                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    File.Copy(file, targetFilePath, true); // Копирование с перезаписью

                    _log.Debug($"Файл {file} успешно скопирован в {targetFilePath}.");
                }
                catch (UnauthorizedAccessException ex)
                {
                    _log.Error($"Ошибка доступа к файлу {file}: {ex.Message}");
                }
                catch (IOException ex)
                {
                    _log.Error($"Ошибка копирования файла {file}: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _log.Error($"Произошла непредвиденная ошибка с файлом {file}: {ex.Message}");
                }
            }
        }*/
    }
}
