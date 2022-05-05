using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMClothing.Models.ObViewModels
{
    public class PedidosViewModel
    {
        public int id_entrega { get; set; }
        public int id_venta { get; set; }
        public string direccion { get; set; }
        public string nombre_cliente { get; set; }

        public string fecha_salida { get; set; }
        public string fecha_entrega { get; set; }

    }
}