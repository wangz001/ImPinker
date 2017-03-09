using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImModel.Enum;

namespace ImpinkerApi.Models
{
    public class NewWeiBoViewModel
    {

        public int UserId { get; set; }

        public string Description { get; set; }

        public string ContentValue { get; set; }

        public WeiBoContentTypeEnum ContentType { get; set; }

        public decimal Longitude { get; set; }

        public decimal Lantitude { get; set; }

        public decimal Height { get; set; }

        public string LocationText { get; set; }

        public WeiBoStateEnum State { get; set; }
        /// <summary>
        /// 硬件版本号（ios Android  等）
        /// </summary>
        public string HardWareType { get; set; }
        /// <summary>
        /// 是否转发
        /// </summary>
        public bool IsRePost { get; set; }

    }

    /// <summary>
    /// 微博列表模型
    /// </summary>
    public class WeiBoListViewModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserHeadImage { get; set; }

        public string Description { get; set; }

        public string ContentValue { get; set; }

        public decimal Longitude { get; set; }

        public decimal Lantitude { get; set; }

        public decimal Height { get; set; }

        public string LocationText { get; set; }

        /// <summary>
        /// 是否转发
        /// </summary>
        public bool IsRePost { get; set; }
    }
}