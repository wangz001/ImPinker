using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImModel.Enum;

namespace ImModel
{
    /// <summary>
    /// 文章收藏表
    /// </summary>
    public class ArticleCollection
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 文章id
        /// </summary>
        public long ArticleId { get; set; }
        /// <summary>
        /// 状态：0：删除  1:关注   2：取消关注
        /// </summary>
        public ArticleCollectionStateEnum State { get; set; }

        public DateTime CreateTime { set; get; }
        public DateTime UpdateTime { set; get; }
    }
}
