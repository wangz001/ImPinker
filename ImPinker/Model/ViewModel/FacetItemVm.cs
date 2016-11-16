namespace ImModel.ViewModel
{
    public class FacetItemVm
    {
        /// <summary>
        /// facet 分组的 tag name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 相关查询结果的数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }
    }
}
