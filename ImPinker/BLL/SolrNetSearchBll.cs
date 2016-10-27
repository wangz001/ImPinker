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
        /// <param name="facetDateTime">时间</param>
        /// <param name="pageNum">数据条数</param>
        /// <param name="pageCount"></param>
        /// <param name="facetCompany">来源</param>
        /// <param name="facetTag">关键词标签</param>
        /// <returns></returns>
        public static SearchResultVm Query(string keyWord, string tab, string facetCompany
            , string facetTag, string facetDateTime, int pageNum, int pageCount)
        {
            var searchParaStr = "Index?";
            searchParaStr += "key=" + keyWord + "&tab=" + tab;
            if (!string.IsNullOrEmpty(facetCompany))
            {
                searchParaStr += "&facetCompany=" + facetCompany;
            }
            if (!string.IsNullOrEmpty(facetTag))
            {
                searchParaStr += "&facetTag=" + facetTag;
            }
            if (!string.IsNullOrEmpty(facetDateTime))
            {
                searchParaStr += "&facetDateTime=" + facetDateTime;
            }

            var startNum = (pageNum - 1) * pageCount;

            #region 查询条件

            //高亮设置
            var high = new HighlightingParameters
            {
                Fields = new List<string> { "ArticleName", "KeyWords", "Description" },
                BeforeTerm = "<font color='red'><b>",
                AfterTerm = "</b></font>"
            };

            //建立排序，条件.
            var options = new QueryOptions
            {
                Start = startNum,
                Rows = pageCount,
                Highlight = high
            };
            var facetQueries = new List<ISolrFacetQuery>();
            if (string.IsNullOrEmpty(facetCompany))
            {
                facetQueries.Add(new SolrFacetFieldQuery("Company"));
            }
            if (string.IsNullOrEmpty(facetTag))
            {
                facetQueries.Add(new SolrFacetFieldQuery("KeyWords"));
            }

            var tNow = DateTime.Now;
            var showTimeRange = new List<Tuple<int, string, DateTime, DateTime>>
            {
                new Tuple<int, string, DateTime, DateTime>(0, "一周内", tNow.AddDays(-7), tNow),
                new Tuple<int, string, DateTime, DateTime>(1, "二周内", tNow.AddDays(-14), tNow),
                new Tuple<int, string, DateTime, DateTime>(2, "一月内", tNow.AddMonths(-1), tNow),
                new Tuple<int, string, DateTime, DateTime>(3, "半年内", tNow.AddMonths(-6), tNow),
                new Tuple<int, string, DateTime, DateTime>(4, "半年以上", tNow.AddYears(-20), tNow.AddMonths(-6))
            };
            if (string.IsNullOrEmpty(facetDateTime))
            {
                var facetTime = new List<ISolrFacetQuery>();
                foreach (var tuple in showTimeRange)
                {
                    var t = new SolrQueryByRange<DateTime>("CreateTime", tuple.Item3, tuple.Item4);
                    facetTime.Add(new SolrFacetQuery(t));
                }
                facetQueries.AddRange(facetTime);
            }

            //facet分组
            var facet = new FacetParameters
            {
                Queries = facetQueries
            };
            options.Facet = facet;

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
            if (!string.IsNullOrEmpty(facetCompany)) //facet分组
            {
                query.Add(new SolrQueryByField("Company", facetCompany));
            }
            if (!string.IsNullOrEmpty(facetTag)) //facet分组
            {
                query.Add(new SolrQueryByField("KeyWords", facetTag));
            }

            int index;
            if (!string.IsNullOrEmpty(facetDateTime) && Int32.TryParse(facetDateTime, out index))
            {
                var tupleTime = showTimeRange[index];
                //创建时间范围条件， 开始时间和结束时间 
                SolrQueryByRange<DateTime> qDateRange = null;
                var stime = DateTime.Parse(tupleTime.Item3.ToString());
                var etime = DateTime.Parse(tupleTime.Item4.ToString());
                //后两个参数,一个是开始时间,一个是结束时时间
                qDateRange = new SolrQueryByRange<DateTime>("CreateTime", stime, etime);
                //时间范围条件加入集合
                query.Add(qDateRange);
            }

            //按照时间倒排序.
            //options.AddOrder(new SortOrder("CreateTime", Order.DESC));

            //条件集合之间的关系
            var qTBO = new SolrMultipleCriteriaQuery(query, "AND");

            #endregion

            //执行查询,有5个重载
            SolrQueryResults<ArticleViewModel> results = SolrInstance.Query(qTBO, options);

            # region 高亮处理

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

            #endregion

            #region facet 结果处理

            var facetDicCompany = new List<FacetItemVm>();
            if (results.FacetFields.ContainsKey("Company"))
            {
                //来源分组
                var companyFacet = results.FacetFields["Company"];
                foreach (var f in companyFacet)
                {
                    if (!string.IsNullOrEmpty(f.Key.Trim()) && f.Value > 0)
                    {
                        facetDicCompany.Add(
                            new FacetItemVm() { Name = f.Key, Count = f.Value, Url = searchParaStr + "&facetCompany=" + f.Key });
                    }
                }
            }


            var facetDicTag = new List<FacetItemVm>();
            if (results.FacetFields.ContainsKey("KeyWords"))
            {
                //标签分组
                var tagsFacet = results.FacetFields["KeyWords"];
                foreach (var f in tagsFacet)
                {
                    if (!string.IsNullOrEmpty(f.Key.Trim()) && f.Value > 0)
                    {
                        facetDicTag.Add(new FacetItemVm() { Name = f.Key, Count = f.Value, Url = searchParaStr + "&facetTag=" + f.Key });
                    }
                    if (facetDicTag.Count > 12) break;
                }
            }

            //时间区间查询
            var facetDicDateTime = new List<FacetItemVm>();
            if (results.FacetQueries.Count > 0)
            {
                var rangeFacet = results.FacetQueries;
                var i = 0;
                foreach (var f in rangeFacet)
                {
                    if (f.Value > 0)
                    {
                        facetDicDateTime.Add(new FacetItemVm() { Name = showTimeRange[i].Item2, Count = f.Value, Url = searchParaStr + "&facetDateTime=" + showTimeRange[i].Item1 });
                    }
                    i++;
                }
            }

            #endregion
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
