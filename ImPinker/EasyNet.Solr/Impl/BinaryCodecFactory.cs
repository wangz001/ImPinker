
namespace EasyNet.Solr.Impl
{
    using Commons;

    /// <summary>
    /// 二进制格式编解码器工厂
    /// </summary>
    public class BinaryCodecFactory : ICodecFactory
    {
        /// <summary>
        /// 创建编解码器
        /// </summary>
        /// <returns>javabin格式编解码器</returns>
        public ICodec Create()
        {
            return new JavaBinCodec();
        }
    }
}
