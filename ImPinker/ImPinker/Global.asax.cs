using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ImModel.ViewModel;
using ImPinker.App_Start;

namespace ImPinker
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            //日志
            log4net.Config.XmlConfigurator.Configure();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            string solrServer = ConfigurationManager.AppSettings.Get("SolrServer");
            SolrNet.Startup.Init<ArticleViewModel>(solrServer + "impinker");
            SolrNet.Startup.Init<WeiboVm>(solrServer + "impinker-weibo");
            
        }
    }
}
