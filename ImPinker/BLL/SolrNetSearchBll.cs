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

        public SolrNetSearchBll() 
        {
            
        }

        public DataTable Result = new DataTable();
        public int total;
        public int maxNum;
        public int pageNum = 36;

        public void TestSolrNetSearch(string keyword)
        {
            //定义solr
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<testVm>>();

            //建立排序，条件.
            QueryOptions options = new QueryOptions();
            options.Rows = pageNum;//数据条数
            options.Start = 0;//开始项


            //创建查询条件
            var qTB = new SolrQueryByField("text", "越野");

            //创建条件集合
            List<ISolrQuery> query = new List<ISolrQuery>();
            //添加条件
            query.Add(qTB);

          
            //按照时间倒排序.
            options.AddOrder(new SortOrder("CreateTime", Order.DESC));


            //条件集合之间的关系
            var qTBO = new SolrMultipleCriteriaQuery(query, "AND");

            //执行查询,有5个重载
            SolrQueryResults<testVm> results = solr.Query(qTBO, options);

            this.total = results.NumFound;
            maxNum = total / pageNum + 1;

          
           
        }

        public class testVm
        {
             [SolrUniqueKey("id")]
            public string Id { get; set; }

            [SolrField("userid")]
            public string Userid { get; set; }

            [SolrField("ArticleName")]
            public string ArticleName { get; set; }

            [SolrField("Url")]
            public string Url { get; set; }

            [SolrField("Description")]
            public string Description { get; set; }

            [SolrField("KeyWords")]
            public string KeyWords { get; set; }

            [SolrField("CoverImage")]
            public string CoverImage { get; set; }

            [SolrField("CreateTime")]
            public DateTime CreateTime { get; set; }

            [SolrField("UpdateTime")]
            public DateTime UpdateTime { get; set; }

            [SolrField("Content")]
            public string Content { get; set; }

        }
    }
}
