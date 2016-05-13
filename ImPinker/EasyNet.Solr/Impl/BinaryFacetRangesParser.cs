
namespace EasyNet.Solr.Impl
{
    using System;
    using System.Collections.Generic;

    using Commons;

    public class BinaryFacetRangesParser : ISolrResponseParser<NamedList, IDictionary<string, FacetRange>>
    {
        public IDictionary<string, FacetRange> Parse(NamedList result)
        {
            if (result == null)
            {
                return null;
            }

            var facetCountsNamedList = (NamedList)result.Get("facet_counts");

            if (facetCountsNamedList == null)
            {
                return null;
            }

            var facetRangesNamedList = (NamedList)facetCountsNamedList.Get("facet_ranges");

            if (facetRangesNamedList == null)
            {
                return null;
            }

            var facetRangesResult = new Dictionary<string, FacetRange>(facetRangesNamedList.Count);

            for (var i = 0; i < facetRangesNamedList.Count; i++)
            {
                var fieldName = facetRangesNamedList.GetName(i);
                var fieldValue = (NamedList)facetRangesNamedList.GetVal(i);

                if (!facetRangesResult.ContainsKey(fieldName))
                {
                    facetRangesResult.Add(fieldName, new FacetRange() { Start = fieldValue.Get("start"), End = fieldValue.Get("end"), Gap = fieldValue.Get("gap") });
                }

                var countsNamedList = (NamedList)fieldValue.Get("counts");

                if (countsNamedList != null)
                {
                    facetRangesResult[fieldName].Counts = new List<FacetField>(countsNamedList.Count);

                    for (var j = 0; j < countsNamedList.Count; j++)
                    {
                        facetRangesResult[fieldName].Counts.Add(new FacetField() { Name = countsNamedList.GetName(j), Count = Convert.ToInt32(countsNamedList.GetVal(j)) });
                    }
                }
            }

            return facetRangesResult;

        }
    }
}
