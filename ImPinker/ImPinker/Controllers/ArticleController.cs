using System;
using System.Web.Mvc;
using BLL;
using ImPinker.Models;
using Microsoft.AspNet.Identity;
using Model;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class ArticleController : Controller
    {
		private static readonly ArticleBll ArticleBll=new ArticleBll();
		private static readonly UserBll UserBll=new UserBll();
        private const int MyPageCount = 10;

        //
        // GET: /Article/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 我的文章
        /// </summary>
        /// <returns></returns>
        public ActionResult MyArticle()
        {
            var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
            var ds = ArticleBll.GetMyListByPage( userId, 1, MyPageCount);
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
        [HttpGet]
        public string GetMyNextPage(int pageIndex, int pageCount)
        {
            var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
            var ds = ArticleBll.GetMyListByPage(userId, pageIndex, pageCount);
            var articles = ArticleBll.DataTableToList(ds.Tables[0]);
            if (articles!=null&&articles.Count>0)
            {
                return JsonConvert.SerializeObject(articles);
            }
            return string.Empty;
        }

        //
        // GET: /Article/Details/5
        public ActionResult Details(int id)
        {
            return RedirectToAction("Index");
        }

        //
        // GET: /Article/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="createArticleVm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CreateArticleViewModel createArticleVm)
        {
            try
            {
	            var article = new Article
	            {
					Url=createArticleVm.ArticleUrl,
					ArticleName = createArticleVm.ArticleName,
					Description = createArticleVm.Description,
					KeyWords = createArticleVm.KeyWords,
					UserId =UserBll.GetModelByAspNetId( User.Identity.GetUserId()).Id,
					State = (int)ArticleStateEnum.BeCheck,   //待审核状态
   					CreateTime = DateTime.Now,
					UpdateTime = DateTime.Now,
                    Company = ""
	            };
				//审核通过后添加索引
	            var flag=ArticleBll.Add(article);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

	    //
        // GET: /Article/Edit/5
        public ActionResult Edit(int id)
        {
            return RedirectToAction("Index");
        }

        //
        // POST: /Article/Edit/5
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
        public ActionResult Delete(int id)
        {
            return RedirectToAction("Index");
        }

        //
        // POST: /Article/Delete/5
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
