using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel
{
    /// <summary>
    /// 微博赞
    /// </summary>
    public class WeiBoVote
    {
        public long Id { get; set; }

        public long WeiBoId { get; set; }

        public int UserId { get; set; }

        public bool Vote { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
