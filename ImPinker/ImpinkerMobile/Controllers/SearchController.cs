using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel.Dto;
using ImModel.ViewModel;
using Newtonsoft.Json;

namespace ImpinkerMobile.Controllers
{
    public class SearchController : Controller
    {
        private const int IndexPageCount = 10;
        //
        // GET: /Search/

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

        private SearchResultVm GetByPage(SearchDto dto)
        {
            var searchvm = new SearchResultVm();
            if (string.IsNullOrEmpty(dto.Key) && string.IsNullOrEmpty(dto.Tab)) return searchvm;
            var urlDecode = HttpUtility.UrlDecode(dto.Key);
            if (urlDecode != null)
                dto.Key = urlDecode.Replace(" ", ",");  //url解码，去除特殊字符
            searchvm = SolrNetSearchBll.Query(dto.Key, dto.Tab, dto.FacetCompany, dto.FacetTag, dto.FacetDateTime, dto.PageNum, dto.PageCount);
            return searchvm;
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageNum">页码</param>
        /// <param name="pageCount">每页的个数</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetNextPage(SearchDto dto)
        {
            var searchvm = GetByPage(dto);
            return PartialView("_Index_Article", searchvm.ArticleList);
        }
    }
}
