using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel;
using ImPinker.Filters;
using Microsoft.AspNet.Identity;

namespace ImPinker.Controllers
{
    public class ArticleVoteController : Controller
    {
        private static readonly UserBll UserBll = new UserBll();
        private static readonly ArticleVoteBll ArticleVoteBll = new ArticleVoteBll();
        private static readonly ArticleCommentBll ArticleCommentBll = new ArticleCommentBll();
         
        //
        // GET: /ArticleVote/
        public string Index()
        {
            return "";
        }
        [AuthorizationFilter]
        [HttpGet]
        public string UserVote(long articleId, int vote)
        {
            var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;

            if (articleId>0)
            {
                var model = new ArticleVote()
                {
                    ArticleId = articleId,
                    UserId = userId,
                    Vote = vote>0
                };
                return ArticleVoteBll.AddVote(model)?"success":"error";
            }
            return "success";
        }


        /// <summary>
        /// 文章评论
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ArticleComment()
        {
            return PartialView("ArticleComment");
        }

        /// <summary>
        /// 提交评论
        /// </summary>
        /// <returns></returns>
        [AuthorizationFilter]
        [HttpPost]
        public string ArticleCommentSubmit(int articleId,string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;

                if (articleId > 0)
                {
                    var model = new ArticleComment()
                    {
                        ArticleId = articleId,
                        UserId = userId,
                       Content = content,
                       CreateTime = DateTime.Now,
                    };
                    return ArticleCommentBll.Add(model) ? "success" : "error";
                }
                return "success"; 
            }
            return "error";
        }
	}
}