//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyMClothing.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DETALLE_VENTA
    {
        public int ID_ITEM { get; set; }
        public int ID_VENTA { get; set; }
        public int ID_PRODUCTO { get; set; }
        public int CANTIDAD { get; set; }
        public decimal SUB_TOTAL { get; set; }
    
        public virtual PRODUCTO PRODUCTO { get; set; }
        public virtual VENTA VENTA { get; set; }
    }
}
