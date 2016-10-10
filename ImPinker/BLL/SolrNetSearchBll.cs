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
        private static readonly ISolrOperations<ArticleViewModel> _solr; 
        static SolrNetSearchBll() 
        {
            _solr = ServiceLocator.Current.GetInstance<ISolrOperations<ArticleViewModel>>();
        }

        public DataTable Result = new DataTable();
        public static int total;
        public static int maxNum;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="startNum">开始项</param>
        /// <param name="pageNum">数据条数</param>
        /// <returns></returns>
        public static List<ArticleViewModel> Query(string keyWord, int startNum, int pageNum)
        {
            //建立排序，条件.
            var options = new QueryOptions();
            options.Rows = pageNum;
            options.Start = startNum;


            //创建查询条件
            var qTB = new SolrQueryByField("text", keyWord);

            //创建条件集合
            var query = new List<ISolrQuery>();
            //添加条件
            query.Add(qTB);

          
            //按照时间倒排序.
            options.AddOrder(new SortOrder("CreateTime", Order.DESC));


            //条件集合之间的关系
            var qTBO = new SolrMultipleCriteriaQuery(query, "AND");

            var high = new HighlightingParameters();
            high.Fields = new List<string> { "ArticleName" };
            high.BeforeTerm = "<font color='red'><b>";
            high.AfterTerm = "</b></font>";
            options.Highlight = high;


            //执行查询,有5个重载
            SolrQueryResults<ArticleViewModel> results = _solr.Query(qTBO, options);

            var highlights = results.Highlights;
            foreach (var item in results)
            {
                var t = highlights[item.Id].Values.ToList()[0].ToList()[0];
                item.ArticleName = t;
            }

            total = results.NumFound;
            maxNum = total / pageNum + 1;

            return results;

        }

    }
}
