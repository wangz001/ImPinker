using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using ImPinker.Filters;
using Model.ViewModel;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ArticleBll ArticleBll = new ArticleBll();
        private const int IndexPageCount = 20;

        /// <summary>
        /// 首页
        /// </summary>默认缓存1分钟
        /// <returns></returns>
        [OutputCache(Duration = 1, VaryByParam = "*")]
        public ActionResult Index()
        {
            ViewBag.ArticleJson = GetByPage(1, 30);
            ViewBag.pageCount = IndexPageCount;
            return View();
        }

        private string GetByPage(int pageNum, int pageCount)
        {
            //如果是新用户，则推荐热门文章；老用户，则根据用户兴趣标签，智能推荐
            var list = new List<ArticleViewModel>();
            const string userInterestKey = "";
            if (string.IsNullOrEmpty(userInterestKey))
            {
                list = ArticleBll.GetIndexListByPage(pageNum, pageCount);
            }
            else
            {
                list = SolrNetSearchBll.Query(userInterestKey, "", "", "", "", pageNum, pageCount).ArticleList;
            }
            if (list.Count == 0)
            {
                return string.Empty;
            }
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [OutputCache(Duration = 1, VaryByParam = "*")]
        [HttpGet]
        public string GetNextPage(int pageNum, int pageCount)
        {
            var str = GetByPage(pageNum, pageCount);
            return str;
        }


        [AuthorizationFilter]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [ValidateAntiForgeryToken]
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