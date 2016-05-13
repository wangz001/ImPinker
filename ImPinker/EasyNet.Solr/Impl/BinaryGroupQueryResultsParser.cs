
namespace EasyNet.Solr.Impl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Commons;

    public class BinaryGroupQueryResultsParser<T> : ISolrResponseParser<NamedList, IList<GroupQueryResults<T>>>
    {
        private readonly IObjectDeserializer<T> objectDeserialize;

        public BinaryGroupQueryResultsParser(IObjectDeserializer<T> objectDeserialize)
        {
            this.objectDeserialize = objectDeserialize;
        }

        public IList<GroupQueryResults<T>> Parse(NamedList result)
        {
            if (result == null)
            {
                return null;
            }

            var groupedNamedList = (NamedList)result.Get("grouped");

            if (groupedNamedList == null)
            {
                return null;
            }

            var queryResults = new List<GroupQueryResults<T>>(groupedNamedList.Count);

            for (var i = 0; i < groupedNamedList.Count; i++)
            {
                var groupName = groupedNamedList.GetName(i);
                var groupValue = (NamedList)groupedNamedList.GetVal(i);

                var groupQueryResults = new GroupQueryResults<T>() { GroupName = groupName };

                if (groupValue != null)
                {
                    groupQueryResults.Matches = Convert.ToInt32(groupValue.Get("matches"));

                    var groupsList = (ArrayList)groupValue.Get("groups");

                    if (groupsList != null)
                    {
                        groupQueryResults.Groups = new List<GroupQueryResult<T>>(groupsList.Count);

                        for (var j = 0; j < groupsList.Count; j++)
                        {
                            var groupNamedList = (NamedList)groupsList[j];

                            var groupQueryResult = new GroupQueryResult<T>() { GroupValue = groupNamedList.Get("groupValue").ToString() };
                            var solrDocumentList = (SolrDocumentList)groupNamedList.Get("doclist");

                            if (solrDocumentList != null)
                            {
                                groupQueryResult.QueryResults = new QueryResults<T>() { NumFound = solrDocumentList.NumFound, MaxScore = solrDocumentList.MaxScore };

                                var docs = objectDeserialize.Deserialize(solrDocumentList);

                                foreach (T doc in docs)
                                {
                                    groupQueryResult.QueryResults.Add(doc);
                                }
                            }

                            groupQueryResults.Groups.Add(groupQueryResult);
                        }
                    }
                }

                queryResults.Add(groupQueryResults);
            }

            return queryResults;
        }
    }
}
