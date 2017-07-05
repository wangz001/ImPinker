using ImBLL;
using ImpinkerApi.Common;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ImpinkerApi.Controllers
{
    public class WeiBoVoteController : BaseApiController
    {
        WeiBoVoteBll _weiboVoteBll = new WeiBoVoteBll();
        WeiBoCommentBll _weiboCommentBll = new WeiBoCommentBll();
        #region 点赞操作
        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [TokenCheck]
        [HttpPost]
        public HttpResponseMessage NewWeiBoVote([FromBody]WeiBoVoteViewModel vm)
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            bool flag = _weiboVoteBll.AddWeiBoVote(vm.WeiBoId, userinfo.Id);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = flag? 1:0,
                Data = null,
                Description = ""
            }); 
        }
        /// <summary>
        /// 取消赞
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [TokenCheck]
        [HttpPost]
        public HttpResponseMessage CancelWeiBoVote([FromBody]WeiBoVoteViewModel vm)
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            bool flag = _weiboVoteBll.CancelWeiboVote(vm.WeiBoId, userinfo.Id);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = flag ? 1 : 0,
                Data = null,
                Description = ""
            });
        }
        #endregion

        #region 评论操作
        [TokenCheck]
        [HttpPost]
        public HttpResponseMessage NewWeiBoComment([FromBody]WeiBoCommentViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.CommentStr) || vm.WeiBoId == 0)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Data = null,
                    Description = "评论内容不能为空"
                });
            }
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            bool flag = _weiboCommentBll.AddComment(vm.WeiBoId,vm.CommentStr,vm.ToCommentId, userinfo.Id);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = flag ? 1 : 0,
                Data = vm,
                Description = ""
            });
        }


        #endregion 

        #region 获取评论列表
        [HttpGet]
        public HttpResponseMessage GetWeiboCommentList(int weiboid)
        {
            if (weiboid > 0)
            {
                var list = _weiboCommentBll.GetList(weiboid,1,10);
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = list.Count > 0 ? 1 : 0,
                    Data = list,
                    Description = "获取评论成功"
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess =  0,
                Data = null,
                Description = "获取评论成功"
            });
        }

        #endregion
    }
}
