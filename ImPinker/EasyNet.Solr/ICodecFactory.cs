
namespace EasyNet.Solr
{
    /// <summary>
    /// 编解码工厂
    /// </summary>
    public interface ICodecFactory
    {
        /// <summary>
        /// 创建编解码器
        /// </summary>
        /// <returns></returns>
        ICodec Create();
    }
}
