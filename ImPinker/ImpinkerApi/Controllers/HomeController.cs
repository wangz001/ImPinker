using Common.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImpinkerApi.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ILog MLogger = LogManager.GetLogger("WebLogger");
        public ActionResult Index()
        {
            MLogger.Warn("测试记录日志");
            MLogger.Error("测试记录日志");
            return View();
        }
    }
}
