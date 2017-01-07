using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel
{
    /// <summary>
    /// 对某篇文章的评论的点赞
    /// </summary>
    public class ArticleCommentVote
    {
        public long Id { get; set; }

        public long ArticleCommentId { get; set; }

        public int UserId { get; set; }

        public bool Vote { get; set; }
       
        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
