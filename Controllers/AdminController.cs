using MyMClothing.Filters;
using MyMClothing.Models;
using MyMClothing.Models.TableViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using MyMClothing.Models.ObViewModels;
using System.IO;

namespace MyMClothing.Controllers
{
    public class AdminController : Controller
    {
        //private DB_MMCEntities1 db = new DB_MMCEntities1();
        // GET: Admin
        [AuthorizeUser(idRol:1)]
        public ActionResult Index()
        {
            string[] listmeses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Noviembre", "Diciembre" };
            var meses = new List<string>();
            var ventas = new List<int>();
            var usuarios = new List<int>();
            int Cventas = 0;
            int Cusuarios = 0;
            decimal ganancias = 0;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                for( var c = 1; c<= DateTime.Today.Month; c++)
                {
                    int v = Convert.ToInt32(db.sp_contarVentas(c, DateTime.Today.Year).Single());
                    int u = Convert.ToInt32(db.sp_nuevosUsuarios(c, DateTime.Today.Year).Single());
                    meses.Add(listmeses[c - 1]);
                    ventas.Add(v);
                    usuarios.Add(u);
                }
                var g = db.sp_gananciasTienda(DateTime.Today.Month, DateTime.Today.Year).SingleOrDefault();
                if(g == null)
                {
                    ganancias = 0;
                }
                else
                {
                    ganancias = Convert.ToDecimal(g);
                }
                Cventas = Convert.ToInt32(db.sp_contarVentas(DateTime.Today.Month, DateTime.Today.Year).Single());
                Cusuarios = Convert.ToInt32(db.sp_nuevosUsuarios(DateTime.Today.Month, DateTime.Today.Year).Single());
            }
            ViewBag.Usuarios = usuarios;
            ViewBag.Meses = meses;
            ViewBag.Ventas = ventas;
            ViewBag.Cganancias = ganancias;
            ViewBag.Cventas = Cventas;
            ViewBag.Cusuarios = Cusuarios;

            return View();
        }

        #region CRUDProductos

        [AuthorizeUser(idRol: 1)]
        
        public ActionResult Productos(string txtbusqueda)
        {
            string busqueda = "";
            if(txtbusqueda != null)
            {
                busqueda = txtbusqueda;
            }
            
            List<ProductoTableViewModel> lst = null;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                var lista = db.listaProductos(busqueda).ToList();
                lst = lista.ConvertAll(l =>
                {
                    return new ProductoTableViewModel()
                    {
                        ID_Producto = l.ID_PRODUCTO,
                        Nombre = l.NOMBRE_PRODUCTO,
                        Precio_Venta = l.PRECIO_VENTA,
                        Precio_Produccion = l.PRECIO_PRODUCCION,
                        Reseña = l.PROMEDIO_RESENA,
                        Categoria = l.NOMBRE,
                        Temporada = l.NOM_TEMPORADA,
                        Stock = l.STOCK_TOTAL,
                        Talla = l.TALLA,
                        descuento = l.DESCUENTO
                    };
                });
            }
            return View(lst);
        }
        [AuthorizeUser(idRol: 1)]
        [HttpGet]
        public ActionResult AgregarProducto()
        {

            ViewBag.items1 = ObtenerCategoria();
            ViewBag.items2 = ObtenerTemporada();
            return View();
        }
        [HttpPost]
        public ActionResult AgregarProducto(AddProductoViewModel model, int Categoria, int Temporada)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.items1 = ObtenerCategoria(Categoria);
                ViewBag.items2 = ObtenerTemporada(Temporada);
                return View(model);
            }
            string tip = model.Url_Imagen.ContentType.Substring(0, 5);
            if(tip != "image")
            {
                ModelState.AddModelError("Url_Imagen", "Solo puede ingresar imagenes");
                ViewBag.items1 = ObtenerCategoria(Categoria);
                ViewBag.items2 = ObtenerTemporada(Temporada);
                return View(model);
            }


            string RutaSitio = Server.MapPath("~/");
            string PathImagen = Path.Combine(RutaSitio + "/img/" + model.Url_Imagen.FileName);
            model.Url_Imagen.SaveAs(PathImagen);

            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                PRODUCTO oProducto = new PRODUCTO();
                oProducto.NOMBRE_PRODUCTO = model.Nombre;
                oProducto.DESCRIPCION = model.Descripcion;
                oProducto.PRECIO_VENTA = model.Precio_Venta;
                oProducto.PRECIO_PRODUCCION = model.Precio_Produccion;
                oProducto.PROMEDIO_RESENA = 0;
                oProducto.ID_CATEGORIA = Categoria;
                oProducto.STOCK_TOTAL = model.Stock;
                oProducto.URL_IMAGEN = model.Url_Imagen.FileName;
                oProducto.ESTADO = 1;
                oProducto.TALLA = model.Talla;
                oProducto.ID_TEMPORADA = Temporada;
                db.PRODUCTO.Add(oProducto);
                db.SaveChanges();
            }
            return Redirect(Url.Content("~/Admin/Productos"));

        }
        [AuthorizeUser(idRol: 1)]
        public ActionResult EditarProducto(int Id)
        {
            EditProductoViewModel model = new EditProductoViewModel();
            string RutaSitio = Server.MapPath("~/");
            
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                var oProducto = db.PRODUCTO.Find(Id);
                model.Id = oProducto.ID_PRODUCTO;
                model.Nombre = oProducto.NOMBRE_PRODUCTO;
                model.Descripcion = oProducto.DESCRIPCION;
                model.Precio_Venta = oProducto.PRECIO_VENTA;
                model.Precio_Produccion = oProducto.PRECIO_PRODUCCION;
                model.Stock = oProducto.STOCK_TOTAL;
                string PathImagen = Path.Combine(RutaSitio + "/img/" + oProducto.URL_IMAGEN);
                //model.Url_Imagen = ;
                model.Talla = oProducto.TALLA;
                model.Categoria = oProducto.ID_CATEGORIA;
                model.Temporada = oProducto.ID_TEMPORADA;
            }

            ViewBag.items1 = ObtenerCategoria(model.Categoria);
            ViewBag.items2 = ObtenerTemporada(model.Temporada);

            return View(model);
        }
        [AuthorizeUser(idRol: 1)]
        [HttpPost]
        public ActionResult EditarProducto(EditProductoViewModel model, int Categoria, int Temporada)
        {


            if (!ModelState.IsValid)
            {
                ViewBag.items1 = ObtenerCategoria(Categoria);
                ViewBag.items2 = ObtenerTemporada(Temporada);
                return View(model);
            }

            if (model.Url_Imagen != null)
            {
                string tip = model.Url_Imagen.ContentType.Substring(0, 5);
                if (tip != "image")
                {
                    ModelState.AddModelError("Url_Imagen", "Solo puede ingresar imagenes");
                    return View(model);
                }
                else
                {
                    string RutaSitio = Server.MapPath("~/");
                    string PathImagen = Path.Combine(RutaSitio + "/img/" + model.Url_Imagen.FileName);
                    model.Url_Imagen.SaveAs(PathImagen);
                }
            }

            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                var oProducto = db.PRODUCTO.Find(model.Id);
                oProducto.NOMBRE_PRODUCTO = model.Nombre;
                oProducto.DESCRIPCION = model.Descripcion;
                oProducto.PRECIO_VENTA = model.Precio_Venta;
                oProducto.PRECIO_PRODUCCION = model.Precio_Produccion;
                oProducto.ID_CATEGORIA = Categoria;
                oProducto.STOCK_TOTAL = model.Stock;
                if (model.Url_Imagen != null)
                    oProducto.URL_IMAGEN = model.Url_Imagen.FileName;
                oProducto.TALLA = model.Talla;
                oProducto.ID_TEMPORADA = Temporada;

                db.Entry(oProducto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Redirect(Url.Content("~/Admin/Productos"));
        }
        [AuthorizeUser(idRol: 1)]
        [HttpPost]
        public ActionResult EliminarProducto(int Id)
        {
            try
            {
                using (DB_MMCEntities1 db = new DB_MMCEntities1())
                {
                    var oProducto = db.PRODUCTO.Find(Id);
                    oProducto.ESTADO = 0;

                    db.Entry(oProducto).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
            
        }
        [AuthorizeUser(idRol: 1)]
        public ActionResult AplicarDescuento(int id, int cantidad)
        {
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                var oProducto = db.PRODUCTO.Find(id);
                if(cantidad == 0)
                {
                    oProducto.DESCUENTO = null;
                }
                else
                {
                    oProducto.DESCUENTO = cantidad;
                }
                db.Entry(oProducto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

                return Redirect(Url.Content("~/Admin/Productos"));
        }
        [AuthorizeUser(idRol: 1)]
        public ActionResult QuitarDescuento(int id)
        {
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                var oProducto = db.PRODUCTO.Find(id);
                oProducto.DESCUENTO = null;
                db.Entry(oProducto).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Redirect(Url.Content("~/Admin/Productos"));
        }

        #endregion

        #region CRUDCategoria

        [AuthorizeUser(idRol: 1)]
        public ActionResult Categorias()
        {
            List<CategoriaViewModel> lst = null;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                lst = (from d in db.CATEGORIA
                       where d.ESTADO == 1
                       orderby d.ID_CATEGORIA descending
                        select new CategoriaViewModel
                        {
                            id = d.ID_CATEGORIA,
                            nombre = d.NOMBRE,
                            url_imagen = d.URL_IMAGEN
                        }).ToList();

            }

                return View(lst);
        }

        [AuthorizeUser(idRol: 1)]
        public ActionResult AgregarCategoria()
        {
                return View();
        }
        [AuthorizeUser(idRol: 1)]
        [HttpPost]
        public ActionResult AgregarCategoria(AddCategoriaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string tip = model.Url_imagen.ContentType.Substring(0, 5);
            if (tip != "image")
            {
                ModelState.AddModelError("Url_imagen", "Solo puede ingresar imagenes");
                return View(model);
            }


            string RutaSitio = Server.MapPath("~/");
            string PathImagen = Path.Combine(RutaSitio + "/img/" + model.Url_imagen.FileName);
            model.Url_imagen.SaveAs(PathImagen);

            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                CATEGORIA oCategoria = new CATEGORIA();
                
                oCategoria.NOMBRE = model.Nombre;
                oCategoria.URL_IMAGEN = model.Url_imagen.FileName;
                oCategoria.ESTADO = 1;

                db.CATEGORIA.Add(oCategoria);
                db.SaveChanges();

            }
            return Redirect(Url.Content("~/Admin/Categorias"));
        }

        [AuthorizeUser(idRol: 1)]
        public ActionResult EditarCategoria(int id)
        {
            EditCategoriaViewModel model = new EditCategoriaViewModel();

            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                var oCategoria = db.CATEGORIA.Find(id);
                model.id = oCategoria.ID_CATEGORIA;
                model.Nombre = oCategoria.NOMBRE;
                //model.Url_imagen = oCategoria.URL_IMAGEN;
            }

            return View(model);
        }
        [AuthorizeUser(idRol: 1)]
        [HttpPost]
        public ActionResult EditarCategoria(EditCategoriaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if(model.Url_imagen != null)
            {
                string tip = model.Url_imagen.ContentType.Substring(0, 5);
                if (tip != "image")
                {
                    ModelState.AddModelError("Url_imagen", "Solo puede ingresar imagenes");
                    return View(model);
                }
                else
                {
                    string RutaSitio = Server.MapPath("~/");
                    string PathImagen = Path.Combine(RutaSitio + "/img/" + model.Url_imagen.FileName);
                    model.Url_imagen.SaveAs(PathImagen);
                }
            }

            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                var oCategoria = db.CATEGORIA.Find(model.id);
                oCategoria.NOMBRE = model.Nombre;
                if(model.Url_imagen != null)
                    oCategoria.URL_IMAGEN = model.Url_imagen.FileName;

                db.Entry(oCategoria).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
                return Redirect(Url.Content("~/Admin/Categorias"));
        }
        [AuthorizeUser(idRol: 1)]
        [HttpPost]
        public ActionResult EliminarCategoria(int Id)
        {
            try
            {
                using (DB_MMCEntities1 db = new DB_MMCEntities1())
                {
                    var oCategoria = db.CATEGORIA.Find(Id);
                    oCategoria.ESTADO = 0;

                    db.Entry(oCategoria).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
            
        }
        #endregion
        [AuthorizeUser(idRol: 1)]
        public ActionResult Ventas()
        {
            List<VentasViewModel> ventas = null;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                ventas = (from d in db.VENTA
                          join u in db.USUARIO
                          on d.ID_USUARIO equals u.ID_USUARIO
                          where d.ESTADO != "Cancelado"
                          orderby d.ESTADO descending
                          orderby d.ID_VENTA descending
                          select new VentasViewModel
                          {
                              id_venta = d.ID_VENTA,
                              nombre_usuario = u.NOMBRES +" "+u.APELLIDOS,
                              correo = u.EMAIL,
                              fecha = d.FECHA_HORA,
                              total = d.TOTAL,
                              estado = d.ESTADO

                          }).ToList();
            }
            return View(ventas);
        }
        [AuthorizeUser(idRol: 1)]
        public ActionResult DetalleVenta(int id)
        {
            List<DetalleVentaViewModel> dv = null;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                dv = (from d in db.DETALLE_VENTA
                      join p in db.PRODUCTO
                      on d.ID_PRODUCTO equals p.ID_PRODUCTO
                      where d.ID_VENTA == id
                      select new DetalleVentaViewModel
                      {
                          id_venta = d.ID_VENTA,
                          id_producto = d.ID_PRODUCTO,
                          producto = p.NOMBRE_PRODUCTO,
                          cantidad = d.CANTIDAD,
                          subtotal = d.SUB_TOTAL
                      }).ToList();
            }



            return View(dv);
        }
        [AuthorizeUser(idRol: 1)]
        public ActionResult ConfirmarVenta(int id)
        {
            try
            {
                using (DB_MMCEntities1 db = new DB_MMCEntities1())
                {
                    db.sp_confirmarventa(id);

                }
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
            

        }
        [AuthorizeUser(idRol: 1)]
        public ActionResult CancelarVenta(int id)
        {
            try
            {
                using (DB_MMCEntities1 db = new DB_MMCEntities1())
                {
                    db.sp_cancelarventa(id);

                }
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
            

        }
        [AuthorizeUser(idRol: 1)]
        public ActionResult Pedidos()
        {
            List<PedidosViewModel> pedidos = null;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                pedidos = (from d in db.ENTREGA_PRODUCTO
                           join v in db.VENTA
                           on d.ID_VENTA equals v.ID_VENTA
                           join u in db.USUARIO
                           on v.ID_USUARIO equals u.ID_USUARIO
                           orderby d.ID_ENTREGA descending
                           select new PedidosViewModel
                           {
                               id_entrega = d.ID_ENTREGA,
                               id_venta = d.ID_VENTA,
                               direccion = u.DIRECCION,
                               nombre_cliente = u.NOMBRES + " " + u.APELLIDOS,
                               fecha_entrega = d.FECHA_ENTREGA.ToString(),
                               fecha_salida = d.FECHA_SALIDA.ToString()
                           }).ToList();
            }

            return View(pedidos);
        }
        [AuthorizeUser(idRol: 1)]
        public ActionResult FechaEntrega(int id)
        {
            try
            {
                var fecha = DateTime.Now;
                using (DB_MMCEntities1 db = new DB_MMCEntities1())
                {
                    db.sp_fechaentrega(id, fecha);

                }
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
            

        }
        [AuthorizeUser(idRol: 1)]
        public ActionResult FechaSalida(int id)
        {
            try
            {
                var fecha = DateTime.Now;
                using (DB_MMCEntities1 db = new DB_MMCEntities1())
                {
                    db.sp_fechasalida(id, fecha);

                }
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
            

        }
        #region List

        private List<SelectListItem> ObtenerCategoria()
        {
            List<CategoriaViewModel> lst1 = null;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                lst1 = (from d in db.CATEGORIA
                        where d.ESTADO == 1
                        select new CategoriaViewModel
                        {
                            id = d.ID_CATEGORIA,
                            nombre = d.NOMBRE
                        }).ToList();
            }
            List<SelectListItem> items1 = lst1.ConvertAll(d =>
            {
                return new SelectListItem()
                {
                    Text = d.nombre.ToString(),
                    Value = d.id.ToString(),
                    Selected = false
                };
            });

            return items1;
        }

        private List<SelectListItem> ObtenerCategoria(int id)
        {
            List<CategoriaViewModel> lst1 = null;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                lst1 = (from d in db.CATEGORIA
                        where d.ESTADO == 1
                        select new CategoriaViewModel
                        {
                            id = d.ID_CATEGORIA,
                            nombre = d.NOMBRE
                        }).ToList();
            }
            List<SelectListItem> items1 = lst1.ConvertAll(d =>
            {
                if(d.id == id)
                {
                    return new SelectListItem()
                    {
                        Text = d.nombre.ToString(),
                        Value = d.id.ToString(),
                        Selected = true
                    };
                }
                else
                {
                    return new SelectListItem()
                    {
                        Text = d.nombre.ToString(),
                        Value = d.id.ToString(),
                        Selected = false
                    };
                }
                
            });

            return items1;
        }

        private List<SelectListItem> ObtenerTemporada()
        {
            List<TemporadaViewModel> lst2 = null;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                lst2 = (from d in db.TEMPORADA
                        select new TemporadaViewModel
                        {
                            id_temporada = d.ID_TEMPORADA,
                            temporada = d.NOM_TEMPORADA
                        }).ToList();
            }
            List<SelectListItem> items2 = lst2.ConvertAll(d =>
            {
                return new SelectListItem()
                {
                    Text = d.temporada.ToString(),
                    Value = d.id_temporada.ToString(),
                    Selected = false
                };
            });

            return items2;
        }

        private List<SelectListItem> ObtenerTemporada(int id)
        {
            List<TemporadaViewModel> lst2 = null;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                lst2 = (from d in db.TEMPORADA
                        select new TemporadaViewModel
                        {
                            id_temporada = d.ID_TEMPORADA,
                            temporada = d.NOM_TEMPORADA
                        }).ToList();
            }
            List<SelectListItem> items2 = lst2.ConvertAll(d =>
            {
                if(d.id_temporada == id)
                {
                    return new SelectListItem()
                    {
                        Text = d.temporada.ToString(),
                        Value = d.id_temporada.ToString(),
                        Selected = true
                    };
                }
                else
                {
                    return new SelectListItem()
                    {
                        Text = d.temporada.ToString(),
                        Value = d.id_temporada.ToString(),
                        Selected = false
                    };
                }
                
            });

            return items2;
        }

        #endregion


    }
}