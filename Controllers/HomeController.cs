using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using MyMClothing.Models;
using MyMClothing.Models.ObViewModels;

namespace MyMClothing.Controllers
{
    public class HomeController : Controller
    {
        private DB_MMCEntities1 db = new DB_MMCEntities1();
        public ActionResult Index()
        {
            List<ProductoViewModel> listaprod = null;
            using (db)
            {
                listaprod = (from d in db.PRODUCTO where d.ESTADO == 1 orderby d.ID_PRODUCTO descending
                             select new ProductoViewModel
                             {
                                 id = d.ID_PRODUCTO,
                                 nombreProd = d.NOMBRE_PRODUCTO,
                                 precio_venta = d.PRECIO_VENTA,
                                 prom = d.PROMEDIO_RESENA,
                                 img = d.URL_IMAGEN,
                                 descuento = d.DESCUENTO

                             }).Take(12).ToList();
            }

            
            return View(listaprod);
        }

        public ActionResult Catalogo()
        {
            List<CategoriaViewModel> listaCat = null;
            using (db)
            {
                listaCat = (from d in db.CATEGORIA
                            where d.ESTADO == 1
                            select new CategoriaViewModel
                            {
                                id = d.ID_CATEGORIA,
                                nombre = d.NOMBRE,
                                url_imagen = d.URL_IMAGEN
                            }).ToList();
            }
            return View(listaCat);
        }
        public ActionResult Descuento()
        {
            List<ProductoViewModel> listaprod = null;
            using (db)
            {
                listaprod = (from d in db.PRODUCTO
                             where d.ESTADO == 1
                             where d.DESCUENTO != null
                             select new ProductoViewModel
                             {
                                 id = d.ID_PRODUCTO,
                                 nombreProd = d.NOMBRE_PRODUCTO,
                                 precio_venta = d.PRECIO_VENTA,
                                 prom = d.PROMEDIO_RESENA,
                                 img = d.URL_IMAGEN,
                                 descuento = d.DESCUENTO

                             }).ToList();
            }
            return View(listaprod);
        }
        public ActionResult Nosotros()
        {
            return View();
        }
        public ActionResult Producto(int? id, int? cantidad)
        {
            if(id == null)
            {
                return Redirect(Url.Content("~/Home/Index"));
            }
            if(cantidad != null)
            {
                List<CarritoViewModel> idProductos;
                if (Session["Productos"] == null)
                {
                    idProductos = new List<CarritoViewModel>();
                }
                else
                {
                    idProductos = Session["Productos"] as List<CarritoViewModel>;
                }
                CarritoViewModel prod = new CarritoViewModel { id = id, cantidad = cantidad};
                idProductos.Add(prod);
                Session["Productos"] = idProductos;
            }
            

            ProductoViewModel producto = null;
            using (db)
            {
                producto = (from d in db.PRODUCTO
                            where d.ID_PRODUCTO == id
                            select new ProductoViewModel
                            {
                                id = d.ID_PRODUCTO,
                                nombreProd = d.NOMBRE_PRODUCTO,
                                precio_venta = (decimal)d.PRECIO_VENTA,
                                talla = d.TALLA,
                                prom = d.PROMEDIO_RESENA,
                                descripcion = d.DESCRIPCION,
                                img = d.URL_IMAGEN,
                                descuento = d.DESCUENTO,
                                cantidad = d.STOCK_TOTAL 
                            }).FirstOrDefault();
            }
            if(producto == null)
            {
                return Redirect(Url.Content("~/Home/Index"));
            }
            ViewBag.Carrito = Session["Productos"];
            return View(producto);

        }
        public ActionResult Carrito(int? id)
        {
            if(id != null)
            {
                List<CarritoViewModel> lista = (List<CarritoViewModel>)Session["Productos"];
                lista.Remove(lista.Find(c => c.id == id));
                if (lista.Count() == 0)
                {
                    Session["Productos"] = null;
                }
                else
                {
                    Session["Productos"] = lista;
                }
            }
            if(Session["Productos"] == null)
            {
                return View();
            }
            else
            {
                List<CarritoViewModel> lista = (List<CarritoViewModel>)Session["Productos"];
                List<ProductoViewModel> producto = null;
                List<int?> idp = new List<int?>();
                foreach (var i in lista)
                {
                    idp.Add(i.id);
                    
                }
                List<ProductoViewModel> productoasdasd = (from d in db.PRODUCTO
                                                          where idp.Contains(d.ID_PRODUCTO)
                                                          select new ProductoViewModel
                                                          {
                                                              id = d.ID_PRODUCTO,
                                                              nombreProd = d.NOMBRE_PRODUCTO,
                                                              precio_venta = (decimal)d.PRECIO_VENTA,
                                                              talla = d.TALLA,
                                                              prom = d.PROMEDIO_RESENA,
                                                              descripcion = d.DESCRIPCION,
                                                              img = d.URL_IMAGEN,
                                                              descuento = d.DESCUENTO,
                                                          }).ToList();
                productoasdasd.ForEach(x => x.cantidad = lista.Find(s => s.id == x.id).cantidad.Value);
                producto = productoasdasd;
                return View(producto);
            }

            
        }

        public ActionResult Comprar()
        {
            try
            {
                List<CarritoViewModel> listaprod = (List<CarritoViewModel>)Session["Productos"];
                var fecha = DateTime.Now;
                USUARIO user = (USUARIO)Session["User"];
                var iduser = user.ID_USUARIO;

                using (db)
                {
                    db.sp_generarventa(iduser, fecha);
                    foreach (var p in listaprod)
                    {
                        db.sp_insertdetalleventa(iduser, fecha, p.id, p.cantidad);
                    }

                }
                Session["Productos"] = null;
                SendEmailComp(user.EMAIL, fecha);
                

                return Content("1");
            }
            catch
            {
                return Content("0");
            }
            
        }
        public ActionResult Categoria(int id)
        {
            List<ProductoViewModel> listaprod = null;
            using (db)
            {
                listaprod = (from d in db.PRODUCTO
                             where d.ESTADO == 1
                             where d.ID_CATEGORIA == id
                             select new ProductoViewModel
                             {
                                 id = d.ID_PRODUCTO,
                                 nombreProd = d.NOMBRE_PRODUCTO,
                                 precio_venta = d.PRECIO_VENTA,
                                 prom = d.PROMEDIO_RESENA,
                                 img = d.URL_IMAGEN,
                                 descuento = d.DESCUENTO

                             }).ToList();
            }
            return View(listaprod);
        }


        public ActionResult CatalogoCompleto()
        {
            List<ProductoViewModel> listaprod = null;
            using (db)
            {
                listaprod = (from d in db.PRODUCTO
                             where d.ESTADO == 1
                             orderby d.ID_PRODUCTO descending
                             select new ProductoViewModel
                             {
                                 id = d.ID_PRODUCTO,
                                 nombreProd = d.NOMBRE_PRODUCTO,
                                 precio_venta = d.PRECIO_VENTA,
                                 prom = d.PROMEDIO_RESENA,
                                 img = d.URL_IMAGEN,
                                 descuento = d.DESCUENTO

                             }).ToList();
            }


            return View(listaprod);
        }

        public ActionResult Perfil()
        {
            USUARIO user = (USUARIO)Session["User"];
            

            if (user == null)
            {
                return Redirect(Url.Content("~/Home/Index"));
            }
            var id = user.ID_USUARIO;
            UserViewModelPer oUser = null;
            using (db)
            {
                oUser = (from d in db.USUARIO
                         where d.ID_USUARIO == id
                         select new UserViewModelPer
                         {
                             id = d.ID_USUARIO,
                             Nombres = d.NOMBRES,
                             Apellidos = d.APELLIDOS,
                             Email = d.EMAIL,
                             Dni = d.DNI,
                             Edad = d.EDAD,
                             Direccion = d.DIRECCION
                         }).SingleOrDefault();
            }
            if(oUser == null)
            {
                return Redirect(Url.Content("~/Home/Index"));
            }
            return View(oUser);
        }
        [HttpPost]
        public ActionResult Perfil(UserViewModelPer model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorAc = "No se pudo actualizar, verfique los datos ingresados y vuelva a intentarlo";
                return View(model);
            }
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                var oUser = db.USUARIO.Find(model.id);
                oUser.NOMBRES = model.Nombres;
                oUser.APELLIDOS = model.Apellidos;
                oUser.EDAD = model.Edad;
                oUser.DIRECCION = model.Direccion;
                oUser.DNI = model.Dni;
                oUser.EMAIL = model.Email;
                db.Entry(oUser).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Redirect(Url.Content("~/Home/Perfil"));
        }

        public ActionResult AgregarProductos(int? id)
        {
            if(id != null)
            {
                List<int> idProductos;
                if (Session["Productos"] == null)
                {
                    idProductos = new List<int>();
                }
                else
                {
                    idProductos = Session["Productos"] as List<int>;
                }
                idProductos.Add(id.Value);
                Session["Productos"] = idProductos;
            }
            ViewBag.Carrito = Session["Productos"];
            return View("Index");
        }

        public ActionResult Calificacion(int calificacion, int idproducto, int iduser)
        {
            try
            {
                using (db)
                {
                    var res = db.sp_resena(calificacion, idproducto, iduser).SingleOrDefault();
                    if (res == 0)
                    {
                        return Content("Se actualizó correctamente su calificacion");
                    }
                    else
                    {
                        return Content("Se envió correctamente su calificacion");
                    }
                    
                }
            }
            catch(Exception e)
            {
                return Content("Ocurrio un error");
            }
            
        }




        private void SendEmailComp(string EmailDestino, DateTime fecha)
        {
            string EmailOrigen = "mymclothingproj@gmail.com";
            string Contraseña = "MMClothing2021";
            USUARIO user = (USUARIO)Session["User"];
            var iduser = user.ID_USUARIO;
            string listasend = "";
            decimal total = 0;
            using (DB_MMCEntities1 dbb = new DB_MMCEntities1())
            {
                var prods = dbb.sp_emailproductos(iduser, fecha).ToList();
                foreach (var p in prods)
                {
                    listasend = listasend + "<p><span style='margin-right:2rem'>Producto: " + p.NOMBRE_PRODUCTO + "</span><span style='margin-right:2rem'>Cantidad: " + p.CANTIDAD+"</span><span>SubTotal: S/" + p.SUB_TOTAL + "</span></p><br>";

                    total += p.SUB_TOTAL;
                }
            }
                

            MailMessage oMailMessage = new MailMessage(EmailOrigen, EmailDestino, "Pedido realizado en M&M Clothing", //asusnto
                                                                                    "<p>Los productos pedidos son: </p><br>"+listasend+"<br><h4>Total: S/"+ total + "</h4>" //cuerpo
                                                                                     );

            oMailMessage.IsBodyHtml = true;

            SmtpClient oSmtpClient = new SmtpClient("smtFp.gmail.com");
            oSmtpClient.EnableSsl = true;
            oSmtpClient.UseDefaultCredentials = false;
            oSmtpClient.Port = 587;//puerto que dice gmail por el cual enviar los correos
            oSmtpClient.Credentials = new System.Net.NetworkCredential(EmailOrigen, Contraseña);

            oSmtpClient.Send(oMailMessage);

            oSmtpClient.Dispose();


        }
    }
}