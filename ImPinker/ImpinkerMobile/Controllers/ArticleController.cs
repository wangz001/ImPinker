using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Utils;
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
            try
            {
                vm = SolrNetSearchBll.GetArticleById(idStr);
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(e.ToString());
            }
            if (vm == null || string.IsNullOrEmpty(vm.Content))
            {
                vm = ArticleBll.GetModelWithContent(idInt);
            }
            if (string.IsNullOrEmpty(vm.Company))
            {//原创文章
                vm.Id = idInt.ToString();
                ViewBag.Article = vm;
                var user = UserBll.GetModelByCache(Int32.Parse(vm.Userid));
                ViewBag.UserInfo = user;
                return View("Index_original");
            }
            else
            {//网络爬的文章
                vm.Id = idInt.ToString(CultureInfo.InvariantCulture);
                ViewBag.Article = vm;
                return View("Index");
            }
        }

    }
}
