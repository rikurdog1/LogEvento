using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogEvento
{
    /// <summary>
    /// Clase para monitorear un directorio de log.
    /// </summary>
    class MonitorLog
    {
        private String _directorio;
        private string _arch_tipo;
        String duplicado = null;

        // Declaración del manejador de log.
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Costructor con parametros.
        /// <param name="directorio">directorio String Directorio a monitorear.</param>
        /// <param name="arch_tipo">arch_tipo   String Tipo de archivo a moritorear Ejm: *.log.</param>
        /// </summary>
        public MonitorLog(String directorio, string arch_tipo)
        {
            this._directorio = directorio;
            this._arch_tipo = arch_tipo;
        }

        /// <summary>
        /// Inicia el proceso de monitoreo.
        /// 
        ///<exception cref = "LogEvento.monitorException" > Lanza una excepción de tipo monitorException.</exception>
        /// </summary>
        public void monitorear()
        {

            // Valida si existe el directorio a monitorear, lanza una exepción de tipo monitorException.
            if (!Directory.Exists(this._directorio))
            {

                Directory.CreateDirectory(this._directorio);
                PrometheusLog.prometheo_exception.Inc(1);
                throw new monitorException($"El directorio {this._directorio} no existe, debe proporcionar una carpeta para poder monitorear los Log de la aplicación SIMF.. ");
            }

            // Acción cuando se detecta la modificación de un archivo.
            Action<object, FileSystemEventArgs> cambio = (s, e) =>
            {
                String linetiempo = e.Name + DateTime.Now.ToString("yyyyMMddHHmm");
                if (duplicado != (e.Name + linetiempo))
                {
                    Console.WriteLine("Diferentes..................{0}", linetiempo);
                    duplicado = e.Name + linetiempo;
                    manejadorArchivo m = new manejadorArchivo();
                    m.Accion(e.FullPath, e.Name);
                }
                else
                {
                    Console.WriteLine("iguales..................{0}", linetiempo);
                    duplicado = null;
                }
                //Thread.Sleep(2);
            };

            // Acción cuando se detecta el borrado de un archivo.
            Action<object, FileSystemEventArgs> borrado = (s, e) =>
            {
                PrometheusLog.prometheo_error.Inc(1);
                logger.Error("Se ha borrados el archivo: {0}", e.FullPath);
                Console.WriteLine(e.FullPath);
            };

            // Inicia el proceso de monitoteo del directorio.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = this._directorio;
            watcher.IncludeSubdirectories = false;
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size;

            watcher.Filter = this._arch_tipo;
            watcher.Changed += new FileSystemEventHandler(cambio);
            //watcher.Created += new FileSystemEventHandler(borrado);
            //watcher.Deleted += new FileSystemEventHandler(borrado);

            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Esperando cambios en archivos...........");
        }
    }
}