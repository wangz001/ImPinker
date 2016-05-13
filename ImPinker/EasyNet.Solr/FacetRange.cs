
namespace EasyNet.Solr
{
    using System.Collections.Generic;

    public class FacetRange
    {
        public object Start { get; set; }

        public object End { get; set; }

        public object Gap { get; set; }

        public IList<FacetField> Counts { get; set; }
    }
}
