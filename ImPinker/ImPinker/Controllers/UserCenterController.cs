using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class UserCenterController : Controller
    {
        private static readonly ArticleBll ArticleBll = new ArticleBll();
        private static readonly UserBll UserBll = new UserBll();
        //个人中心首页
        // GET: /UserCenter/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyArticles()
        {
            var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
            var ds = ArticleBll.GetMyListByPage(userId, 1, 10);
            var articles = ArticleBll.DataTableToList(ds.Tables[0]);
            ViewBag.jsonData = JsonConvert.SerializeObject(articles);
            ViewBag.pageCount = 10;
            return View();
        }

        public ActionResult UserSetting()
        {
            return View();
        }
	}
}