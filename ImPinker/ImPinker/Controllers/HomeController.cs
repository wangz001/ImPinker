using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using ImPinker.Common;
using ImPinker.Models;
using Model;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ArticleBll ArticleBll = new ArticleBll();
        private const int IndexPageCount = 10;

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            //如果是新用户，则推荐热门文章；老用户，则根据用户兴趣标签，智能推荐
            var list = new List<ArticleViewModel>();
            var userInterestKey = "越野，自驾游";
            if (string.IsNullOrEmpty(userInterestKey))
            {
                var ds = ArticleBll.GetIndexListByPage(1, IndexPageCount);
                List<Article> articles = ArticleBll.DataTableToList(ds.Tables[0]);
                if (articles != null && articles.Count > 0)
                {
                    foreach (var article in articles)
                    {
                        if (article.ArticleName.Length > 25)
                        {
                            article.ArticleName = article.ArticleName.Substring(0, 25) + "……";
                        }
                        list.Add(new ArticleViewModel()
                        {
                            ArticleName = article.ArticleName,
                            ArticleUrl = article.Url,
                            Description = article.Description,
                            KeyWords = article.KeyWords,
                        });
                    }
                }
            }
            else
            {
                list = EasyNetSolrUtil.Query(userInterestKey, 1, IndexPageCount);
            }
            ViewBag.ArticleJson =  JsonConvert.SerializeObject(list);
            ViewBag.pageCount = IndexPageCount;
            return View();
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult Search(string key)
        {
            var list = new List<ArticleViewModel>();
            if (!string.IsNullOrEmpty(key))
            {
                list = EasyNetSolrUtil.Query(key, 1, 10);
            }
            ViewBag.ArticleVms = list;
            return View();
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