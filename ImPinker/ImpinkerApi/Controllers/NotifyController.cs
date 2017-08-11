using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;

namespace ImpinkerApi.Controllers
{
    public class NotifyController : Controller
    {
        NotifyBll _notifyBll=new NotifyBll();
        //
        // GET: /Notify/

        public ActionResult Index()
        {
            return null;
        }

    }
}
