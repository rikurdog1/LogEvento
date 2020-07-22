using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NLog;

namespace LogEvento.archAtencAcciion
{
    class ArchAtencAccion 
    {
        // Declaración del manejador de log.
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Action<String, String> moverarch = (orgArch, destArch) => File.Move(orgArch, destArch);

        public Action<String, String, String> adjuntarrarch = (orgArch, destArch, dir) => {
            if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
            String destino = dir + DateTime.Now.ToString("yyyyMMdd") + destArch;
            File.AppendAllLines(destino, File.ReadAllLines(orgArch));
            File.WriteAllText(orgArch, string.Empty);
            logger.Info("Se adjuntaron los eventos del archivo {0} al log de atendidos en el archivo {1}.", orgArch, destino);
        };
     
        public Action<String> marcar = (o) => { Console.WriteLine(o); };
    }
}