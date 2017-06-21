using ImBLL;
using ImModel;
using ImModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImpinkerMobile.Controllers
{
    public class ArticleCommentController : Controller
    {
        private static readonly UserBll UserBll = new UserBll();
        private static readonly ArticleVoteBll ArticleVoteBll = new ArticleVoteBll();
        private static readonly ArticleCommentBll ArticleCommentBll = new ArticleCommentBll();
        private static readonly ArticleCommentVoteBll ArticleCommentVoteBll = new ArticleCommentVoteBll();
        //
        // GET: /ArticleComment/
        [ChildActionOnly]
        public ActionResult ArticleComment(ArticleViewModel articleViewModel)
        {
            int totalCount = 0;
            var articleId = long.Parse(articleViewModel.Id);
            var commentLists = ArticleCommentBll.GetCommentsWithToComments(articleId, 1, 20, out totalCount);

            var usersDic = new Dictionary<int, Users>();
            foreach (var articleComment in commentLists)
            {
                var userId = articleComment.UserId;
                var user = UserBll.GetModelByCache(userId);
                if (!usersDic.ContainsKey(userId))
                {
                    usersDic.Add(userId, user);
                }
            }
            ViewBag.CommentLists = commentLists;
            ViewBag.UsersDic = usersDic;
            ViewBag.TotalCount = totalCount;
            return PartialView("CommentPartial");
        }

        
    }
}
