using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMClothing.Models.ObViewModels
{
    public class DetalleVentaViewModel
    {
        public int id_venta { get; set; }
        public int id_producto { get; set; }
        public string producto { get; set; }
        public int cantidad { get; set; }
        public decimal subtotal { get; set; }
    }
}