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
		public ActionResult Index()
		{
			ViewBag.ArticleVms = null;
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