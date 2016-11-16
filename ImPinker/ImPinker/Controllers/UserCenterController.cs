using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImPinker.Filters;
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
       [AuthorizationFilter]
        public ActionResult Index()
        {
            var user = UserBll.GetModelByAspNetId(User.Identity.GetUserId());
            ViewBag.User = user;
            return View();
        }
        
        /// <summary>
        /// 账号设置
        /// </summary>
        /// <returns></returns>
        [AuthorizationFilter]
        public ActionResult UserSetting()
        {
            var user= UserBll.GetModelByAspNetId(User.Identity.GetUserId());
            ViewBag.User = user;
            return View();
        }
	}
}