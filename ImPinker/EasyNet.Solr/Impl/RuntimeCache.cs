
namespace EasyNet.Solr.Impl
{
    using System;
    using System.Runtime.Caching;

    /// <summary>
    /// 运行时缓存
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RuntimeCache<T> : ISolrCache<T>
    {
        private readonly ObjectCache objectCache = MemoryCache.Default;

        /// <summary>
        /// 滑动过期时间，以分钟为单位，默认为5分钟
        /// </summary>
        public int SlidingMinutes { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RuntimeCache()
        {
            SlidingMinutes = 5;
        }

        public SolrCacheEntity<T> this[string url]
        {
            get { return (SolrCacheEntity<T>)objectCache[url]; }
        }

        public void Add(SolrCacheEntity<T> entity)
        {
            objectCache.Add(entity.Url, entity, new CacheItemPolicy()
            {
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration,
                SlidingExpiration = TimeSpan.FromMinutes(SlidingMinutes)
            });
        }
    }
}
