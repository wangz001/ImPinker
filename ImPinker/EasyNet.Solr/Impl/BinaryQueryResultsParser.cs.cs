
namespace EasyNet.Solr.Impl
{
    using Commons;

    /// <summary>
    /// 二进制格式返回数据查询结果解析器
    /// </summary>
    /// <typeparam name="T">查询结果类型</typeparam>
    public class BinaryQueryResultsParser<T> : ISolrResponseParser<NamedList, QueryResults<T>>
    {
        private readonly IObjectDeserializer<T> objectDeserialize;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="objectDeserialize">对象反序列化器</param>
        public BinaryQueryResultsParser(IObjectDeserializer<T> objectDeserialize)
        {
            this.objectDeserialize = objectDeserialize;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="result">名称对象集合</param>
        /// <returns>查询结果</returns>
        public QueryResults<T> Parse(NamedList result)
        {
            if (result == null)
            {
                return null;
            }

            var queryResults = new QueryResults<T>();
            var solrDocumentList = (SolrDocumentList)result.Get("response");

            if (solrDocumentList == null)
            {
                return null;
            }

            queryResults.NumFound = solrDocumentList.NumFound;
            queryResults.MaxScore = solrDocumentList.MaxScore;

            var docs = objectDeserialize.Deserialize(solrDocumentList);

            foreach (T doc in docs)
            {
                queryResults.Add(doc);
            }

            return queryResults;
        }
    }
}
