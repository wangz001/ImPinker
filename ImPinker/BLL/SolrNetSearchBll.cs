using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Model.ViewModel;
using SolrNet;
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
        /// <param name="pageNum">数据条数</param>
        /// <param name="pageCount"></param>
        /// <param name="facetgroup">facet分组类型</param>
        /// <param name="facet">值</param>
        /// <returns></returns>
        public static SearchResultVm Query(string keyWord, string tab, string facet
            , string facetgroup, int pageNum, int pageCount)
        {
            var startNum = (pageNum - 1) * pageCount;
            #region 查询条件
            //高亮设置
            var high = new HighlightingParameters();
            high.Fields = new List<string> { "ArticleName", "KeyWords", "Description" };
            high.BeforeTerm = "<font color='red'><b>";
            high.AfterTerm = "</b></font>";

            //建立排序，条件.
            var options = new QueryOptions
            {
                Start = startNum,
                Rows = pageCount,
                Highlight = high
            };

            var tNow = DateTime.Now;
            var oneweek = new SolrQueryByRange<DateTime>("CreateTime", tNow.AddDays(-7), tNow);
            var twoweek = new SolrQueryByRange<DateTime>("CreateTime", tNow.AddDays(-14), tNow);
            var onemonth = new SolrQueryByRange<DateTime>("CreateTime", tNow.AddMonths(-1), tNow);
            var sixmonth = new SolrQueryByRange<DateTime>("CreateTime", tNow.AddMonths(-6), tNow);
            var moretime = new SolrQueryByRange<DateTime>("CreateTime", tNow.AddYears(-20), tNow.AddMonths(-6));
            string[] showTRange = { "一周内", "二周内", "一月内", "半年内", "更多"};
            //来源分组
            var facetCompany = new FacetParameters
            {
                Queries = new ISolrFacetQuery[] { 
                    new SolrFacetFieldQuery("Company")
                    , new SolrFacetFieldQuery("KeyWords")
                    ,new SolrFacetQuery(oneweek), new SolrFacetQuery(twoweek), new SolrFacetQuery(onemonth),
                    new SolrFacetQuery(sixmonth), new SolrFacetQuery(moretime)
                }
            };
            options.Facet = facetCompany;

            //创建条件集合
            var query = new List<ISolrQuery>();
            if (!string.IsNullOrEmpty(keyWord))
            {
                var ar = new List<ISolrQuery>
                {
                    new SolrQueryByField("ArticleName", keyWord),
                    new SolrQueryByField("KeyWords", keyWord),
                    new SolrQueryByField("Description", keyWord)
                };
                //创建keyWord 条件集合的关系,是OR还是AND
                var kw = new SolrMultipleCriteriaQuery(ar, "OR");
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
            #endregion
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
            var facetDicCompany = new Dictionary<string, int>();
            foreach (var f in companyFacet)
            {
                if (!string.IsNullOrEmpty(f.Key.Trim()) && f.Value > 0)
                {
                    facetDicCompany.Add(f.Key, f.Value);
                }
            }
            //标签分组
            var tagsFacet = results.FacetFields["KeyWords"];
            var facetDicTag = new Dictionary<string, int>();
            foreach (var f in tagsFacet)
            {
                if (!string.IsNullOrEmpty(f.Key.Trim()) && f.Value > 0)
                {
                    facetDicTag.Add(f.Key, f.Value);
                }
                if (facetDicTag.Count > 12) break;
            }

            //时间区间查询
            var facetDicDateTime = new Dictionary<string, int>();
            var rangeFacet = results.FacetQueries;
            var i = 0;
            foreach (var f in rangeFacet)
            {
                if (f.Value > 0)
                {
                    facetDicDateTime.Add(showTRange[i], f.Value);
                }
                i++;
            }

            var searchVm = new SearchResultVm()
            {
                ArticleList = results,
                PageCount = pageCount,
                PageNum = pageNum,
                TotalCount = results.NumFound,
                MaxPageNum = results.NumFound / pageNum + 1,
                FacetDicCompany = facetDicCompany,
                FacetDicTag = facetDicTag,
                FacetDicDateTime = facetDicDateTime
            };
            return searchVm;
        }

    }
}
