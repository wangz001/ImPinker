using System;
using ImBLL;
using ImModel.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestRedisGet()
        {
            var flag = Common.Redis.RedisHelper.Set("aaaaa", "奔驰G500", 0);

            var str=Common.Redis.RedisHelper.StringGet("aaaaa");

        }
         [TestMethod]
        public void TestWeiboSearch()
        {
            SolrNet.Startup.Init<WeiboVm>("http://127.0.0.1:8080/solr/impinker-weibo");
            //var aa = SolrNetSearchBll.QueryWeiboByGeo("aa", 1, 10);
        }
         [TestMethod]
         public void TestArticleSearch()
         {
             SolrNet.Startup.Init<ArticleViewModel>("http://127.0.0.1:8080/solr/impinker");
             var aa = SolrNetSearchBll.GetArticleById("travel_1206");
         }
    }
}
