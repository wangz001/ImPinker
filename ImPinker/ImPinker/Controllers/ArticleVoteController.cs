using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel;
using ImModel.ViewModel;
using ImPinker.Filters;
using ImPinker.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class ArticleVoteController : Controller
    {
        private static readonly UserBll UserBll = new UserBll();
        private static readonly ArticleVoteBll ArticleVoteBll = new ArticleVoteBll();
        private static readonly ArticleCommentBll ArticleCommentBll = new ArticleCommentBll();
        private static readonly ArticleCommentVoteBll ArticleCommentVoteBll = new ArticleCommentVoteBll();

        
        #region 文章点赞
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

            if (articleId > 0)
            {
                var model = new ArticleVote()
                {
                    ArticleId = articleId,
                    UserId = userId,
                    Vote = vote > 0
                };
                return ArticleVoteBll.AddVote(model) ? "success" : "error";
            }
            return "success";
        }

        #endregion 

        #region 文章评论

        /// <summary>
        /// 文章评论
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ArticleComment(ArticleViewModel articleViewModel)
        {
            int totalCount = 0;
            var articleId = long.Parse(articleViewModel.Id);
            var commentLists = ArticleCommentBll.GetCommentsWithToComments(articleId, 1, 20,out totalCount);
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
            return PartialView("ArticleComment");
        }

        /// <summary>
        /// 提交评论
        /// </summary>
        /// <returns></returns>
        [AuthorizationFilter]
        [HttpPost]
        public string ArticleCommentSubmit(int articleId, int commentid, string content)
        {
            Response.ContentType = "application/json; charset=utf-8";
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
                        ToCommentId = commentid,
                        CreateTime = DateTime.Now,
                    };
                    var flag = ArticleCommentBll.Add(model);
                    if (flag)
                    {
                        return JsonConvert.SerializeObject(new AjaxReturnViewModel
                            {
                                IsSuccess = 1,
                                Description = "该手机号码已被注册，请直接登录",
                                Data = ""
                            });
                    }
                    else
                    {
                        return JsonConvert.SerializeObject(new AjaxReturnViewModel
                                {
                                    IsSuccess = 0,
                                    Description = "该手机号码已被注册，请直接登录",
                                    Data = ""
                                });
                    }
                }
                return JsonConvert.SerializeObject(new AjaxReturnViewModel
                {
                    IsSuccess = 1,
                    Description = "该手机号码已被注册，请直接登录",
                    Data = ""
                });
            }
            return JsonConvert.SerializeObject(new AjaxReturnViewModel
            {
                IsSuccess = 0,
                Description = "该手机号码已被注册，请直接登录",
                Data = ""
            });
        }

        #endregion

        #region 给文章的评论点赞
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult ArticleCommentVote(int commentId,int vote)
        {
            var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;

            if (commentId > 0)
            {
                var model = new ArticleCommentVote()
                {
                    ArticleCommentId = commentId,
                    UserId = userId,
                    Vote = vote > 0,
                    CreateTime=DateTime.Now,
                    UpdateTime=DateTime.Now
                };
                return ArticleCommentVoteBll.AddVote(model) ? Json("success") : Json("error");
            }
            return Json("success");
        }

        #endregion
    }
}