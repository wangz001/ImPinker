using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.ViewModel;
using SolrNet;
using SolrNet.Attributes;
using SolrNet.Commands.Parameters;

namespace BLL
{
    public class SolrNetSearchBll
    {
        private static readonly ISolrOperations<ArticleViewModel> SolrInstance;
        static SolrNetSearchBll()
        {
            SolrInstance = ServiceLocator.Current.GetInstance<ISolrOperations<ArticleViewModel>>();
        }

        /// <summary>
        /// 查询-关键字简单查询
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="tab">标签分类:新闻,改装,评测等</param>
        /// <param name="startNum">开始项</param>
        /// <param name="pageNum">数据条数</param>
        /// <param name="total"></param>
        /// <param name="maxNum"></param>
        /// <param name="facetDic">facet分组标签</param>
        /// <param name="facetgroup">facet分组类型</param>
        /// <param name="facet">值</param>
        /// <returns></returns>
        public static List<ArticleViewModel> Query(string keyWord, string tab, string facet, string facetgroup, int startNum, int pageNum
            , out int total, out int maxNum, out Dictionary<string, int> facetDic)
        {
            //高亮
            var high = new HighlightingParameters();
            high.Fields = new List<string> { "ArticleName", "KeyWords", "Description" };
            high.BeforeTerm = "<font color='red'><b>";
            high.AfterTerm = "</b></font>";

            //建立排序，条件.
            var options = new QueryOptions
            {
                Rows = pageNum,
                Start = startNum,
                Highlight = high
            };

            // 时间分组(测试)
            var facetPara = new FacetParameters
            {
                Queries = new[] { 
                    new SolrFacetDateQuery(
                        "CreateTime", 
                        new DateTime(2016, 08, 1) /* 开始时间 */, 
                        new DateTime(2016,10, 31) /* 结束时间 */, 
                        "+7DAY" /* 时间间隔 */) 
                    {
                        HardEnd = true,
                        Other = new[] {FacetDateOther.After, FacetDateOther.Before}
                    },
                }
            };

            var oneweek = new SolrQueryByRange<DateTime>("CreateTime", DateTime.Now, DateTime.Now.AddDays(-7));
            var twoweek = new SolrQueryByRange<DateTime>("CreateTime", DateTime.Now, DateTime.Now.AddDays(-14));
            var onemonth = new SolrQueryByRange<DateTime>("CreateTime", DateTime.Now, DateTime.Now.AddMonths(-1));
            var sixmonth = new SolrQueryByRange<DateTime>("CreateTime", DateTime.Now, DateTime.Now.AddMonths(-6));
            var moretime = new SolrQueryByRange<DateTime>("CreateTime", DateTime.Now.AddMonths(-6), DateTime.Now.AddYears(-20));

            //来源分组
            var facetCompany = new FacetParameters
            {
                Queries = new ISolrFacetQuery[] { 
                    new SolrFacetFieldQuery("Company")
                    , new SolrFacetFieldQuery("KeyWords")
                    ,new SolrFacetDateQuery(
                        "CreateTime", 
                        new DateTime(2016, 08, 1) /* 开始时间 */, 
                        new DateTime(2016,10, 31) /* 结束时间 */, 
                        "+7DAY" /* 时间间隔 */) 
                    {
                        HardEnd = true,
                        Other = new[] {FacetDateOther.After, FacetDateOther.Before}
                    }
                    ,
                    new SolrFacetQuery(oneweek), new SolrFacetQuery(twoweek), new SolrFacetQuery(onemonth),
                    new SolrFacetQuery(sixmonth), new SolrFacetQuery(moretime)
                }
            };

            options.Facet = facetCompany;

            //创建条件集合
            var query = new List<ISolrQuery>();
            //添加条件
            if (!string.IsNullOrEmpty(keyWord))
            {
                var ar = new List<ISolrQuery>
                {
                    new SolrQueryByField("ArticleName", keyWord),
                    new SolrQueryByField("KeyWords", keyWord),
                    new SolrQueryByField("Description", keyWord)
                };

                //创建ID 条件集合的关系,是OR还是AND
                var kw = new SolrMultipleCriteriaQuery(ar, "OR");
                //添加至条件集合
                query.Add(kw);
            }
            if (!string.IsNullOrEmpty(tab))
            {
                var tb = new SolrQueryByField("KeyWords", tab);
                query.Add(tb);
            }
            if (!string.IsNullOrEmpty(facet) && !string.IsNullOrEmpty(facetgroup)) //facet分组
            {
                if (facetgroup.Equals("Company"))
                {
                    var tb = new SolrQueryByField("Company", facet);
                    query.Add(tb);
                }
                else
                {
                    var tb = new SolrQueryByField("KeyWords", facet);
                    query.Add(tb);
                }
            }

            //按照时间倒排序.
            //options.AddOrder(new SortOrder("CreateTime", Order.DESC));

            //条件集合之间的关系
            var qTBO = new SolrMultipleCriteriaQuery(query, "AND");

            //执行查询,有5个重载
            SolrQueryResults<ArticleViewModel> results = SolrInstance.Query(qTBO, options);

            var highlights = results.Highlights;
            if (highlights != null && highlights.Count > 0)
            {
                foreach (var item in results)
                {
                    var snippets = highlights[item.Id].Snippets;
                    foreach (var snippet in snippets)
                    {
                        if (snippet.Key.Equals("ArticleName"))
                        {
                            var t = snippet.Value.ToList()[0];
                            item.ArticleName = t;
                        }
                        if (snippet.Key.Equals("KeyWords"))
                        {
                            var t = snippet.Value.ToList()[0];
                            item.KeyWords = t;
                        }
                        if (snippet.Key.Equals("Description"))
                        {
                            var t = snippet.Value.ToList()[0];
                            item.Description = t;
                        }
                    }
                }
            }

            //来源分组
            var companyFacet = results.FacetFields["Company"];

            facetDic = new Dictionary<string, int>();
            foreach (var f in companyFacet)
            {
                if (!string.IsNullOrEmpty(f.Key.Trim()) && f.Value > 0)
                {
                    facetDic.Add(f.Key, f.Value);
                }
            }
            //标签分组
            var keywordsFacet = results.FacetFields["KeyWords"];
            foreach (var f in keywordsFacet)
            {
                if (!string.IsNullOrEmpty(f.Key.Trim()) && f.Value > 0)
                {
                    facetDic.Add(f.Key, f.Value);
                }
                if (facetDic.Count > 12) break;
            }
            //时间分组
            var dateFacet = results.FacetDates["CreateTime"];
            foreach (KeyValuePair<DateTime, int> dr in dateFacet.DateResults)
            {
                if (dr.Value > 0)
                {
                    facetDic.Add(dr.Key.ToLongDateString(), dr.Value);
                }
                if (facetDic.Count > 20) break;
            }
            //区间查询
            var rangeFacet = results.FacetQueries;
            foreach (var f in rangeFacet)
            {
                if (f.Value > 0)
                {
                    facetDic.Add(f.Key, f.Value);
                }
                if (facetDic.Count > 25) break;
            }
            total = results.NumFound;
            maxNum = total / pageNum + 1;

            return results;
        }

    }
}
