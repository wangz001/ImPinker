
namespace EasyNet.Solr
{
    using System.Collections.Generic;

    /// <summary>
    /// Solr链接泛型接口
    /// </summary>
    /// <typeparam name="IT">输入的数据类型</typeparam>
    /// <typeparam name="OT">输出的数据类型</typeparam>
    public interface ISolrUpdateConnection<IT, OT>
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        string ServerUrl { get; set; }


        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="relativeUrl">相对地址</param>
        /// <param name="data">IT类型数据</param>
        /// <param name="parameters">参数</param>
        /// <returns>OT类型数据</returns>
        OT Post(string relativeUrl, IT data, IDictionary<string, ICollection<string>> parameters);
    }
}
