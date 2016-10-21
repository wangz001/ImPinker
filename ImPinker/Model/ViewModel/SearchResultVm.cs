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
        public Dictionary<string, int> FacetDicCompany { get; set; }
        /// <summary>
        /// 标签分类
        /// </summary>
        public Dictionary<string, int> FacetDicTag { get; set; }
        /// <summary>
        /// 时间分组
        /// </summary>
        public Dictionary<string, int> FacetDicDateTime { get; set; } 
    }
}
