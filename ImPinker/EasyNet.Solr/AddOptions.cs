
namespace EasyNet.Solr
{
    /// <summary>
    /// 添加或更新文档时的选项
    /// </summary>
    public struct AddOptions
    {
        /// <summary>
        /// 是否覆盖相同键值的文档，默认为true
        /// </summary>
        public bool? Overwrite;

        /// <summary>
        /// 在多少毫秒内提交添加或更新的文档
        /// </summary>
        public long? CommitWithin;
    }
}
