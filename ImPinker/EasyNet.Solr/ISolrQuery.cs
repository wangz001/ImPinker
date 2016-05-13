
namespace EasyNet.Solr
{
    /// <summary>
    /// Solr查询接口
    /// </summary>
    public interface ISolrQuery
    {
        /// <summary>
        /// 查询字符串
        /// </summary>
        string Query { get; }
    }
}
