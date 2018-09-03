using System;
using System.Collections.Generic;
using System.Text;

namespace ElGasCamion.Models
{
    public class Parametro
    {
        public int IdParametro { get; set; }

        public string Nombre { get; set; }

        public double? Valor { get; set; }

        public string Mensaje { get; set; }
    }
}
