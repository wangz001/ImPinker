
namespace EasyNet.Solr
{
    public class GroupQueryResult<T>
    {
        public object GroupValue { get; set; }

        public QueryResults<T> QueryResults { get; set; }
    }
}
