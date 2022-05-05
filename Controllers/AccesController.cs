using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MyMClothing.Models;
using MyMClothing.Models.ObViewModels;

namespace MyMClothing.Controllers
{
    public class AccesController : Controller
    {

        string urlDomain = "https://mymclothing.azurewebsites.net/";
        DB_MMCEntities1 db = new DB_MMCEntities1();
        // GET: Acces
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IniciarSesion(string correo, string password) //el nombre de los parametros debe ser igual al name de los input
        {
            try
            {
                var passwordencript = GetSha256(password);
                using (DB_MMCEntities1 db = new DB_MMCEntities1())
                {
                    var lst = (from d in db.USUARIO
                              where d.EMAIL == correo && d.PASSWORD == passwordencript
                              select d).FirstOrDefault(); //Devuelve el primero o si no hay devuelve null
                    if (lst == null)
                    {
                        return Content("Usuario invalido vuelva a intentarlo");
                    }
                    Session["User"] = lst; //creacion de sesion //
                    return Content("correcto");
                }
            }
            catch (Exception ex)
            {
                return Content("Ocurrio un error" + ex.Message);
            }
        }

        public ActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrarse(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); //al devolver el modelo regresamos a la vista con los datos que se llenaron por los helpers
            }

            var email = from d in db.USUARIO
                      where d.EMAIL == model.Email
                      select d;
            var dni = from e in db.USUARIO
                        where e.DNI == model.Dni
                        select e;
            if (dni.Count() > 0)
            {
                ModelState.AddModelError("Dni", "Este DNI ya esta registrado");
                return View(model);
            }

            if (email.Count() > 0)
            {
                ModelState.AddModelError("Email", "Este correo electronico ya esta registrado");
                return View(model);
            }
            var passwordencript = GetSha256(model.Password);

            using (var db = new DB_MMCEntities1())
            {
                USUARIO oUser = new USUARIO();
                oUser.NOMBRES = model.Nombres;
                oUser.APELLIDOS = model.Apellidos;
                oUser.ID_ROL = 2;
                oUser.EMAIL = model.Email;
                oUser.EDAD = model.Edad;
                oUser.PASSWORD = passwordencript;
                oUser.DIRECCION = model.Direccion;
                oUser.DNI = model.Dni;
                //string format = "dd/MM/yyyy HH:mm:ss";
                //2021 - 01 - 21 00:00:00.000
                oUser.FECHA_CREACION = DateTime.Now;
                db.USUARIO.Add(oUser);

                db.SaveChanges();

                Session["User"] = oUser;

            }
            return Redirect(Url.Content("~/Home/"));
        }

        [HttpPost]
        public ActionResult startRecovery(string correorec )
        {

              try
                {
                    string token = GetSha256(Guid.NewGuid().ToString());
                    using (DB_MMCEntities1 db = new DB_MMCEntities1())
                    {
                        var lst = (from d in db.USUARIO
                                   where d.EMAIL == correorec
                                   select d).FirstOrDefault(); //Devuelve el primero o si no hay devuelve null
                        if (lst != null)
                        {
                            lst.TOKEN_RECOVERY = token;
                            db.Entry(lst).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            //enviar mail
                            SendEmail(lst.EMAIL, token);
                        }
                        else
                        {
                            return Content("0");
                        }

                    }
                    string confirm = "<div class='alert alert-success role='alert'>¡Te hemos enviado por correo el enlace para restablecer tu contraseña!</ div >";
                    return Content(confirm);     
                }
                catch (Exception ex)
                {
                    string error = "<div class='alert alert-danger role='alert'>Ha ocurrido un error</ div >";
                    return Content(error);
                }

        }
        [HttpGet]
        public ActionResult Recovery(string token)
        {
            Models.ObViewModels.RecoveryPasswordViewModel model = new Models.ObViewModels.RecoveryPasswordViewModel();
            model.token = token;
            using (DB_MMCEntities1 db = new DB_MMCEntities1())
            {
                if (model.token == null || model.token.Trim().Equals(""))
                {
                    return View("Index");
                }
                var oUser = db.USUARIO.Where(d => d.TOKEN_RECOVERY == model.token).FirstOrDefault();
                if (oUser == null)
                {
                    ViewBag.Error = "<div class='alert alert-danger' role='alert'>Ha expirado este token</ div >";
                    return View("Index");
                }
            }


            return View(model);
        }
        [HttpPost]
        public ActionResult Recovery(Models.ObViewModels.RecoveryPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var passwordencript = GetSha256(model.Password);
                using (DB_MMCEntities1 db = new DB_MMCEntities1())
                {
                    var oUser = db.USUARIO.Where(d => d.TOKEN_RECOVERY == model.token).FirstOrDefault();

                    if (oUser != null)
                    {
                        oUser.PASSWORD = passwordencript;
                        oUser.TOKEN_RECOVERY = null;
                        db.Entry(oUser).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            ViewBag.Message = "<div class='alert alert-success'  role='alert'>Se reestablecio correctemante</ div >";
            return View("Index");
        }

        #region HELPERS
        private string GetSha256(string str)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        private void SendEmail(string EmailDestino, string token)
        {
            string EmailOrigen = "mymclothingproj@gmail.com";
            string Contraseña = "dknfiftqvthjgclq";
            string url = urlDomain + "Acces/Recovery/?token=" + token;

            MailMessage oMailMessage = new MailMessage(EmailOrigen, EmailDestino, "Recuperacion de contraseña de M&M Clothing", //asusnto
                                                                                    "<p>Usted está recibiendo este correo electronico porque recibimos una solicitud de restableciemiento de contraseña para su cuenta</p><br>" + //cuerpo
                                                                                     "<a href='" + url + "'>Click para recuperar</a>");

            oMailMessage.IsBodyHtml = true;

            SmtpClient oSmtpClient = new SmtpClient();
            oSmtpClient.Host = "smtp.gmail.com";
            
            
            oSmtpClient.Port = 587;//puerto que dice gmail por el cual enviar los correos
            
            oSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            oSmtpClient.UseDefaultCredentials = false;
            NetworkCredential nc = new NetworkCredential(EmailOrigen, Contraseña);
            oSmtpClient.Credentials = nc;
            oSmtpClient.EnableSsl = true;

            oSmtpClient.Send(oMailMessage);
            
            oSmtpClient.Dispose();


        }

        

        #endregion

    }
}