using System.Web;
using System.Web.Mvc;
using ImpinkerApi.Filters;

namespace ImpinkerApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}