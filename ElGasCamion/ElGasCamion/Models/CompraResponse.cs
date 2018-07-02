using System;
using System.Collections.Generic;
using System.Text;

namespace ElGasCamion.Models
{
    public class CompraResponse
    {
        public int IdCompra { get; set; }
        public int? IdCliente { get; set; }
        public int? IdDistribuidor { get; set; }
        public double? Longitud { get; set; }
        public double? Latitud { get; set; }
        public int? Cantidad { get; set; }
        public string NombreCliente { get; set; }
        public string Telefono { get; set; }
        public double? ValorTotal { get; set; }

    }
}
