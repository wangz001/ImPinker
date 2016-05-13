
namespace EasyNet.Solr.Impl
{
    using System;
    using System.Collections.Generic;

    using Commons;

    /// <summary>
    /// 二进制格式返回数据Facet.Field解析器
    /// </summary>
    public class BinaryFacetFieldsParser : ISolrResponseParser<NamedList, IDictionary<string, IList<FacetField>>>
    {
        public IDictionary<string, IList<FacetField>> Parse(NamedList result)
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

            var facetFieldsNamedList = (NamedList)facetCountsNamedList.Get("facet_fields");

            if (facetFieldsNamedList == null)
            {
                return null;
            }

            var facetFieldsResult = new Dictionary<string, IList<FacetField>>(facetFieldsNamedList.Count);

            for (var i = 0; i < facetFieldsNamedList.Count; i++)
            {
                var fieldName = facetFieldsNamedList.GetName(i);

                if (!facetFieldsResult.ContainsKey(fieldName))
                {
                    facetFieldsResult.Add(fieldName, new List<FacetField>());
                }

                var fieldCountNamedList = (NamedList)facetFieldsNamedList.GetVal(i);

                if (fieldCountNamedList != null)
                {
                    for (var j = 0; j < fieldCountNamedList.Count; j++)
                    {
                        facetFieldsResult[fieldName].Add(new FacetField() { Name = fieldCountNamedList.GetName(j), Count = Convert.ToInt32(fieldCountNamedList.GetVal(j)) });
                    }
                }
            }

            return facetFieldsResult;
        }
    }
}
