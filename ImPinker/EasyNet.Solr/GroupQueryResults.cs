
namespace EasyNet.Solr
{
    using System.Collections.Generic;

    public class GroupQueryResults<T>
    {
        public string GroupName { get; set; }

        public int Matches { get; set; }

        public IList<GroupQueryResult<T>> Groups { get; set; }
    }
}
