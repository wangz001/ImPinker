using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using ImPinker.Common;
using ImPinker.Models;

namespace ImPinker.Controllers
{
    public class HomeController : Controller
    {
        private static ArticleBll _articleBll = new ArticleBll();

        public ActionResult Index()
        {
            var list = new List<ArticleViewModel>();
            var ds = _articleBll.GetListByPage("", " createtime desc ", 0, 100);
            var articles = _articleBll.DataTableToList(ds.Tables[0]);
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

            ViewBag.ArticleVms = list;
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
            return View("Index");
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