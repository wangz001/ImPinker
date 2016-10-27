using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ViewModel
{
    public class SearchResultVm
    {
        /// <summary>
        /// 当前查询的url
        /// </summary>
        public string SearchUrl { get; set; }
        /// <summary>
        /// 搜索结果
        /// </summary>
        public List<ArticleViewModel> ArticleList { get; set; }
        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNum { get; set; }
        /// <summary>
        /// 搜索结果总数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int MaxPageNum { get; set; }
        /// <summary>
        /// 来源分类
        /// </summary>
        public List<FacetItemVm> FacetDicCompany { get; set; }
        /// <summary>
        /// 标签分类
        /// </summary>
        public List<FacetItemVm> FacetDicTag { get; set; }
        /// <summary>
        /// 时间分组
        /// </summary>
        public List<FacetItemVm> FacetDicDateTime { get; set; }
        /// <summary>
        /// 已选择的facet分类.对应的链接是删除该条件后的链接
        /// </summary>
        public List<FacetItemVm> FacetSelected { get; set; } 
    }
}
