using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BLL;
using ImPinker.Models;
using Microsoft.AspNet.Identity;
using Model;

namespace ImPinker.Controllers
{
    public class ArticleController : Controller
    {
		private static ArticleBll _articleBll=new ArticleBll();
		private static UserBll _userBll=new UserBll();
        //
        // GET: /Article/
        public ActionResult Index()
        {
            return View();
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

        //
        // POST: /Article/Create
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
					UserId =_userBll.GetModelByAspNetId( User.Identity.GetUserId()).Id,
					State = (int)ArticleStateEnum.BeCheck,   //待审核状态
   					CreateTime = DateTime.Now,
					UpdateTime = DateTime.Now
	            };
				//审核通过后添加索引
	            var flag=_articleBll.Add(article);
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
