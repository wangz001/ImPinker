using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel
{
    public class ArticleComment
    {
        public long Id { get; set; }

        public long ArticleId { get; set; }

        public int UserId { get; set; }

        public string Content { get; set; }
        /// <summary>
        /// 回复某个评论，可为空
        /// </summary>
        public int ToCommentId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
