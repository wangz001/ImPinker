using System;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using BLL;
using ImPinker.Filters;
using ImPinker.Models;
using Microsoft.AspNet.Identity;
using Model;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class ArticleController : Controller
    {
        private static readonly ArticleBll ArticleBll = new ArticleBll();
        private static readonly UserBll UserBll = new UserBll();
        private const int MyPageCount = 10;

        /// <summary>
        /// 文章预览。爬虫抓取到的文章。即userid=2的文章。用户收藏的文章直接跳转到原始页面
        /// </summary>
        /// <param name="id">文章id：travels_id  或者  id</param>
        /// <returns></returns>
        public ActionResult Index(int id)
        {
            var indexId = "travels_" + id;
            var article = SolrNetSearchBll.GetArticleById(indexId);
            //var article = ArticleBll.GetModel(id);
            return new RedirectResult(article.Url);
            //抓取的内容显示比较混乱，直接跳转到源网页
            ViewBag.Article = article;
            return View("Index");
        }

        /// <summary>
        /// 我的文章
        /// </summary>
        /// <returns></returns>
        [AuthorizationFilter]
        public ActionResult MyArticle()
        {
            var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
            var ds = ArticleBll.GetMyListByPage(userId, 1, MyPageCount);
            var articles = ArticleBll.DataTableToList(ds.Tables[0]);
            ViewBag.jsonData = JsonConvert.SerializeObject(articles);
            ViewBag.pageCount = MyPageCount;
            return View();
        }

        /// <summary>
        /// 分页获取数据接口
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        [AuthorizationFilter]
        [HttpGet]
        public string GetMyNextPage(int pageIndex, int pageCount)
        {
            var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
            var ds = ArticleBll.GetMyListByPage(userId, pageIndex, pageCount);
            var articles = ArticleBll.DataTableToList(ds.Tables[0]);
            if (articles != null && articles.Count > 0)
            {
                return JsonConvert.SerializeObject(articles);
            }
            return string.Empty;
        }

        /// <summary>
        /// 文章预览。爬虫抓取到的文章。即userid=2的文章。用户收藏的文章直接跳转到原始页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            return View("Index");
        }

        //
        // GET: /Article/Create
        [AuthorizationFilter]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="createArticleVm"></param>
        /// <returns></returns>
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult Create(CreateArticleViewModel createArticleVm)
        {
            var article = new Article
            {
                Url = createArticleVm.ArticleUrl,
                ArticleName = createArticleVm.ArticleName,
                Description = createArticleVm.Description,
                KeyWords = createArticleVm.KeyWords,
                UserId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id,
                State = (int)ArticleStateEnum.BeCheck,   //待审核状态
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Company = ""
            };
            //审核通过后添加索引
            var flag = ArticleBll.Add(article);
            return RedirectToAction("MyArticle");

        }

        //
        // GET: /Article/Edit/5
        [AuthorizationFilter]
        public ActionResult Edit(int id)
        {
            return RedirectToAction("Index");
        }

        //
        // POST: /Article/Edit/5
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Article/Delete/5
        [AuthorizationFilter]
        public ActionResult Delete(int id)
        {
            return RedirectToAction("Index");
        }

        //
        // POST: /Article/Delete/5
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}
