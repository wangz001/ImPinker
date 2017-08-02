using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel.ViewModel
{
    public class WeiboVm:WeiBo
    {
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
