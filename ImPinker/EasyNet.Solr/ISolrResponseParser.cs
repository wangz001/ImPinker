
namespace EasyNet.Solr
{
    /// <summary>
    /// Solr返回解析器泛型接口
    /// </summary>
    /// <typeparam name="ST">ST类型返回数据</typeparam>
    /// <typeparam name="DT">DT类型要解析为的数据</typeparam>
    public interface ISolrResponseParser<ST, DT>
    {
        DT Parse(ST result);
    }
}
