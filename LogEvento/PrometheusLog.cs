using System;
using System.Collections.Generic;
using System.Text;
using Prometheus;

namespace LogEvento
{

    public static class PrometheusLog
    {
        public static Gauge prometheo_error { get; set; }
        public static Gauge prometheo_warning { get; set; }
        public static Gauge prometheo_exception { get; set; }
        public static Gauge prometheo_info { get; set; }
    }
}