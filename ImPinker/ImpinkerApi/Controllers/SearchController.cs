using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using ImBLL;
using ImModel.Enum;
using ImModel.ViewModel;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class SearchController : BaseApiController
    {
        /// <summary>
        /// 微博，根据地理位置查询
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="weiboId"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="distance"></param>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetWeiboByGeo(int userid,int weiboId,string lat,string lng,int distance,int pagenum,int pagesize)
        {
            pagenum = pagenum > 0 ? pagenum : 1;
            pagesize = pagesize > 0 ? pagesize : 10;
            double dLat;
            double dLng;
            double.TryParse(lat, out dLat);
            double.TryParse(lng, out dLng);
            if (!string.IsNullOrEmpty(lat)&&!string.IsNullOrEmpty(lng)&&dLat>0&&dLng>0)
            {
                var searchVm = SolrNetSearchBll.QueryWeiboByGeo(dLat, dLng, distance, userid,pagenum,pagesize);
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = searchVm,
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "参数错误"
            });
        }

        /// <summary>
        /// 根据关键字搜索微博
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="keyWord"></param>
        /// <param name="pageNum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SearchWeibo(int userid, string keyWord, int pageNum, int pagesize)
        {
            var list = SolrNetSearchBll.QueryWeiboByKeyword(keyWord, pageNum, pagesize, true);

            return GetJson(new JsonResultViewModel
            {
                IsSuccess = (list!=null&&list.TotalCount>0) ?1:0,
                Data = list,
                Description = "查询文章"
            });
        }

        /// <summary>
        /// 根据关键字搜索
        /// </summary>
        /// <param name="userid">如果是0，表示用户未登录</param>
        /// <param name="keyWord">关键字</param>
        /// <param name="pageNum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SearchArticle(int userid, string keyWord, int pageNum, int pagesize)
        {
            var list = SolrNetSearchBll.QueryArticleByKeyword(keyWord, pageNum, pagesize, true);

            return GetJson(new JsonResultViewModel
            {
                IsSuccess = (list != null && list.TotalCount > 0) ? 1 : 0,
                Data = list,
                Description = "查询文章"
            });
        }
        /// <summary>
        /// 关键字查询，包括文章和微博
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="keyWord"></param>
        /// <param name="pageNum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SearchByKeyword(int userid, string keyWord, int pageNum, int pagesize)
        {
            var articleList = SolrNetSearchBll.QueryArticleByKeyword(keyWord, pageNum, pagesize, true);
            var weiboList = SolrNetSearchBll.QueryWeiboByKeyword(keyWord, pageNum, pagesize, true);
            var resultList = new List<ArticleWeiboVm>();
            foreach (var articleVm in articleList.ArticleList)
            {
                resultList.Add(new ArticleWeiboVm
                {
                    ArticleVm = articleVm,
                    CreateTime = articleVm.CreateTime,
                    EntityId = 0,
                    EntityType = (int)EntityTypeEnum.Article,
                    Userid = Int32.Parse(articleVm.Userid)
                });
            }
            foreach (var weiboVm in weiboList.WeiboList)
            {
                resultList.Add(new ArticleWeiboVm
                {
                    WeiboVm = weiboVm,
                    CreateTime = weiboVm.CreateTime,
                    EntityId = (int)weiboVm.Id,
                    EntityType = (int)EntityTypeEnum.Article,
                    Userid = weiboVm.UserId
                });
            }
            resultList = resultList.OrderByDescending(m => m.CreateTime).ToList();
            return GetJson(new JsonResultViewModel
            {
                IsSuccess =resultList.Count>0 ? 1 : 0,
                Data = resultList,
                Description = "查询文章"
            });
        }

    }
}
