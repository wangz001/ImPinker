using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImModel.Enum;
using SolrNet.Attributes;

namespace ImModel.ViewModel
{
    public class WeiboVm
    {
        [SolrUniqueKey("id")]
        public string SolrId { get; set; }

        public long Id { get; set; }
        [SolrField("UserId")]
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserHeadImage { get; set; }

        [SolrField("Description")]
        public string Description { get; set; }
        [SolrField("ContentValue")]
        public string ContentValue { get; set; }
        [SolrField("ContentType")]
        public WeiBoContentTypeEnum ContentType { get; set; }
        [SolrField("Longitude")]
        public decimal Longitude { get; set; }
        [SolrField("Latitude")]
        public decimal Lantitude { get; set; }
        [SolrField("Height")]
        public decimal Height { get; set; }
        [SolrField("LocationText")]
        public string LocationText { get; set; }
        /// <summary>
        /// 'lan,lon'  地理位置查询用
        /// </summary>
        [SolrField("weibo_position")]
        public string WeiboPosition { get; set; }
        [SolrField("State")]
        public WeiBoStateEnum State { get; set; }
        /// <summary>
        /// 硬件版本号（ios Android  等）
        /// </summary>
        public string HardWareType { get; set; }
        /// <summary>
        /// 是否转发
        /// </summary>
        public bool IsRePost { get; set; }
        [SolrField("CreateTime")]
        public DateTime CreateTime { get; set; }
        [SolrField("UpdateTime")]
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 发布时间，字符串
        /// </summary>
        public string PublishTime { get; set; }

        /// <summary>
        /// 点赞总数
        /// </summary>
        public int VoteCount { get; set; }

        /// <summary>
        /// 评论总数
        /// </summary>
        public int CommentCount { get; set; }

        
    }
}
