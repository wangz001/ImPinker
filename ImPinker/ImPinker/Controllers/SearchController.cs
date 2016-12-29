using System.Collections.Generic;
using System.Web.Mvc;
using Common.DateTimeUtil;
using ImBLL;
using ImModel.Dto;
using ImModel.ViewModel;
using ImPinker.Common;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: 搜索，获取首页数据
        public ActionResult Index(SearchDto dto)
        {
            if (dto.PageNum == 0)
            {
                dto.PageNum = 1;
            }
            if (dto.PageCount == 0)
            {
                dto.PageCount = 32;
            }
            dto.IsHighLight = true;
            var searchvm = GetByPage(dto);
            ViewBag.searchVm = searchvm;
            return View();
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetNextPage(SearchDto dto)
        {
            var searchvm = GetByPage(dto);
            return PartialView("_Index_Article", searchvm.ArticleList);
        }
        /// <summary>
        /// 搜索，按分页获取数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private SearchResultVm GetByPage(SearchDto dto)
        {
            var searchvm = new SearchResultVm();
            if (string.IsNullOrEmpty(dto.Key) && string.IsNullOrEmpty(dto.Tab)) return searchvm;
            var urlDecode = System.Web.HttpUtility.UrlDecode(dto.Key);
            if (urlDecode != null)
                dto.Key = urlDecode.Replace(" ", ",");  //url解码，去除特殊字符
            searchvm = SolrNetSearchBll.Query(dto.Key, dto.Tab, dto.FacetCompany, dto.FacetTag, dto.FacetDateTime, dto.PageNum, dto.PageCount);
            return searchvm;
        }

        /// <summary>
        /// 首页热门标签分类及文章热门标签页面
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult HotTag(SearchDto dto)
        {
            if (dto.PageNum == 0)
            {
                dto.PageNum = 1;
            }
            if (dto.PageCount == 0)
            {
                dto.PageCount = 32;
            }
            dto.IsHighLight = false;
            var searchvm = SolrNetSearchBll.QueryHotTag(dto.Key, dto.Tab, dto.FacetCompany, dto.FacetTag, dto.FacetDateTime, dto.PageNum, dto.PageCount, false); ;
            ViewBag.HotTag = dto.Key;
            ViewBag.searchVm = searchvm;
            return View();
        }

    }
}