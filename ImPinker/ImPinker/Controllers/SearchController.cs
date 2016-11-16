using System.Collections.Generic;
using System.Web.Mvc;
using ImBLL;
using ImModel.Dto;
using ImModel.ViewModel;
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
                dto.PageCount = 30;
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
            var searchvm=new SearchResultVm();
            if (string.IsNullOrEmpty(dto.Key)&&string.IsNullOrEmpty(dto.Tab)) return searchvm;
            var urlDecode = System.Web.HttpUtility.UrlDecode(dto.Key);
            if (urlDecode != null)
                dto.Key = urlDecode.Replace(" ", ",");  //url解码，去除特殊字符

            searchvm = SolrNetSearchBll.Query(dto.Key, dto.Tab, dto.FacetCompany, dto.FacetTag,dto.FacetDateTime, dto.PageNum , dto.PageCount);

            return searchvm;
        }
    }
}