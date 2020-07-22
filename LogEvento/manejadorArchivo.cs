using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Configuration;
using LogEvento.archAtencAcciion;

namespace LogEvento
{
    /// <summary>
    /// Clase paar manejar los archivos modificados detectados por el monitor de archivo.
    /// </summary>
    class manejadorArchivo {
        // Declaración del manejador de log.
        private static Logger logger = LogManager.GetCurrentClassLogger();

        ArchUtiliti a = new ArchUtiliti();

        public Action<int,String> action_lanzMail = (umbral, arcRuta) => {
            if (umbral == 0) return;
            if ((umbral == 1) || ((umbral % Convert.ToInt32(ConfigurationManager.AppSettings["sug_umbral_Error"])) == 0)) 
                logger.Error("Existen {0} errores sin atender en la ruta {1}.", umbral, arcRuta);
            };

            /// <summary>
            /// Interpreta con el nombre del archivo la acción a ejecutar para el archivo de log.
            /// 
            /// <param name="ruta">ruta String Ruta del archivo.</param>
            /// <param name="nombre">nombre String Nombre del archivo.</param>
            /// 
            /// </summary>
            public void Accion(String ruta, String nombre) {
            switch (nombre) {
                case "prueba.log":
                    archLavel(ruta);
                    break;
                case "caracas.log":
                    //archPrueba(ruta);
                    a.adjuntarrarch.Invoke(@"C:\Proyectos C#\LogSIMF\Prueba.log", "Prueba.log", @ConfigurationManager.AppSettings["logDirAtendidos"]);
                    break;
                default:
                    PrometheusLog.prometheo_warning.Inc(1);
                    logger.Warn("No existe manejador para este archivo donde visualizar el registro.");
                    Console.WriteLine("No existe manejador para este archivo donde visualizar el registro.");
                    break;
            }
            return;
        }

        /// <summary>
        /// Interpreta el archivo Prueba.log.
        /// 
        /// <param name="ruta">ruta String Ruta del archivo.</param>
        /// 
        /// </summary>
        private void archPrueba(String ruta) {
            Console.WriteLine(LeerAllReg(ruta).Where(x => !String.IsNullOrEmpty(x)).Last());
        }

        private void archLavel(String ruta) {
            List<string> lineas = LeerAllReg(ruta);
            
            PrometheusLog.prometheo_info.Set(lineas.Where(x => x.Contains("[INFO]", StringComparison.OrdinalIgnoreCase)).Count());
            PrometheusLog.prometheo_warning.Set(lineas.Where(x => x.Contains("[WARN]", StringComparison.OrdinalIgnoreCase)).Count());
            PrometheusLog.prometheo_error.Set(lineas.Where(x => x.Contains("[ERROR]", StringComparison.OrdinalIgnoreCase)).Count());
           
            Console.WriteLine(PrometheusLog.prometheo_info.Value);
            Console.WriteLine(PrometheusLog.prometheo_error.Value);

            action_lanzMail.Invoke((int)PrometheusLog.prometheo_error.Value, ruta);
           
            a.extraLogLevel.Invoke("INFO", a.LeerAllrow.Invoke(ruta))?. ForEach(a => Console.WriteLine("Reg level: " + a.ToString()));
        }

        /// <summary>
        /// Permite leer un archivo de principio a fin y colicarlo en una lista.
        /// 
        /// <param name="ruta">ruta String Ruta del archivo.</param>
        ///  <returns>Retorna una lista con los registro del archivo</returns>
        /// </summary>
        private List<String> LeerAllReg(String ruta) {

            try {
                List<string> lines = new List<string>();
                using (FileStream fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs)) {

                    PrometheusLog.prometheo_info.Inc(1);
                    logger.Info("Se modifico el archivo: {0}", ruta);
                    Console.WriteLine("Antesde leer...." + ruta);
                                     
                    //while (!sr.EndOfStream) {lines.Add(sr.ReadLine()); }
                                                
                    String line;
                    while ((line = sr.ReadLine()) != null) lines.Add(line);

                    //Console.WriteLine(lines.Where(x => !String.IsNullOrEmpty(x)).Last());
                    //lines.ForEach((l)=> Console.WriteLine(l));

                    sr.Close(); sr.Dispose();
                    fs.Close(); fs.Dispose();
                
                    //if (lines.Count() == 0) lines = LeerAllReg(ruta);

                    return lines;
                }
            } catch (Exception e) {
                Console.WriteLine("Error....." + e.Message);
                return null;
            }
        }
    }
}
