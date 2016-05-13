
namespace EasyNet.Solr.Commons
{
    using System.Collections.Generic;

    /// <summary>
    /// Solr返回文档集合
    /// </summary>
    public class SolrDocumentList : List<SolrDocument>
    {
        /// <summary>
        /// 符合条件的记录数
        /// </summary>
        public long NumFound { get; set; }

        /// <summary>
        /// 起始记录
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// 最大评分
        /// </summary>
        public float? MaxScore { get; set; }
    }
}
