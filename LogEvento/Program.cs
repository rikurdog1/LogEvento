using NLog;
using NLog.Fluent;
using System;
using Prometheus;
using System.Configuration;

namespace LogEvento
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args) {

            // Inicializar variables para Nlog
            //NlogIni();

            MetricServer metricServer = new MetricServer(port: 7073);
            int milliSecondsToSleep = 500;

            metricServer.Start(); 

            const int warning_interval      = 0;
            const int exception_interval    = 0;
            const int error_interval        = 0;
            const int information_interval  = 1000;


            //scrape counters
            PrometheusLog.prometheo_error       = Metrics.CreateGauge("prometheo_error", "Este campo indica que han ocurridos errores en los logs.");
            PrometheusLog.prometheo_warning     = Metrics.CreateGauge("prometheo_warning", "Este campo indica el numero de warnigs en los logs.");
            PrometheusLog.prometheo_exception   = Metrics.CreateGauge("prometheo_exception", "Este campo indica el numero de exception en los logs.");
            PrometheusLog.prometheo_info        = Metrics.CreateGauge("prometheo_info", "Este campo indica el numero de los logs de información.");

            //Counters
            /*
            PrometheusLog.prometheo_error       = prometheo_error;
            PrometheusLog.prometheo_warning     = prometheo_warning;
            PrometheusLog.prometheo_exception   = prometheo_exception;
            PrometheusLog.prometheo_info        = prometheo_info;
            */


            PrometheusLog.prometheo_info.Inc(1);
            logger.Info("Iniciando Monitor de log............");

            // Inicializa el motor de monitoreo.

            try {
                MonitorLog monitor = new MonitorLog(@ConfigurationManager.AppSettings["logDir"], "*.log");        
                monitor.monitorear();
            }catch (monitorException mex) {
                PrometheusLog.prometheo_exception.Inc(1);
                logger.Error(mex.Message);
                return;
             }
            
            Console.WriteLine("Precione <enter. para continuar");

            var String = Console.ReadLine();
            if (String=="Fin") {
                metricServer.Stop();
            }
        }

        public static void NlogIni() {
        
        // Configuracion para el correo --------------------------------------------------------------
            GlobalDiagnosticsContext.Set("usuario", ConfigurationManager.AppSettings["usuario"]);
            GlobalDiagnosticsContext.Set("clave", ConfigurationManager.AppSettings["clave"]);
            GlobalDiagnosticsContext.Set("email", ConfigurationManager.AppSettings["email"]);
            GlobalDiagnosticsContext.Set("to", ConfigurationManager.AppSettings["to"]);

            //LogManager.Configuration.Variables["UserId_From_DB"] = "42";
        }
    }
}