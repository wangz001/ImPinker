
namespace EasyNet.Solr.Impl
{
    using System.Collections;
    using System.Collections.Generic;

    using Commons;

    /// <summary>
    /// 二进制格式返回数据高亮解析器
    /// </summary>
    public class BinaryHighlightingParser : ISolrResponseParser<NamedList, IDictionary<string, IDictionary<string, IList<string>>>>
    {
        public IDictionary<string, IDictionary<string, IList<string>>> Parse(NamedList result)
        {
            if (result == null)
            {
                return null;
            }

            var highlightingNamedList = (NamedList)result.Get("highlighting");

            if (highlightingNamedList == null)
            {
                return null;
            }

            var highlightingResult = new Dictionary<string, IDictionary<string, IList<string>>>(highlightingNamedList.Count);

            for (var i = 0; i < highlightingNamedList.Count; i++)
            {
                var itemName = highlightingNamedList.GetName(i);
                var itemNamedList = (NamedList)highlightingNamedList.GetVal(i);

                if (!highlightingResult.ContainsKey(itemName))
                {
                    highlightingResult[itemName] = new Dictionary<string, IList<string>>(itemNamedList.Count);
                }

                for (var j = 0; j < itemNamedList.Count; j++)
                {
                    var hlItemName = itemNamedList.GetName(j);
                    var hlItemArrayList = (ArrayList)itemNamedList.GetVal(j);

                    if (!highlightingResult[itemName].ContainsKey(hlItemName))
                    {
                        highlightingResult[itemName][hlItemName] = new List<string>();
                    }

                    if (hlItemArrayList != null)
                    {
                        foreach (object obj in hlItemArrayList)
                        {
                            highlightingResult[itemName][hlItemName].Add(obj.ToString());
                        }
                    }
                }

            }

            return highlightingResult;
        }

    }
}
