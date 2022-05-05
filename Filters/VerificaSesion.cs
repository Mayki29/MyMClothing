using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMClothing.Controllers;
using MyMClothing.Models;

namespace MyMClothing.Filters
{
    public class VerificaSesion : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var oUser = (USUARIO)HttpContext.Current.Session["User"];

            //if(oUser == null)
            //{
            //    if(filterContext.Controller is AccesController == false || filterContext.Controller is AdminController == true)
            //    {
            //        filterContext.HttpContext.Response.Redirect("~/Acces/Index");
            //    }
            //}
            //else
            //{
            //    if (filterContext.Controller is AccesController == true)
            //    {
            //        filterContext.HttpContext.Response.Redirect("~/Home/Index");
            //    }
            //}

            if(oUser != null)
            {
                if(filterContext.Controller is AccesController == true)
                {
                    filterContext.HttpContext.Response.Redirect("~/Home/Index");
                }
            }
             base.OnActionExecuting(filterContext);  
        }
    }
}