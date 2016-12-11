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
            var ds = ArticleBll.GetMyListByPage(user.Id, 1, 3);
            var articles = ArticleBll.DataTableToList(ds.Tables[0]);
            ViewBag.User = user;
           ViewBag.Articles = articles;
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