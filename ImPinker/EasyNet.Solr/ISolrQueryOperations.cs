
namespace EasyNet.Solr
{
    using System.Collections.Generic;

    /// <summary>
    /// Solr查询操作泛型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISolrQueryOperations<T>
    {
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="collection">Solr collection</param>
        /// <param name="handler">查询地址</param>
        /// <param name="query">Solr查询</param>
        /// <param name="options">查询参数</param>
        /// <returns>T类型数据</returns>
        T Query(string collection, string handler, ISolrQuery query, IDictionary<string, ICollection<string>> options);
    }
}
