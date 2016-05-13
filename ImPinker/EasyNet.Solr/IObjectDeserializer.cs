
namespace EasyNet.Solr
{
    using System.Collections.Generic;

    using Commons;

    /// <summary>
    /// 反序列化泛型接口
    /// </summary>
    /// <typeparam name="T">反序列化后对象的类型</typeparam>
    public interface IObjectDeserializer<T>
    {
        /// <summary>
        /// 对Solr文档反序列化
        /// </summary>
        /// <param name="result">Solr文档</param>
        /// <returns>迭代对象</returns>
        IEnumerable<T> Deserialize(SolrDocumentList result);
    }
}
