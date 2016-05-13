
namespace EasyNet.Solr
{
    /// <summary>
    /// Solr更新操作泛型接口
    /// </summary>
    /// <typeparam name="T">返回结果数据类型</typeparam>
    public interface ISolrUpdateOperations<T>
    {
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="collection">Solr collection</param>
        /// <param name="handler">更新地址</param>
        /// <param name="updateOptions">更新选项</param>
        /// <returns>T类型返回数据</returns>
        T Update(string collection, string handler, UpdateOptions updateOptions);

        /// <summary>
        /// 回退
        /// </summary>
        /// <param name="collection">Solr collection</param>
        /// <param name="handler">handler</param>
        /// <returns>T类型数据</returns>
        T Rollback(string collection, string handler);
    }
}
