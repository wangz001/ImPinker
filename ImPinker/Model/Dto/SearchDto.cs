using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dto
{
    /// <summary>
    /// 搜索参数
    /// </summary>
    public class SearchDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 分类标签
        /// </summary>
        public string Tab { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string FacetCompany { get; set; }
        /// <summary>
        /// 关键字标签
        /// </summary>
        public string FacetTag { get; set; }
        /// <summary>
        /// 时间范围
        /// </summary>
        public string FacetDateTime { get; set; }
    }
}
