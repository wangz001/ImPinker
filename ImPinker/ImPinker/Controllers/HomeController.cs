using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Common.DateTimeUtil;
using ImBLL;
using ImModel.ViewModel;
using ImPinker.Common;
using ImPinker.Filters;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ArticleBll ArticleBll = new ArticleBll();
        private const int IndexPageCount = 30;

        /// <summary>
        /// 首页
        /// </summary>默认缓存1分钟
        /// <returns></returns>
        [OutputCache(Duration = 1, VaryByParam = "*")]
        public ActionResult Index()
        {
            ViewBag.pageCount = IndexPageCount;
            ViewBag.DailyAdded = ArticleBll.GetRecordCount(string.Format(" CreateTime > '{0}' ", DateTime.Now.AddDays(-1).ToShortDateString()));//今日新增文章
            return View();
        }

        private List<ArticleViewModel> GetByPage(int pageNum, int pageCount)
        {
            //如果是新用户，则推荐热门文章；老用户，则根据用户兴趣标签，智能推荐
            List<ArticleViewModel> list;
            const string userInterestKey = "";
            if (string.IsNullOrEmpty(userInterestKey))
            {
                list = ArticleBll.GetIndexListByPage(pageNum, pageCount);
            }
            else
            {
                list = SolrNetSearchBll.Query(userInterestKey, "", "", "", "", pageNum, pageCount).ArticleList;
            }
            return list;
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [OutputCache(Duration = 1, VaryByParam = "*")]
        [HttpGet]
        public ActionResult GetNextPage(int pageNum, int pageCount)
        {
            var list = GetByPage(pageNum, pageCount);
            return PartialView("_Index_Article",list);
        }

        /// <summary>
        /// 首页轮播图
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult SlideSwiper()
        {
            var list = GetByPage(1, 35);
            if (list.Count > 12)
            {
                var a = list.Take(12).ToList();
                return PartialView("_Index_Swiper", a);
            }

            return PartialView("_Index_Swiper",list);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Default()
        {
            return View();
        }
    }
}