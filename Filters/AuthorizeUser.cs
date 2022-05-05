using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMClothing.Models;

namespace MyMClothing.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeUser : AuthorizeAttribute
    {
        private USUARIO oUser;
        private DB_MMCEntities1 db = new DB_MMCEntities1();
        private int idRol;

        public AuthorizeUser(int idRol = 0)
        {
            this.idRol = idRol;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            try
            {
                oUser = (USUARIO)HttpContext.Current.Session["User"];
                var rol = from m in db.ROLES where m.ID_ROL == oUser.ID_ROL select m;

                if(oUser.ID_ROL != idRol)
                {
                    filterContext.HttpContext.Response.Redirect("~/Home/Index");
                }
            }
            catch
            {
                filterContext.HttpContext.Response.Redirect("~/Home/Index");
            }


            //base.OnAuthorization(filterContext);
        }
        
    }
}