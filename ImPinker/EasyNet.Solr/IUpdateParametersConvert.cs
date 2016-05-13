
namespace EasyNet.Solr
{
    /// <summary>
    /// Solr更新操作参数选项转换器泛型接口
    /// </summary>
    /// <typeparam name="T">转换为的数据类型</typeparam>
    public interface IUpdateParametersConvert<T>
    {
        /// <summary>
        /// 转换更新操作参数
        /// </summary>
        /// <param name="updateOptions">更新选项</param>
        /// <returns>T类型数据</returns>
        T ConvertUpdateParameters(UpdateOptions updateOptions);

        /// <summary>
        /// 转换回退参数
        /// </summary>
        /// <returns>T类型数据</returns>
        T ConvertRollbackParameters();

    }
}
