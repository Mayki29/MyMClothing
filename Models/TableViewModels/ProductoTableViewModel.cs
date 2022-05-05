using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMClothing.Models.TableViewModels
{
    public class ProductoTableViewModel
    {
        public int ID_Producto { get; set; }
        public string Nombre { get; set; }
        public decimal Precio_Venta { get; set; }
        public decimal Precio_Produccion { get; set; }
        public decimal Reseña { get; set; }
        public string Categoria { get; set; }
        public string Temporada { get; set; }
        public int Stock { get; set; }
        public string Talla { get; set; }
        public int? descuento { get; set; }

    }
}