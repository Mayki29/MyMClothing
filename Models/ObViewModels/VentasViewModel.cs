using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMClothing.Models.ObViewModels
{
    public class VentasViewModel
    {
        public int id_venta { get; set; }
        public string nombre_usuario { get; set; }
        public string correo { get; set; }
        public DateTime fecha { get; set; }
        public decimal total { get; set; }
        public string estado { get; set; }
    }
}