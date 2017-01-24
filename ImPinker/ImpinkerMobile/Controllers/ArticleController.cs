﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel.ViewModel;

namespace ImpinkerMobile.Controllers
{
    public class ArticleController : Controller
    {
        private static readonly ArticleBll ArticleBll = new ArticleBll();
        private static readonly UserBll UserBll = new UserBll();

        //
        // GET: /Article/

        public ActionResult Index(string id)
        {
            var vm = new ArticleViewModel();
            var idStr = id;
            long idInt = 0;
            if (!id.StartsWith("travels_"))
            {
                idStr = "travels_" + id;
            }
            long.TryParse(idStr.Replace("travels_", ""), out idInt);
            var article = SolrNetSearchBll.GetArticleById(idStr);
            if (article != null && article.Content != null && article.Content.Count > 0)
            {
                vm = article;
            }
            else
            {
                vm = ArticleBll.GetModelWithContent(idInt);
            }
            vm.Id = idInt.ToString();
            ViewBag.Article = vm;
            return View("Index");
        }

    }
}
