using System;
using System.Collections.Generic;
using System.Text;

namespace ElGasCamion.Models
{
    public partial class Ruta
    {
        public int IdRuta { get; set; }
        public int? IdDistribuidor { get; set; }
        public double? Longitud { get; set; }
        public double? Latitud { get; set; }
    }
    public class Distribuidores
    {
        string id;

    }

    public partial class Rutav2
    {
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
    }
}
