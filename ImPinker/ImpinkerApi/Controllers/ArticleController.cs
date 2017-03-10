using System.Net.Http;
using ImBLL;
using System.Web.Mvc;
using ImpinkerApi.Common;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class ArticleController : BaseApiController
    {

        private static readonly ArticleBll ArticleBll = new ArticleBll();

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetByPage(int pageNum, int pageSize)
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var userid = 0;
            if (userinfo!=null&&userinfo.Id>0)
            {
                userid = userinfo.Id;
            }
            var list = ArticleBll.GetListByPage(pageNum, pageSize, userid);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    item.CoverImage = ImPinkerApi.Common.ImageUrlHelper.GetArticleCoverImage(item.CoverImage, 0);
                }
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = list,
                    Description = "ok"
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "没有更多数据"
            });
        }

    }
}
