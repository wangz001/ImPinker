using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Common.DateTimeUtil;
using Common.Utils;
using ImModel.ViewModel;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace ImBLL
{
    public class SolrNetSearchBll
    {
        private static readonly ISolrOperations<ArticleViewModel> SolrInstance;
        static SolrNetSearchBll()
        {
            SolrInstance = ServiceLocator.Current.GetInstance<ISolrOperations<ArticleViewModel>>();
        }

        /// <summary>
        /// 搜索-关键字简单查询
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
            //选定的afcet查询条件
            var facetSelectedList = new List<FacetItemVm>();

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
                facetSelectedList.Add(new FacetItemVm()
                {
                    Name = "来源：" + facetCompany,
                    Url = searchParaStr.Replace("&facetCompany=" + facetCompany, "")
                });
            }
            if (!string.IsNullOrEmpty(facetTag)) //facet分组
            {
                query.Add(new SolrQueryByField("KeyWords", facetTag));
                facetSelectedList.Add(new FacetItemVm()
                {
                    Name = "标签：" + facetTag,
                    Url = searchParaStr.Replace("&facetTag=" + facetTag, "")
                });
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
                facetSelectedList.Add(new FacetItemVm()
                {
                    Name = "时间：" + tupleTime.Item2,
                    Url = searchParaStr.Replace("&facetDateTime=" + facetDateTime, "")
                });
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
                    if (f.Value > 0)
                    {
                        var key = string.IsNullOrEmpty(f.Key.Trim()) ? "其他" : f.Key;
                        facetDicCompany.Add(
                            new FacetItemVm() { Name = key, Count = f.Value, Url = searchParaStr + "&facetCompany=" + key });
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
                FacetDicDateTime = facetDicDateTime,
                FacetSelected = facetSelectedList
            };
            return searchVm;
        }

        /// <summary>
        /// 热门标签搜索，返回搜索结果
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="tab"></param>
        /// <param name="facetCompany"></param>
        /// <param name="facetTag"></param>
        /// <param name="facetDateTime"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <param name="isHighLight"></param>
        /// <returns></returns>
        public static SearchResultVm QueryHotTag(string keyWord, string tab, string facetCompany
            , string facetTag, string facetDateTime, int pageNum, int pageCount, bool isHighLight)
        {
            var searchParaStr = "HotTag?";
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
            //选定的afcet查询条件
            var facetSelectedList = new List<FacetItemVm>();

            var startNum = (pageNum - 1) * pageCount;

            #region 查询条件
            //建立排序，条件.
            var options = new QueryOptions
            {
                Start = startNum,
                Rows = pageCount
            };


            if (isHighLight)
            {
                //高亮设置
                var high = new HighlightingParameters
                {
                    Fields = new List<string> { "ArticleName", "KeyWords", "Description" },
                    BeforeTerm = "<font color='red'><b>",
                    AfterTerm = "</b></font>"
                };
                options.Highlight = high;
            }

            #region 分组等条件查询
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
            #endregion

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
                facetSelectedList.Add(new FacetItemVm()
                {
                    Name = "来源：" + facetCompany,
                    Url = searchParaStr.Replace("&facetCompany=" + facetCompany, "")
                });
            }
            if (!string.IsNullOrEmpty(facetTag)) //facet分组
            {
                query.Add(new SolrQueryByField("KeyWords", facetTag));
                facetSelectedList.Add(new FacetItemVm()
                {
                    Name = "标签：" + facetTag,
                    Url = searchParaStr.Replace("&facetTag=" + facetTag, "")
                });
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
                facetSelectedList.Add(new FacetItemVm()
                {
                    Name = "时间：" + tupleTime.Item2,
                    Url = searchParaStr.Replace("&facetDateTime=" + facetDateTime, "")
                });
            }

            //条件集合之间的关系
            var qTBO = new SolrMultipleCriteriaQuery(query, "AND");

            #endregion

            //执行查询,有5个重载
            SolrQueryResults<ArticleViewModel> results = SolrInstance.Query(qTBO, options);

            # region 高亮处理

            if (isHighLight)
            {
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
                    if (f.Value > 0)
                    {
                        var key = string.IsNullOrEmpty(f.Key.Trim()) ? "其他" : f.Key;
                        facetDicCompany.Add(
                            new FacetItemVm() { Name = key, Count = f.Value, Url = searchParaStr + "&facetCompany=" + key });
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
                FacetDicDateTime = facetDicDateTime,
                FacetSelected = facetSelectedList
            };
            return searchVm;
        }

        /// <summary>
        /// 根据不同的viewtype。不同页面的搜索框。有通过标签跳转的，有页面的相关文章等
        /// </summary>
        /// <param name="ViewType"></param>
        /// <param name="keyWord"></param>
        /// <param name="IsHighLight"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public static SearchResultVm QueryByViewTpye(string ViewType, string keyWord, bool IsHighLight
            , int pageNum, int pageCount)
        {
            switch (ViewType)
            {
                case "RelativeArticle":
                    //文章详情页，右侧相关车型推荐
                    IsHighLight = false;
                    break;
                case "MyArticles":
                    //搜索我发的帖子和我收藏的帖子
                    //userid=。。。。。
                    break;
                case "3":
                    break;
                default: break;
            }
            var startNum = (pageNum - 1) * pageCount;

            #region 查询条件

            //建立排序，条件.
            var options = new QueryOptions
            {
                Start = startNum,
                Rows = pageCount
            };
            if (IsHighLight)
            {
                //高亮设置
                var high = new HighlightingParameters
                {
                    Fields = new List<string> { "ArticleName", "KeyWords", "Description" },
                    BeforeTerm = "<font color='red'><b>",
                    AfterTerm = "</b></font>"
                };
                options.Highlight = high;
            }

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

            //按照时间倒排序.
            //options.AddOrder(new SortOrder("CreateTime", Order.DESC));

            //条件集合之间的关系
            var qTBO = new SolrMultipleCriteriaQuery(query, "AND");

            #endregion

            //执行查询,有5个重载
            SolrQueryResults<ArticleViewModel> results = SolrInstance.Query(qTBO, options);

            # region 高亮处理

            if (IsHighLight)
            {
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
            }
            #endregion

            var searchVm = new SearchResultVm()
            {
                ArticleList = results,
                PageCount = pageCount,
                PageNum = pageNum,
                TotalCount = results.NumFound,
                MaxPageNum = results.NumFound / pageNum + 1
            };
            return searchVm;
        }


        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="travelId"></param>
        /// <returns></returns>
        public static ArticleViewModel GetArticleById(string travelId)
        {
            //创建条件集合
            var query = new List<ISolrQuery>();
            if (!string.IsNullOrEmpty(travelId))
            {
                var ar = new List<ISolrQuery>
                {
                    new SolrQueryByField("id", travelId)
                };
                //创建keyWord 条件集合的关系,是OR还是AND
                var kw = new SolrMultipleCriteriaQuery(ar, "OR");
                query.Add(kw);
            }
            //建立排序，条件.
            var options = new QueryOptions
            {
                Start = 0,
                Rows = 1
            };

            //条件集合之间的关系
            var qTBO = new SolrMultipleCriteriaQuery(query, "AND");


            //执行查询,有5个重载
            SolrQueryResults<ArticleViewModel> results = SolrInstance.Query(qTBO, options);
            if (results != null && results.Count > 0)
            {
                return results[0];
            }
            return null;
        }
        /// <summary>
        /// 批量添加索引文件
        /// </summary>
        /// <returns></returns>
        public static bool AddIndex(List<ArticleViewModel> list )
        {
            if (list == null || list.Count <= 0) return false;
            SolrInstance.AddRange(list);
            var result=SolrInstance.Commit();
            return result.Status>0;
        }
    }
}
