using System;
using System.Net.Http;
using System.Web.Http;
using ImBLL;
using ImModel;
using ImpinkerApi.Common;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class ArticleVoteController : BaseApiController
    {
        readonly ArticleCommentBll _articleCommentBll = new ArticleCommentBll();
        readonly ArticleVoteBll _articleVoteBll = new ArticleVoteBll();

        #region 文章评论
        /// <summary>
        /// 评论
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        [TokenCheck]
        public HttpResponseMessage NewArticleComment([FromBody]NewArticleCommentVm vm)
        {
            if (string.IsNullOrEmpty(vm.CommentStr) || vm.ArticleId == 0)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Data = null,
                    Description = "评论内容不能为空"
                });
            }
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var model = new ArticleComment
            {
                ArticleId = vm.ArticleId,
                Content = vm.CommentStr,
                UserId = userinfo.Id,
                ToCommentId = (int)vm.ToCommentId,
                CreateTime = DateTime.Now
            };
            var flag = _articleCommentBll.Add(model);
            if (flag)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = null,
                    Description = "评论成功"
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "评论失败"
            });
        }

        #endregion

        #region 获取文章评论
        [HttpGet]
        public HttpResponseMessage GetArticleComments(long articleid, int pagenum, int pagesize)
        {
            int totalCount;
            var commentLists = _articleCommentBll.GetCommentsWithToComments(articleid, pagenum, pagesize, out totalCount);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = commentLists,
                Description = "获取评论成功"
            });
        }
        #endregion

        #region 文章点赞
        [HttpPost]
        [TokenCheck]
        public HttpResponseMessage NewArticleVote([FromBody]NewArticleCommentVm vm)
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var voteModel = new ArticleVote
            {
                ArticleId = vm.ArticleId,
                UserId = userinfo.Id,
                Vote=true
            };
            if (_articleVoteBll.Exists(voteModel.ArticleId, voteModel.UserId))
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess =  1 ,
                    Data = voteModel,
                    Description = "您已赞过"
                });
            }
            var flag = _articleVoteBll.AddVote(voteModel);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = flag? 1:0,
                Data = null,
                Description = "点赞"
            });
        }

        #endregion
    }
}
