using System.Net.Http;
using System.Web.Http;
using ImBLL;
using ImpinkerApi.Common;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;
using ImModel;
using ImModel.Enum;

namespace ImpinkerApi.Controllers
{
    /// <summary>
    /// 文章收藏操作
    /// </summary>
    public class UserCollectionController : BaseApiController
    {
        readonly UserCollectionBll _articleCollectBll = new UserCollectionBll();
        
        /// <summary>
        /// 我的收藏
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TokenCheck]
        public HttpResponseMessage GetMyCollect(int pageNum, int pageSize)
        {
            var userid = TokenHelper.GetUserInfoByHeader(Request.Headers).Id;
            int totalCount;
            var lists = _articleCollectBll.GetMyListByPage(userid, pageNum, pageSize, out totalCount);
            foreach (var article in lists)
            {
                article.CoverImage = ImageUrlHelper.GetArticleImage(article.Company, 360);
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = lists,
                Description = "请求成功"
            });
        }
        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpGet]
        [TokenCheck]
        public HttpResponseMessage AddWeiboCollect(long weiboId)
        {
            var userid = TokenHelper.GetUserInfoByHeader(Request.Headers).Id;
            var flag = _articleCollectBll.AddCollect(weiboId, userid,EntityTypeEnum.Weibo);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = flag ? 1 : 0,
                Data = weiboId,
                Description = "请求成功"
            });
        }
        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [HttpGet]
        [TokenCheck]
        public HttpResponseMessage RemoveCollect(long articleId)
        {
            var userid = TokenHelper.GetUserInfoByHeader(Request.Headers).Id;
            bool flag = _articleCollectBll.RemoveCollect(articleId, userid,EntityTypeEnum.Weibo);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = flag ? 1:0,
                Data = articleId,
                Description = "请求成功"
            });
        }

    }
}
