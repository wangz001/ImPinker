using System;
using System.Collections.Generic;
using SolrNet.Attributes;

namespace ImModel.ViewModel
{
    /// <summary>
    /// solr查询返回的序列化对象
    /// </summary>
    public class ArticleViewModel
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("UserId")]
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

        [SolrField("content")]
        public List<Object> Content { get; set; }
        [SolrField("Company")]
        public string Company { get; set; }
        /// <summary>
        /// 文章被赞的总数
        /// </summary>
        public int VoteCount { get; set; }

        /// <summary>
        /// 评论总数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int ViewCount { get; set; }
        /// <summary>
        /// 用户名（显示名优先）
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 头像地址
        /// </summary>
        public string UserHeadUrl{get;set;}
        /// <summary>
        /// 时间显示
        /// </summary>
        public string CreateTimeStr { get; set; }
    }
}
