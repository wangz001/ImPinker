
namespace EasyNet.Solr.Impl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using Commons;

    /// <summary>
    /// Solr readonly connection,used to query
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SolrQueryConnection<T> : ISolrQueryConnection<T>
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// 用于读取返回数据的编解码器
        /// </summary>
        public ICodecFactory ResponseCodecFactory { get; set; }

        /// <summary>
        /// 缓存
        /// </summary>
        public ISolrCache<T> Cache { get; set; }

        /// <summary>
        /// HttpWebRequest的KeepAlive属性
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// HttpWebRequest的ReadWriteTimeout属性
        /// </summary>
        public int ReadWriteTimeout { get; set; }

        /// <summary>
        /// HttpWebRequest的Timeout属性
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 默认构造函数
        /// 设置ResponseCodecFactory为BinaryCodecFactory
        /// 设置Cache为RuntimeCache
        /// </summary>
        public SolrQueryConnection()
        {
            ResponseCodecFactory = new BinaryCodecFactory();
            Cache = new RuntimeCache<T>();
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="relativeUrl">相对地址</param>
        /// <param name="parameters">参数</param>
        /// <returns>T类型数据</returns>
        public T Get(string relativeUrl, IDictionary<string, ICollection<string>> parameters)
        {
            var u = new UriBuilder(ServerUrl);

            u.Path += relativeUrl;

            var request = WebRequest.Create(u.Uri) as HttpWebRequest;
            var bytes = System.Text.Encoding.UTF8.GetBytes(parameters.BuildParams());

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bytes.Length;
            request.KeepAlive = KeepAlive;

            if (ReadWriteTimeout > 0)
            {
                request.ReadWriteTimeout = ReadWriteTimeout;
            }

            if (Timeout > 0)
            {
                request.Timeout = Timeout;
            }

            var cached = Cache[u.Uri.ToString()];

            if (cached != null)
            {
                request.Headers.Add(HttpRequestHeader.IfNoneMatch, cached.ETag);
            }

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            WebResponse response = null;
            Stream stream = null;

            try
            {
                response = request.GetResponse();
                stream = response.GetResponseStream();
                T result = (T)ResponseCodecFactory.Create().Unmarshal(stream);
                var etag = response.Headers[HttpResponseHeader.ETag];

                Cache.Add(new SolrCacheEntity<T>(u.Uri.ToString(), etag, result));

                return result;
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    var r = (HttpWebResponse)e.Response;

                    if (r.StatusCode == HttpStatusCode.NotModified)
                    {
                        return cached.Data;
                    }
                }

                throw e;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }
    }
}
