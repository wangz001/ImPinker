using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel
{
    /// <summary>
    /// 微博评论
    /// </summary>
    public class WeiBoComment
    {
        public long Id { get; set; }

        public long WeiBoId { get; set; }

        public int UserId { get; set; }

        public string ContentText { get; set; }

        public long ToCommentId { get; set; }

        public int State { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
