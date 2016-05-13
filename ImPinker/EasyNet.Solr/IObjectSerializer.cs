
namespace EasyNet.Solr
{
    using System.Collections.Generic;

    using Commons;

    /// <summary>
    /// 序列化接口
    /// </summary>
    /// <typeparam name="T">要进行序列化的对象的类型</typeparam>
    public interface IObjectSerializer<T>
    {
        /// <summary>
        /// 对象序列化
        /// </summary>
        /// <param name="objs">要进行序列化的迭代对象</param>
        /// <returns>Solr输入文档集合</returns>
        IEnumerable<SolrInputDocument> Serialize(IEnumerable<T> objs);
    }
}
