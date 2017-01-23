using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel.ViewModel;
using Newtonsoft.Json;

namespace ImpinkerMobile.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ArticleBll ArticleBll = new ArticleBll();
        private const int IndexPageCount = 10;
        public ActionResult Index()
        {
            ViewBag.ArticleJson = GetByPage(1, IndexPageCount);
            ViewBag.pageCount = IndexPageCount;
            return View();
        }

        private List<ArticleViewModel> GetByPage(int pageNum, int pageCount)
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
                //list = SolrNetSearchBll.Query(userInterestKey, "", "","","",pageNum,pageCount);
            }
            
            return list;
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetNextPage(int pageNum, int pageCount)
        {
            var list = GetByPage(pageNum, pageCount);
            return PartialView("_Index_Article", list);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
