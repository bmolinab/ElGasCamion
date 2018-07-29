using System;
using System.Collections.Generic;
using System.Text;

namespace ElGasCamion.Helpers
{
    /// <summary>
    /// Clase para envio de mensajes genericos desde el api rest
    /// </summary>
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }

    }
}
