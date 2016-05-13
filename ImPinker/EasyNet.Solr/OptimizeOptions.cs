
namespace EasyNet.Solr
{
    /// <summary>
    /// 优化命令选项
    /// </summary>
    public struct OptimizeOptions
    {
        /// <summary>
        /// 是否索引更新磁盘前一直阻塞，默认为true
        /// </summary>
        public bool? WaitFlush;

        /// <summary>
        /// 是否在新的检索器打开并注册到主查询检索器，索引变化前一直阻塞，默认为true
        /// </summary>
        public bool? WaitSearcher;

        /// <summary>
        /// 
        /// </summary>
        public bool? SoftCommit;

        /// <summary>
        /// 优化索引的段数，默认为1
        /// </summary>
        public int? MaxSegments;
    }
}
