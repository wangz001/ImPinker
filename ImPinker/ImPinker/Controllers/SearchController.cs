using System.Collections.Generic;
using System.Web.Mvc;
using BLL;
using Model.Dto;
using Model.ViewModel;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: 搜索，获取首页数据
        public ActionResult Index(SearchDto dto)
        {
            if (dto.PageNum==0)
            {
                dto.PageNum = 1;
            }
            if (dto.PageCount == 0)
            {
                dto.PageCount = 10;
            }
            var searchvm = GetByPage(dto);
            ViewBag.searchVm = searchvm;
            return View();
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetNextPage(SearchDto dto)
        {
            var searchvm= GetByPage(dto);
            if (searchvm.ArticleList != null && searchvm.ArticleList.Count > 0)
            {
                return JsonConvert.SerializeObject(searchvm.ArticleList);
            }
            return string.Empty;
        }

        private SearchResultVm GetByPage(SearchDto dto)
        {
            if (string.IsNullOrEmpty(dto.Key)) return null;
            var urlDecode = System.Web.HttpUtility.UrlDecode(dto.Key);
            if (urlDecode != null)
                dto.Key = urlDecode.Replace(" ", ",");  //url解码，去除特殊字符

            var searchvm = SolrNetSearchBll.Query(dto.Key, dto.Tab, dto.FacetCompany, dto.FacetTag, dto.PageNum , dto.PageCount);

            return searchvm;
        }
    }
}