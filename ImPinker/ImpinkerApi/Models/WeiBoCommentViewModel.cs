using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImpinkerApi.Models
{
    public class WeiBoCommentViewModel
    {
        public long WeiBoId { get; set; }

        public string CommentStr { get; set; }
        /// <summary>
        /// 不为0表示 是评论某条评论
        /// </summary>
        public long ToCommentId { get; set; }
    }
}