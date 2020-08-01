using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogEvento.archAtencAcciion
{
    class ArchUtiliti 
    {
        // Declaración del manejador de log.
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly String regexLeveMask = @"^(INFO|ERROR|DEBUG|TRACE|WARN|FATAL|OFF|ALL)$";

        public Action<String, String> moverarch = (orgArch, destArch) => File.Move(orgArch, destArch);

        // Mueve el contenido de un archivo (orgArch) a otro archivo de texto en la ruta (dir + destArch) conservando el nombre del archivo origen (orgArch)
        // colocando un prefijo de yyyyMMdd.
        public Action<String, String, String> adjuntarrarch = (dirOrg, archNombre, dirDest) => {
            
            try {
                if (!Directory.Exists(dirDest)) { Directory.CreateDirectory(dirDest); }
                String origen = dirOrg + archNombre;
                String destino = dirDest + DateTime.Now.ToString("yyyyMMdd") + archNombre;
                File.AppendAllLines(destino, File.ReadAllLines(origen));
                File.WriteAllText(origen, string.Empty);

                logger.Info("Se adjuntaron los eventos del archivo {0} al log de atendidos en el archivo {1}.", origen, destino);
            }
            catch (Exception e)
            {
                logger.Info("excepción al extraer level (LeerAllrow) {0}", e.Message);
            }
        };

        // Retorna una lista (reg) todos los registrod de un archivo (rutaArch).
        public Func<String, List<String>> LeerAllrow = (rutaArch) => {
            try
            {
                List<String> reg = File.ReadAllLines(rutaArch).ToList();
                reg.ForEach(r => Console.WriteLine("Datos de los reg: {0}", r.ToString()));
                return reg;
            }
            catch (Exception e) {
                logger.Info("excepción al extraer level (LeerAllrow) {0}", e.Message);
                return null;     
            }
        };

        // Retorna de un arreglo el level solicitado en el parametro (level) de la lista (listReg).
        public Func<String, List<String>, List<String>> extraLogLevel = (level, listReg) => {
            try
            {
                Regex regexLevel = new Regex(regexLeveMask, RegexOptions.IgnoreCase);
                if (!regexLevel.IsMatch(level)) { throw new monitorException("Debe ser un nivel del log ejem: (INFO ERROR DEBUG...), no puede ser " + level); };
                return listReg.Where(x => x.Contains("[" + level + "]", StringComparison.OrdinalIgnoreCase)).ToList();
            }
            catch (Exception e) {
                logger.Info("excepción al extraer level (extraLogLevel) {0}", e.Message);
                return null;
            }        
        };

        // Lanza el correo de error cuando este en el umbral inicial o el siguiente
        public Action<int, String> action_lanzMail = (umbral, arcRuta) => {
            if (umbral == 0) return;
            if ((umbral == Convert.ToInt32(ConfigurationManager.AppSettings["ini_umbral_Error"])) || ((umbral % Convert.ToInt32(ConfigurationManager.AppSettings["sug_umbral_Error"])) == 0))
                logger.Error("Existen {0} errores sin atender en la ruta {1}.", umbral, arcRuta);
        };
    }
}