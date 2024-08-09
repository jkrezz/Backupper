using System;
using System.IO;
using Newtonsoft.Json;
using Infotecs_1;
using Serilog.Core;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace Infotecs_1
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

            var pathsOptions = configuration.GetSection("Paths").Get<Directories>();

            Directories directories = new Directories();
            directories.Sources = pathsOptions.Sources;
            directories.Target = pathsOptions.Target;

            string time = DateTime.Now.ToString("yyyyMMddHHmmss");

            Backupper backuper = new Backupper(directories, GetLogger(configuration, time));
            backuper.Run(time);

        }

        /// <summary>
        /// Создает и настраивает экземпляр логгера с использованием предоставленной конфигурации.
        /// </summary>
        /// <param name="conf">Объект конфигурации, содержащий настройки для Serilog.</param>
        /// <param name="time">Временной штамп, который используется для создания имени файла журнала.</param>
        /// <returns>Возвращает настроенный логгер</returns>
        static public Logger GetLogger(IConfiguration conf, string time)
        {
            string filePath = $"logs/log_{time}.txt";

            Logger log = new LoggerConfiguration()
            .ReadFrom.Configuration(conf)
            .WriteTo.File(filePath)
            .CreateLogger();

            return log;
        }
    }
}
