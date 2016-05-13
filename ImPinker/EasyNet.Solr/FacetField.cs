
namespace EasyNet.Solr
{
    /// <summary>
    /// Facet.Field查询结果项
    /// </summary>
    public class FacetField
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }
    }
}
