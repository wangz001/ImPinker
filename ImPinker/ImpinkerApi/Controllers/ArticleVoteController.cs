﻿using System;
using System.Collections.Generic;
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
        readonly UserBll _userBll = new UserBll();
        

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

        #region 获取文章评论
        [HttpGet]
        public HttpResponseMessage GetArticleComments(long articleid,int pagenum,int pagesize)
        {
            int totalCount;
            var commentLists = _articleCommentBll.GetCommentsWithToComments(articleid, pagenum, pagesize, out totalCount);
            foreach (var articleCommentVm in commentLists)
            {//头像规格。100
                articleCommentVm.HeadImage = ImageUrlHelper.GetHeadImageUrl(articleCommentVm.HeadImage, 100);
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = commentLists,
                Description = "获取评论成功"
            });
        }

        #endregion

    }
}
