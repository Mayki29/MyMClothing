using System.Web;
using System.Web.Mvc;

namespace MyMClothing
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new Filters.VerificaSesion());
        }
    }
}
