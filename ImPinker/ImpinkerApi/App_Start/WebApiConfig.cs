using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Cors;
using ImpinkerApi.Filters;

namespace ImpinkerApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //异常过滤器（Mvc的就直接在FilterConfig类的RegisterGlobalFilters方法中添加，Web Api的过滤器没有单独一个配置类，可以写在WebApiConfig类的Register中）
            config.Filters.Add(new ExceptionContextFilter());
            //跨域配置
            //config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // 取消注释下面的代码行可对具有 IQueryable 或 IQueryable<T> 返回类型的操作启用查询支持。
            // 若要避免处理意外查询或恶意查询，请使用 QueryableAttribute 上的验证设置来验证传入查询。
            // 有关详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=279712。
            //config.EnableQuerySupport();

            // 若要在应用程序中禁用跟踪，请注释掉或删除以下代码行
            // 有关详细信息，请参阅: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
