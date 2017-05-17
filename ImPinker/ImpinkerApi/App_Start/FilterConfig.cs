using System.Web.Mvc;
using ImpinkerApi.Filters;

namespace ImpinkerApi.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}