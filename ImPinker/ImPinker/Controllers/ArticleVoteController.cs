using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using ImPinker.Filters;
using Microsoft.AspNet.Identity;
using Model;

namespace ImPinker.Controllers
{
    public class ArticleVoteController : Controller
    {
        private static readonly UserBll UserBll = new UserBll();
        private static readonly ArticleVoteBll ArticleVoteBll = new ArticleVoteBll();
         
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

	}
}