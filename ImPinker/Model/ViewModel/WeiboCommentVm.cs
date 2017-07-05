
namespace ImModel.ViewModel
{
    public class WeiboCommentVm:WeiBoComment
    {
        /// <summary>
        /// 该评论的用户信息
        /// </summary>
        public string UserName { get; set; }

        public string HeadImage { get; set; }

        /// <summary>
        /// 评论的点赞数
        /// </summary>
        public int ArticleCommentVoteCount { get; set; }
        /// <summary>
        /// 引用的评论
        /// </summary>
        public WeiboCommentVm ToCommentItemVm { get; set; }
    }
}
