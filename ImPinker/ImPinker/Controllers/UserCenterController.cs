using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel;
using ImModel.ViewModel;
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
            var totalCount = 0;
            var articles = ArticleBll.GetMyListByPage(user.Id, 1, 3, out totalCount);
            ViewBag.User = user;
            ViewBag.Articles = articles;
            ViewBag.totalCount = totalCount;
            return View();
        }

        /// <summary>
        /// 账号设置
        /// </summary>
        /// <returns></returns>
        [AuthorizationFilter]
        public ActionResult UserSetting()
        {
            var user = UserBll.GetModelByAspNetId(User.Identity.GetUserId());
            ViewBag.User = user;
            return View();
        }
        /// <summary>
        /// 用户修改显示昵称
        /// </summary>
        /// <param name="showname"></param>
        /// <returns></returns>
        [AuthorizationFilter]
        public ActionResult UpdateShowName(string showname)
        {
            var user = UserBll.GetModelByAspNetId(User.Identity.GetUserId());
            if (!string.IsNullOrEmpty(showname))
            {
                user.ShowName = showname;
                var falg = UserBll.Update(user);
                if (falg)
                {
                    return RedirectToAction("UserSetting");
                }
            }
            return View("Index");
        }

        /// <summary>
        /// 文章详情页，该文章作者信息
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ArticleWriterInfo(ArticleViewModel articleViewModel)
        {
            int totalCount = 0;
            var articleId = articleViewModel.Id;
            var userid = articleViewModel.Userid;
            var user = UserBll.GetModelByCache(Int32.Parse(userid));
            ViewBag.Writer = user;
            return PartialView();
        }

    }
}