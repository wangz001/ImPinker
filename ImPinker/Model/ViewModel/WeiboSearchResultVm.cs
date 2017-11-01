using System.Collections.Generic;

namespace ImModel.ViewModel
{
    public class WeiboSearchResultVm
    {
        /// <summary>
        /// 当前查询的url
        /// </summary>
        public string SearchUrl { get; set; }
        /// <summary>
        /// 搜索结果
        /// </summary>
        public List<WeiboVm> WeiboList { get; set; }
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
    }
}
