using System;
using System.Collections;
using System.Collections.Generic;
using EasyNet.Solr;
using EasyNet.Solr.Commons;
using EasyNet.Solr.Impl;
using Model.ViewModel;

namespace BLL
{
    public class SolrSearchBll
    {
        //private const string SolrUrl = "http://101.200.175.157:8080/solr/";
        private const string SolrUrl = "http://localhost:8080/solr/";
        private const string CoreName = "impinker";

        static readonly OptimizeOptions OptimizeOptions = new OptimizeOptions();
        static readonly ISolrResponseParser<NamedList, ResponseHeader> BinaryResponseHeaderParser = new BinaryResponseHeaderParser();
        static readonly IUpdateParametersConvert<NamedList> UpdateParametersConvert = new BinaryUpdateParametersConvert();
        static readonly ISolrUpdateConnection<NamedList, NamedList> SolrUpdateConnection = new SolrUpdateConnection<NamedList, NamedList>() { ServerUrl = SolrUrl };
        static readonly ISolrUpdateOperations<NamedList> UpdateOperations = new SolrUpdateOperations<NamedList, NamedList>(SolrUpdateConnection, UpdateParametersConvert) { ResponseWriter = "javabin" };
        static readonly BinaryHighlightingParser HighlightingParser = new BinaryHighlightingParser();
        static readonly ISolrQueryConnection<NamedList> SolrQueryConnection = new SolrQueryConnection<NamedList>() { ServerUrl = SolrUrl };
        static readonly ISolrQueryOperations<NamedList> SolrQueryOperations = new SolrQueryOperations<NamedList>(SolrQueryConnection) { ResponseWriter = "javabin" };

        static SolrSearchBll()
        {

        }

        public static string Query()
        {
            return string.Empty;
        }

        /// <summary>
        /// 查询
        /// </summary>
        public static List<ArticleViewModel> Query(string keyWord, int rowNum, int count)
        {
            var resultLists = new List<ArticleViewModel>();
            var queryParam = new Dictionary<string, ICollection<string>>();
            if (string.IsNullOrEmpty(keyWord))
            {
                return null;
            }
            if (rowNum > 1)
            {
                queryParam.Add("start", new List<string>() { ((rowNum - 1) * count).ToString() });
            }
            queryParam.Add("rows", new List<string>() { count.ToString() });
            queryParam.Add("hl", new List<string>() { "true" });
            //高亮

            var queryStr = string.IsNullOrEmpty(keyWord) ? SolrQuery.All : new SolrQuery("text:" + keyWord);
            var result = SolrQueryOperations.Query(CoreName, "/select", queryStr, queryParam);
            var highLightResult = HighlightingParser.Parse(result);
            var solrDocumentList = (SolrDocumentList)result.Get("response");
            //整合返回的数据
            if (solrDocumentList != null && solrDocumentList.Count > 0)
            {
                var existArticles = new List<int>();
                foreach (SolrDocument solrDocument in solrDocumentList)
                {
                    var idStr = solrDocument["id"].ToString();
                    var travelId = Int32.Parse(idStr.Substring(idStr.IndexOf("_") + 1));
                    var userId = Int32.Parse(solrDocument["UserId"].ToString());
                    var articleName = !solrDocument.ContainsKey("ArticleName") ? "" : solrDocument["ArticleName"].ToString();
                    var keyWords = !solrDocument.ContainsKey("KeyWords") ? "" : solrDocument["KeyWords"].ToString();
                    var description = !solrDocument.ContainsKey("Description") ? "" : solrDocument["Description"].ToString();
                    var url = !solrDocument.ContainsKey("Url") ? "" : solrDocument["Url"].ToString();
                    var coverImage = !solrDocument.ContainsKey("CoverImage") ? "" : solrDocument["CoverImage"].ToString();
                    var createTime = DateTime.Parse(solrDocument["CreateTime"].ToString()).ToString("MM-dd hh:mm");
                    if (existArticles.Contains(travelId)) continue;
                    if (articleName.Length > 25)
                    {
                        articleName = articleName.Substring(0, 25) + "……";
                    }

                    var searchvm = new ArticleViewModel
                    {
                        ArticleName = articleName,
                        ArticleUrl = url,
                        Description = description,
                        KeyWords = keyWords,
                        CoverImage = coverImage,
                        CreateTime = createTime
                    };
                    resultLists.Add(searchvm);
                    existArticles.Add(travelId);
                }
                return resultLists;
            }
            return null;
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="docIds"></param>
        /// <returns></returns>
        public static bool DeleteDoc(List<string> docIds) //docId为 table_id  格式
        {
            var result = UpdateOperations.Update(CoreName, "/update", new UpdateOptions() { OptimizeOptions = OptimizeOptions, DelById = docIds });
            var header = BinaryResponseHeaderParser.Parse(result);
            return header.Status == 0; //返回状态码。0表示成功
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        public static string Index()
        {
            var docs = new List<SolrInputDocument>();
            var doc = new SolrInputDocument();
            doc.Add("id", new SolrInputField("id", "SOLR1000"));
            doc.Add("name", new SolrInputField("name", "Solr, the Enterprise Search Server"));
            doc.Add("features", new SolrInputField("features", new String[] { "Advanced Full-Text Search Capabilities using Lucene", "Optimized for High Volume Web Traffic", "Standards Based Open Interfaces - XML and HTTP", "Comprehensive HTML Administration Interfaces", "Scalability - Efficient Replication to other Solr Search Servers", "Flexible and Adaptable with XML configuration and Schema", "Good unicode support: h&#xE9;llo (hello with an accent over the e)" }));
            doc.Add("price", new SolrInputField("price", 0.0f));
            doc.Add("popularity", new SolrInputField("popularity", 10));
            doc.Add("inStock", new SolrInputField("inStock", true));
            doc.Add("incubationdate_dt", new SolrInputField("incubationdate_dt", new DateTime(2006, 1, 17, 0, 0, 0, DateTimeKind.Utc)));

            docs.Add(doc);

            var result = UpdateOperations.Update("collection1", "/update", new UpdateOptions() { OptimizeOptions = OptimizeOptions, Docs = docs });
            var header = BinaryResponseHeaderParser.Parse(result);
            return string.Format("Update Status:{0} QTime:{1}", header.Status, header.QTime);
        }

        /// <summary>
        /// 根据id获取，爬虫爬取的文章。包含content
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        public static ArticleViewModel QueryById(string docId)
        {
            var queryParam = new Dictionary<string, ICollection<string>>();
            if (string.IsNullOrEmpty(docId))
            {
                return null;
            }
            queryParam.Add("hl", new List<string>() { "true" });
            //高亮

            var queryStr = new SolrQuery("id:" + docId);
            var result = SolrQueryOperations.Query(CoreName, "/select", queryStr, queryParam);
            var highLightResult = HighlightingParser.Parse(result);
            var solrDocumentList = (SolrDocumentList)result.Get("response");
            //整合返回的数据
            if (solrDocumentList != null && solrDocumentList.Count > 0)
            {
                var solrDocument = solrDocumentList[0];
                var idStr = solrDocument["id"].ToString();
                var travelId = Int32.Parse(idStr.Substring(idStr.IndexOf("_") + 1));
                var userId = Int32.Parse(solrDocument["UserId"].ToString());
                var articleName = !solrDocument.ContainsKey("ArticleName") ? "" : solrDocument["ArticleName"].ToString();
                var keyWords = !solrDocument.ContainsKey("KeyWords") ? "" : solrDocument["KeyWords"].ToString();
                var description = !solrDocument.ContainsKey("Description") ? "" : solrDocument["Description"].ToString();
                var url = !solrDocument.ContainsKey("Url") ? "" : solrDocument["Url"].ToString();
                var coverImage = !solrDocument.ContainsKey("CoverImage") ? "" : solrDocument["CoverImage"].ToString();
                var createTime = DateTime.Parse(solrDocument["CreateTime"].ToString()).ToString("MM-dd hh:mm");
                var searchvm = new ArticleViewModel
                {
                    ArticleName = articleName,
                    ArticleUrl = url,
                    Description = description,
                    KeyWords = keyWords,
                    CoverImage = coverImage,
                    CreateTime = createTime
                };
                return searchvm;
            }
            return null;
        }
    }
}