
namespace EasyNet.Solr.Impl
{
    using System;

    using Commons;

    /// <summary>
    /// 二进制格式返回数据头解析器
    /// </summary>
    public class BinaryResponseHeaderParser : ISolrResponseParser<NamedList, ResponseHeader>
    {
        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="result">名称对象集合</param>
        /// <returns>结果头信息</returns>
        public ResponseHeader Parse(NamedList result)
        {
            if (result == null)
            {
                return null;
            }

            var responseHeaderNamedList = (NamedList)result.Get("responseHeader");

            if (responseHeaderNamedList == null)
            {
                return null;
            }

            return new ResponseHeader()
            {
                Status = Convert.ToInt32(responseHeaderNamedList.Get("status")),
                QTime = Convert.ToInt32(responseHeaderNamedList.Get("QTime"))
            };
        }
    }
}
