
namespace EasyNet.Solr
{
    using System.Collections.Generic;

    using Commons;

    /// <summary>
    /// Solr更新操作选项
    /// </summary>
    public class UpdateOptions
    {
        /// <summary>
        /// Solr输入文档迭代器
        /// </summary>
        public IEnumerable<SolrInputDocument> Docs { get; set; }

        /// <summary>
        /// 更新文档选项
        /// </summary>
        public AddOptions? AddOptions { get; set; }

        /// <summary>
        /// 提交选项
        /// </summary>
        public CommitOptions? CommitOptions { get; set; }

        /// <summary>
        /// 优化选项
        /// </summary>
        public OptimizeOptions? OptimizeOptions { get; set; }

        /// <summary>
        /// 删除的Id迭代器
        /// </summary>
        public IEnumerable<string> DelById { get; set; }

        /// <summary>
        /// 删除查询迭代器
        /// </summary>
        public IEnumerable<string> DelByQ { get; set; }
    }
}
