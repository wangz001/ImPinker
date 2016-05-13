
namespace EasyNet.Solr.Impl
{
    using System;
    using System.Collections.Generic;

    using Commons;

    /// <summary>
    /// 二进制格式返回数据Facet.Query解析器
    /// </summary>
    /// 
    public class BinaryFacetQueriesParser : ISolrResponseParser<NamedList, IList<FacetField>>
    {
        public IList<FacetField> Parse(NamedList result)
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

            var facetQueriesNamedList = (NamedList)facetCountsNamedList.Get("facet_queries");

            if (facetQueriesNamedList == null)
            {
                return null;
            }

            var facetFieldsResult = new List<FacetField>(facetQueriesNamedList.Count);

            for (var i = 0; i < facetQueriesNamedList.Count; i++)
            {

                facetFieldsResult.Add(new FacetField() { Name = facetQueriesNamedList.GetName(i), Count = Convert.ToInt32(facetQueriesNamedList.GetVal(i)) });


            }

            return facetFieldsResult;
        }
    }
}
