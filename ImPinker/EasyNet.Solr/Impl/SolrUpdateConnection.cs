
namespace EasyNet.Solr.Impl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using Commons;

    /// <summary>
    /// Solr更新操作泛型类
    /// </summary>
    /// <typeparam name="IT">输入数据类型</typeparam>
    /// <typeparam name="OT">输出数据类型</typeparam>
    public class SolrUpdateConnection<IT, OT> : ISolrUpdateConnection<IT, OT>
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        public string ServerUrl { get; set; }

        /// <summary>
        /// 用于读取返回数据的编解码器
        /// </summary>
        public ICodecFactory RequestCodecFactory { get; set; }

        /// <summary>
        /// 用于写入数据的编解码器
        /// </summary>
        public ICodecFactory ResponseCodecFactory { get; set; }

        /// <summary>
        /// HttpWebRequest的ContentType属性
        /// </summary>
        public string ContentType { get; set; }

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
        /// 设置RequestCodecFactory、ResponseCodecFactory为BinaryCodecFactory
        /// 设置ReadWriteTimeout、Timeout为int.MaxValue
        /// </summary>
        public SolrUpdateConnection()
        {
            RequestCodecFactory = new BinaryCodecFactory();
            ResponseCodecFactory = new BinaryCodecFactory();
            ContentType = "application/javabin";
            ReadWriteTimeout = int.MaxValue;
            Timeout = int.MaxValue;
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="relativeUrl">相对地址</param>
        /// <param name="data">IT类型数据</param>
        /// <param name="parameters">参数</param>
        /// <returns>OT类型数据</returns>
        public OT Post(string relativeUrl, IT data, IDictionary<string, ICollection<string>> parameters)
        {
            var u = new UriBuilder(ServerUrl);

            u.Path += relativeUrl;
            u.Query = parameters.BuildParams();

            var request = WebRequest.Create(u.Uri) as HttpWebRequest;

            if (!string.IsNullOrEmpty(ContentType))
            {
                request.ContentType = ContentType;
            }

            request.Method = "POST";
            request.KeepAlive = KeepAlive;

            if (ReadWriteTimeout > 0)
            {
                request.ReadWriteTimeout = ReadWriteTimeout;
            }

            if (Timeout > 0)
            {
                request.Timeout = Timeout;
            }

            Stream requestStream = null;

            try
            {
                requestStream = request.GetRequestStream();

                if (data != null)
                {
                    RequestCodecFactory.Create().Marshal(data, requestStream);
                }
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                }
            }

            WebResponse response = null;
            Stream stream = null;

            try
            {
                response = request.GetResponse();
                stream = response.GetResponseStream();

                return (OT)ResponseCodecFactory.Create().Unmarshal(stream);
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

        public override string ToString()
        {
            return ServerUrl;
        }
    }
}
