
namespace EasyNet.Solr
{
    /// <summary>
    /// Solr缓存泛型接口
    /// </summary>
    /// <typeparam name="T">缓存对象类型</typeparam>
    public interface ISolrCache<T>
    {
        /// <summary>
        /// 根据url获取Solr缓存的对象
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Solr缓存实体</returns>
        SolrCacheEntity<T> this[string url] { get; }

        /// <summary>
        /// 添加对象到缓存
        /// </summary>
        /// <param name="entity">Solr缓存实体</param>
        void Add(SolrCacheEntity<T> entity);
    }
}
