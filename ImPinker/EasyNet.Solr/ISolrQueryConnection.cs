
namespace EasyNet.Solr
{
    using System.Collections.Generic;

    /// <summary>
    /// Solr只读连接
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISolrQueryConnection<T>
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        string ServerUrl { get; set; }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="relativeUrl">相对地址</param>
        /// <param name="parameters">参数</param>
        /// <returns>T类型数据</returns>
        T Get(string relativeUrl, IDictionary<string, ICollection<string>> parameters);
    }
}
