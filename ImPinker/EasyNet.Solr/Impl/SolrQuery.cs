
namespace EasyNet.Solr.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class SolrQuery : ISolrQuery
    {
        private readonly string query;

        /// <summary>
        /// 
        /// </summary>
        public static readonly ISolrQuery All = new SolrQuery("*:*");

        /// <summary>
        /// 
        /// </summary>
        public string Query
        {
            get
            {
                return query;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        public SolrQuery(string query)
        {
            this.query = query;
        }
    }
}
