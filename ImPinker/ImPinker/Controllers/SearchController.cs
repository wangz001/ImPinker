using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using Model.ViewModel;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class SearchController : Controller
    {
        private const int IndexPageCount = 10;
        //
        // GET: /Search/
        public ActionResult Index(string key)
        {
            string result = GetByPage(key, 1, 10);
            if (string.IsNullOrEmpty(result))
            {
                result = "[]";
            }
            ViewBag.ArticleVms = result;
            ViewBag.pageCount = IndexPageCount;
            return View();
        }

        private string GetByPage(string key , int pageNum,int pageCount)
        {
            if (!string.IsNullOrEmpty(key))
            {
                key = System.Web.HttpUtility.UrlDecode(key).Replace(" ",",");  //url解码，去除特殊字符
                int totlaCount, maxNum;
                List<ArticleViewModel> list = SolrNetSearchBll.Query(key, pageNum, pageCount,out totlaCount,out maxNum);
                if (list != null && list.Count > 0)
                {
                    return JsonConvert.SerializeObject(list);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageNum">页码</param>
        /// <param name="pageCount">每页的个数</param>
        /// <returns></returns>
        [HttpGet]
        public string GetNextPage(string key, int pageNum,int pageCount)
        {
            return GetByPage(key, pageNum, pageCount);
        }
	}
}