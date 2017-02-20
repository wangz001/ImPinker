using ImModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel
{
    public class WeiBo
    {
        public long Id { get; set; }

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

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
