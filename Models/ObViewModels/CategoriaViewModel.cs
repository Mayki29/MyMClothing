using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMClothing.Models.ObViewModels
{
    public class CategoriaViewModel
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string url_imagen { get; set; }
    }

    public class AddCategoriaViewModel
    {
        public int id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        [DisplayName("Imagen")]
        public HttpPostedFileBase Url_imagen { get; set; }
    }

    public class EditCategoriaViewModel
    {
        public int id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [DisplayName("Imagen")]
        public HttpPostedFileBase Url_imagen { get; set; }
    } 
}