using System.Net.Http;
using System.Web.Mvc;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class ArticleVoteController : BaseApiController
    {
        //
        // GET: /ArticleVote/

        public HttpResponseMessage Index()
        {

            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "没有更多数据"
            });
        }
        #region 文章点赞
        
        

        #endregion

        #region 文章评论

        
        #endregion

        #region 文章收藏
        /// <summary>
        /// 用户收藏文章
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [TokenCheck]
        public HttpResponseMessage NewArticleCollect(int articleId)
        {
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
