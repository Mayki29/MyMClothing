using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMClothing.Models.ObViewModels
{
    public class ProductoViewModel
    {
        public int id { get; set; }
        public string nombreProd { get; set; }
        public string descripcion { get; set; }
        public decimal precio_venta { get; set; }
        public decimal precio_produccion { get; set; }
        public decimal prom { get; set; }
        public string categoria { get; set; }
        public string promres { get; set; }
        public string img { get; set; }
        public string talla { get; set; }
        public int? descuento { get; set; }
        public int? cantidad { get; set; }
    }
    public class AddProductoViewModel
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        [DisplayName("Precio de venta")]
        public decimal Precio_Venta { get; set; }
        [Required]
        [DisplayName("Precio de produccion")]
        public decimal Precio_Produccion { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        [DisplayName("Imagen")]
        public HttpPostedFileBase Url_Imagen { get; set; }
        [Required]
        public string Talla { get; set; }
    }

    public class EditProductoViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public decimal Precio_Venta { get; set; }
        [Required]
        public decimal Precio_Produccion { get; set; }
        [Required]
        public int Stock { get; set; }
        [DisplayName("Imagen")]
        public HttpPostedFileBase Url_Imagen { get; set; }
        [Required]
        public string Talla { get; set; }
        public int Categoria { get; set; }
        public int Temporada { get; set; }
    }
}