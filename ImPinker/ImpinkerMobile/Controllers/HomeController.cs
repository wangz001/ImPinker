﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Model;
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

        private string GetByPage(int pageNum, int pageCount)
        {
            //如果是新用户，则推荐热门文章；老用户，则根据用户兴趣标签，智能推荐
            var list = new List<SolrSearchBll.ArticleViewModel>();
            var userInterestKey = "";
            if (string.IsNullOrEmpty(userInterestKey))
            {
                var ds = ArticleBll.GetIndexListByPage(pageNum, pageCount);
                List<Article> articles = ArticleBll.DataTableToList(ds.Tables[0]);
                if (articles != null && articles.Count > 0)
                {
                    foreach (var article in articles)
                    {
                        if (article.ArticleName.Length > 25)
                        {
                            article.ArticleName = article.ArticleName.Substring(0, 25) + "……";
                        }
                        list.Add(new SolrSearchBll.ArticleViewModel()
                        {
                            ArticleName = article.ArticleName,
                            ArticleUrl = article.Url,
                            Description = article.Description,
                            KeyWords = article.KeyWords,
                            CoverImage = article.CoverImage,
                            CreateTime = article.CreateTime.ToString("MM-dd HH:mm")
                        });
                    }
                }
            }
            else
            {
                list = SolrSearchBll.Query(userInterestKey, pageNum, pageCount);
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
        [HttpGet]
        public string GetNextPage(int pageNum, int pageCount)
        {
            var str = GetByPage(pageNum, pageCount);
            return str;
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