using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infotecs_1
{
    /// <summary>
    /// Представляет директории, участвующие в резервном копировании.
    /// </summary>
    public class Directories
    {
        /// <summary>
        /// Получает или задает массив путей к исходным директориям.
        /// </summary>
        public string[] Sources { get; set; }

        /// <summary>
        /// Получает или задает путь к целевой директории.
        /// </summary>У
        public string Target { get; set; }
    }
}
