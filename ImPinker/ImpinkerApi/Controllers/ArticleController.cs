using System.Net.Http;
using ImBLL;
using ImModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class ArticleController : BaseApiController
    {

        private static readonly ArticleBll ArticleBll = new ArticleBll();
        //
        // GET: /Article/

        public string Index()
        {
            return string.Empty;
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetByPage(int pageNum, int pageSize)
        {
            var list = GetListsByPage(pageNum, pageSize);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    item.CoverImage = ImPinkerApi.Common.ImageUrlHelper.GetArticleCoverImage(item.CoverImage, 0);
                }
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = list,
                Description = "ok"
            }); 
        }

        private List<ArticleViewModel> GetListsByPage(int pageNum, int pageCount)
        {
            //如果是新用户，则推荐热门文章；老用户，则根据用户兴趣标签，智能推荐
            var list = new List<ArticleViewModel>();
            var userInterestKey = "";
            if (string.IsNullOrEmpty(userInterestKey))
            {
                list = ArticleBll.GetIndexListByPage(pageNum, pageCount);
            }
            else
            {
            }
            
            return list;
        }
    }
}
