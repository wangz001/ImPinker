using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common.DateTimeUtil;
using ImModel.ViewModel;
using Microsoft.Practices.ServiceLocation;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace ImBLL
{
    public class SolrNetSearchBll
    {
        private static readonly ISolrOperations<ArticleViewModel> ArticleInstance;
        private static readonly ISolrOperations<WeiboVm> WeiboInstance;
        private static readonly UserBll UserBll = new UserBll();
        static SolrNetSearchBll()
        {
            ArticleInstance = ServiceLocator.Current.GetInstance<ISolrOperations<ArticleViewModel>>();
            WeiboInstance = ServiceLocator.Current.GetInstance<ISolrOperations<WeiboVm>>();
        }

        #region 根据关键字查询，并根据时间、来源、关键字进行分组
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
                // ReSharper disable once CSharpWarnings::CS0618
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
                facetSelectedList.Add(new FacetItemVm
                {
                    Name = "来源：" + facetCompany,
                    Url = searchParaStr.Replace("&facetCompany=" + facetCompany, "")
                });
            }
            if (!string.IsNullOrEmpty(facetTag)) //facet分组
            {
                query.Add(new SolrQueryByField("KeyWords", facetTag));
                facetSelectedList.Add(new FacetItemVm
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
                var stime = DateTime.Parse(tupleTime.Item3.ToString(CultureInfo.InvariantCulture));
                var etime = DateTime.Parse(tupleTime.Item4.ToString(CultureInfo.InvariantCulture));
                //后两个参数,一个是开始时间,一个是结束时时间
                var qDateRange = new SolrQueryByRange<DateTime>("CreateTime", stime, etime);
                //时间范围条件加入集合
                query.Add(qDateRange);
                facetSelectedList.Add(new FacetItemVm
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
            SolrQueryResults<ArticleViewModel> results = ArticleInstance.Query(qTBO, options);

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
                            new FacetItemVm { Name = key, Count = f.Value, Url = searchParaStr + "&facetCompany=" + key });
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
                    if (!string.IsNullOrEmpty(f.Key.Trim()) && f.Key.Trim().Count() > 1 && f.Value > 0)
                    {
                        facetDicTag.Add(new FacetItemVm { Name = f.Key, Count = f.Value, Url = searchParaStr + "&facetTag=" + f.Key });
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
                        facetDicDateTime.Add(new FacetItemVm { Name = showTimeRange[i].Item2, Count = f.Value, Url = searchParaStr + "&facetDateTime=" + showTimeRange[i].Item1 });
                    }
                    i++;
                }
            }

            #endregion


            var searchVm = new SearchResultVm
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

        #endregion

        #region 搜索框，关键字查询
        /// <summary>
        /// 搜索-关键字简单查询
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <param name="isHighLight"></param>
        /// <returns></returns>
        public static SearchResultVm QueryArticleByKeyword(string keyWord, int pageNum, int pageCount, bool isHighLight = false)
        {
            pageNum = pageNum > 0 ? pageNum : 1;
            pageCount = (pageCount > 0 && pageCount < 50) ? pageCount : 30;
            #region 查询条件

            //建立排序，条件.
            var options = new QueryOptions
            {
                Rows = pageCount,
                // ReSharper disable once CSharpWarnings::CS0618
                Start = (pageNum - 1) * pageCount
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
            SolrQueryResults<ArticleViewModel> results = ArticleInstance.Query(qTBO, options);

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

            var searchVm = new SearchResultVm
            {
                ArticleList = results,
                PageCount = pageCount,
                PageNum = pageNum,
                TotalCount = results.NumFound,
                MaxPageNum = results.NumFound / pageNum + 1
            };
            return searchVm;
        }


        #endregion

        #region 热门标签搜索，返回搜索结果

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
                // ReSharper disable once CSharpWarnings::CS0618
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
                facetSelectedList.Add(new FacetItemVm
                {
                    Name = "来源：" + facetCompany,
                    Url = searchParaStr.Replace("&facetCompany=" + facetCompany, "")
                });
            }
            if (!string.IsNullOrEmpty(facetTag)) //facet分组
            {
                query.Add(new SolrQueryByField("KeyWords", facetTag));
                facetSelectedList.Add(new FacetItemVm
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
                var startTime = DateTime.Parse(tupleTime.Item3.ToString(CultureInfo.InvariantCulture));
                var endTime = DateTime.Parse(tupleTime.Item4.ToString(CultureInfo.InvariantCulture));
                //后两个参数,一个是开始时间,一个是结束时时间
                var qDateRange = new SolrQueryByRange<DateTime>("CreateTime", startTime, endTime);
                //时间范围条件加入集合
                query.Add(qDateRange);
                facetSelectedList.Add(new FacetItemVm
                {
                    Name = "时间：" + tupleTime.Item2,
                    Url = searchParaStr.Replace("&facetDateTime=" + facetDateTime, "")
                });
            }

            //条件集合之间的关系
            var qTBO = new SolrMultipleCriteriaQuery(query, "AND");

            #endregion

            //执行查询,有5个重载
            SolrQueryResults<ArticleViewModel> results = ArticleInstance.Query(qTBO, options);

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
                            new FacetItemVm { Name = key, Count = f.Value, Url = searchParaStr + "&facetCompany=" + key });
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
                    if (!string.IsNullOrEmpty(f.Key.Trim()) && f.Key.Trim().Count() > 1 && f.Value > 0)
                    {
                        facetDicTag.Add(new FacetItemVm { Name = f.Key, Count = f.Value, Url = searchParaStr + "&facetTag=" + f.Key });
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
                        facetDicDateTime.Add(new FacetItemVm { Name = showTimeRange[i].Item2, Count = f.Value, Url = searchParaStr + "&facetDateTime=" + showTimeRange[i].Item1 });
                    }
                    i++;
                }
            }

            #endregion

            var searchVm = new SearchResultVm
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

        #endregion

        #region 获取文章详情
        /// <summary>
        /// 获取文章详情
        /// </summary>
        /// <param name="travelId"></param>
        /// <returns></returns>
        public static ArticleViewModel GetArticleById(string travelId)
        {
            if (!string.IsNullOrEmpty(travelId) && travelId.StartsWith("travels_"))
            {
                var query = new List<ISolrQuery>
                {
                    new SolrQueryByField("id", travelId)
                };
                var options = new QueryOptions
                {
                    // ReSharper disable once CSharpWarnings::CS0618
                    Start = 0,
                    Rows = 1
                };
                var qTBO = new SolrMultipleCriteriaQuery(query, "AND");
                SolrQueryResults<ArticleViewModel> results = ArticleInstance.Query(qTBO, options);
                if (results != null && results.Count > 0)
                {
                    return results[0];
                }
            }
            return null;
        }

        #endregion

        #region 添加索引


        /// <summary>
        /// 批量添加索引文件
        /// </summary>
        /// <returns></returns>
        public static bool AddIndex(List<ArticleViewModel> list)
        {
            if (list == null || list.Count <= 0) return false;
            ArticleInstance.AddRange(list);
            var result = ArticleInstance.Commit();
            return result.Status > 0;
        }

        #endregion

        #region 根据地理位置获取微博列表
        /// <summary>
        /// 根据地理位置获取微博列表
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <param name="distance">距离</param>
        /// <param name="userid"></param>
        /// <param name="pagenum">页码</param>
        /// <param name="pagesize">条数</param>
        /// <returns></returns>
        public static WeiboSearchResultVm QueryWeiboByGeo(double latitude, double longitude, int distance, int userid, int pagenum, int pagesize)
        {
            //创建条件集合
            var query = new List<ISolrQuery>();
            #pragma warning disable 618
            var geoQuery = new SolrQueryByDistance("weibo_position", latitude, longitude, distance, CalculationAccuracy.BoundingBox);
            #pragma warning restore 618
            query.Add(geoQuery);
            //建立排序，条件.
            var options = new QueryOptions
            {
                Rows = (pagenum) * pagesize,
                // ReSharper disable once CSharpWarnings::CS0618
                Start = (pagenum - 1) * pagesize + 1,
            };
            options.AddOrder(new SortOrder("CreateTime", Order.DESC));
            //options.AddOrder(new SortOrder("geofilt", Order.ASC));
            var qTBO = new SolrMultipleCriteriaQuery(query, "AND");
            SolrQueryResults<WeiboVm> weiboList = WeiboInstance.Query(qTBO, options);
            if (weiboList != null && weiboList.Count > 0)
            {
                foreach (WeiboVm vm in weiboList)
                {
                    var idStr = vm.SolrId;
                    vm.Id = Int32.Parse(idStr.Replace("weibo_", ""));
                    if (!string.IsNullOrEmpty(vm.ContentValue))
                    {
                        vm.ContentValue = ImageUrlHelper.GetWeiboFullImageUrl(vm.ContentValue, 240);
                    }
                    vm.PublishTime = TUtil.DateFormatToString(vm.CreateTime);
                    var userinfo = UserBll.GetModelByCache(vm.UserId);
                    vm.UserName = !string.IsNullOrEmpty(userinfo.ShowName) ? userinfo.ShowName : userinfo.UserName;
                    vm.UserHeadImage = ImageUrlHelper.GetHeadImageUrl(userinfo.ImgUrl, 100);
                }
            }
            var searchVm = new WeiboSearchResultVm
            {
                WeiboList = weiboList,
                PageCount = 10,
                PageNum = 1,
                TotalCount = weiboList != null ? weiboList.NumFound : 0
            };
            return searchVm;
        }

        #endregion

        #region 根据关键字搜索微博

        public static WeiboSearchResultVm QueryWeiboByKeyword(string keyWord, int pageNum, int pageCount, bool isHighLight = false)
        {
            pageNum = pageNum > 0 ? pageNum : 1;
            pageCount = (pageCount > 0 && pageCount < 50) ? pageCount : 30;
            #region 查询条件
            //建立排序，条件.
            var options = new QueryOptions
            {
                Rows = pageCount,
                // ReSharper disable once CSharpWarnings::CS0618
                Start = (pageNum - 1) * pageCount
            };
            if (isHighLight)
            {
                //高亮设置
                var high = new HighlightingParameters
                {
                    Fields = new List<string> { "Description"},
                    BeforeTerm = "<font color='red'><b>",
                    AfterTerm = "</b></font>"
                };
                options.Highlight = high;
            }

            //创建条件集合
            var query = new List<ISolrQuery>();
            if (!string.IsNullOrEmpty(keyWord))
            {
                query.Add(new SolrQueryByField("Description", keyWord));
            }
            //条件集合之间的关系
            var qTBO = new SolrMultipleCriteriaQuery(query, "AND");
            #endregion
            //执行查询,有5个重载
            SolrQueryResults<WeiboVm> results = WeiboInstance.Query(qTBO, options);
            # region 高亮处理
            if (isHighLight)
            {
                var highlights = results.Highlights;
                if (highlights != null && highlights.Count > 0)
                {
                    foreach (var item in results)
                    {
                        var snippets = highlights[item.SolrId].Snippets;
                        foreach (var snippet in snippets)
                        {
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

            #region 数据封装
            if (results != null && results.Count > 0)
            {
                foreach (WeiboVm vm in results)
                {
                    var idStr = vm.SolrId;
                    vm.Id = Int32.Parse(idStr.Replace("weibo_", ""));
                    if (!string.IsNullOrEmpty(vm.ContentValue))
                    {
                        vm.ContentValue = ImageUrlHelper.GetWeiboFullImageUrl(vm.ContentValue, 240);
                    }
                    vm.PublishTime = TUtil.DateFormatToString(vm.CreateTime);
                    var userinfo = UserBll.GetModelByCache(vm.UserId);
                    vm.UserName = !string.IsNullOrEmpty(userinfo.ShowName) ? userinfo.ShowName : userinfo.UserName;
                    vm.UserHeadImage = ImageUrlHelper.GetHeadImageUrl(userinfo.ImgUrl, 100);
                }
            }

            #endregion
            var searchVm = new WeiboSearchResultVm
            {
                WeiboList = results,
                PageCount = pageCount,
                PageNum = pageNum,
                TotalCount = results.NumFound
            };
            return searchVm;
        }


        #endregion

    }
}
