using System;
using System.Collections.Generic;
using System.Text;

namespace LogEvento
{
    /// <summary>
    /// Clase para el manejo de las excepciones lanzadas por el proceso de moniroreo de archivo.
    /// </summary>
    [Serializable]
    class monitorException : Exception{
        public monitorException(String mensaje) : base(mensaje)  { }
    }

}
