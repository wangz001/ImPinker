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
        readonly ArticleCommentBll _articleCommentBll =new ArticleCommentBll();
        

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
            if (string.IsNullOrEmpty(vm.CommentStr)||vm.ArticleId==0)
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
                ToCommentId = 0,
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

        #region 文章收藏
        /// <summary>
        /// 用户收藏文章
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TokenCheck]
        public HttpResponseMessage NewArticleCollect(long articleId)
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var flag = new ArticleCollectionBll().AddCollect(articleId, userinfo.Id);
            if (flag)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = null,
                    Description = "收藏成功"
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "没有更多数据"
            });
        }
        /// <summary>
        /// 取消收藏文章
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpGet]
        [TokenCheck]
        public HttpResponseMessage CancelArticleCollect(long articleId)
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var flag = new ArticleCollectionBll().RemoveCollect(articleId, userinfo.Id);
            if (flag)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = null,
                    Description = "取消收藏成功"
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "没有更多数据"
            });
        }

        #endregion
    }
}
