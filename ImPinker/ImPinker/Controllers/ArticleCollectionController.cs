using ImBLL;
using ImPinker.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using ImPinker.Models;

namespace ImPinker.Controllers
{
    /// <summary>
    /// 文章收藏操作类
    /// </summary>
    public class ArticleCollectionController : Controller
    {
        ArticleCollectionBll _articleCollectBll = new ArticleCollectionBll();
        private static readonly UserBll UserBll = new UserBll();
        // 我的收藏，首页
        // GET: /ArticleCollection/
        [AuthorizationFilter]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult AddCollect(long articleId)
        {
            var userid = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
            var flag = _articleCollectBll.AddCollect(articleId, userid);

            return Json(new AjaxReturnViewModel
            {
                IsSuccess = flag ? 1: 0,
                Description = ""
            });
        }
        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [AuthorizationFilter]
        public ActionResult RemoveCollect(long articleId)
        {

            return Json("");
        }
    }
}