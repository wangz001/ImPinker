using System.Web.Mvc;
using ImPinker.Filters;

namespace ImPinker.App_Start
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionContextFilter());
		}
	}
}
