
namespace EasyNet.Solr.Impl
{
    using System.Collections.Generic;

    /// <summary>
    /// Solr查询操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SolrQueryOperations<T> : ISolrQueryOperations<T>
    {
        private readonly ISolrQueryConnection<T> connection;

        /// <summary>
        /// Solr的wt参数
        /// </summary>
        public string ResponseWriter { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection">Solr链接</param>
        public SolrQueryOperations(ISolrQueryConnection<T> connection)
        {
            this.connection = connection;
        }

        #region ISolrQueryOperations Members

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="collection">Solr collection</param>
        /// <param name="handler">查询地址</param>
        /// <param name="query">Solr查询</param>
        /// <param name="options">查询参数</param>
        /// <returns>T类型数据</returns>
        public T Query(string collection, string handler, ISolrQuery query, IDictionary<string, ICollection<string>> options)
        {
            options = options ?? new Dictionary<string, ICollection<string>>();

            if (!string.IsNullOrEmpty(ResponseWriter) && !options.ContainsKey("wt"))
            {
                options["wt"] = new string[] { ResponseWriter };
            }

            if (query != null && !string.IsNullOrEmpty(query.Query))
            {
                options["q"] = new string[] { query.Query };
            }

            return connection.Get(collection + handler, options);
        }

        #endregion
    }
}
