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
            int totalCount, maxNum;
            Dictionary<string, int> facetDic;
            string result = GetByPage(key, 1, 10,out totalCount,out maxNum,out facetDic);
            if (string.IsNullOrEmpty(result))
            {
                result = "[]";
            }
            ViewBag.ArticleVms = result;
            ViewBag.pageCount = IndexPageCount;
            ViewBag.totalCount = totalCount;
            ViewBag.maxNum = maxNum;
            ViewBag.facetDic = facetDic;

            return View();
        }

        private string GetByPage(string key , int pageNum,int pageCount,out int totalCount,out int maxNum,out Dictionary<string, int> facetDic)
        {
            if (!string.IsNullOrEmpty(key))
            {
                key = System.Web.HttpUtility.UrlDecode(key).Replace(" ",",");  //url解码，去除特殊字符
                List<ArticleViewModel> list = SolrNetSearchBll.Query(key, pageNum, pageCount, out totalCount, out maxNum,out facetDic);
                if (list != null && list.Count > 0)
                {
                    return JsonConvert.SerializeObject(list);
                }
            }
            totalCount=0;
            maxNum=0;
            facetDic=new Dictionary<string, int>();
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
            int totalCount, maxNum;
            Dictionary<string, int> facetDic;
            return GetByPage(key, pageNum, pageCount, out totalCount, out maxNum,out facetDic);
        }
	}
}