﻿using System;
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

        [SolrField("content")]
        public List<Object> Content { get; set; }
        [SolrField("Company")]
        public string Company { get; set; }

        public string CreateTimeStr { get; set; }
    }
}
