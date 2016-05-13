
namespace EasyNet.Solr
{
    using System.IO;

    /// <summary>
    /// 编解码器
    /// </summary>
    public interface ICodec
    {
        /// <summary>
        /// 对对象进行编码，并输出到流
        /// </summary>
        /// <param name="obj">要进行编码的对象</param>
        /// <param name="stream">输出流</param>
        void Marshal(object obj, Stream stream);

        /// <summary>
        /// 对流进行解码
        /// </summary>
        /// <param name="stream">要解码的流</param>
        /// <returns>解码后的对象</returns>
        object Unmarshal(Stream stream);
    }
}
