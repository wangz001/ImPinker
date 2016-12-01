using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel.ViewModel
{
    public class ArticleCommentVm:ArticleComment
    {
        /// <summary>
        /// 引用的评论
        /// </summary>
        public List<ArticleComment> ListToComment { get; set; }
    }
}
