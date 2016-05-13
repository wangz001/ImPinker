
namespace EasyNet.Solr
{
    /// <summary>
    /// 提交命令选项
    /// </summary>
    public struct CommitOptions
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
        /// 是否删除合并已标记删除的数据，默认为false
        /// </summary>
        public bool? ExpungeDeletes;
    }
}
