
namespace EasyNet.Solr
{
    using System;

    [Serializable]
    public class SolrCacheEntity<T>
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// ETag名称
        /// </summary>
        public string ETag { get; private set; }

        /// <summary>
        /// T类型数据
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="eTag">ETag名称</param>
        /// <param name="data">数据</param>
        public SolrCacheEntity(string url, string eTag, T data)
        {
            Url = url;
            ETag = eTag;
            Data = data;
        }
    }
}
