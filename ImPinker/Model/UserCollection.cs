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
    public class UserCollection
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 文章id
        /// </summary>
        public long EntityId { get; set; }
        /// <summary>
        /// 状态：0：删除  1:关注   2：取消关注
        /// </summary>
        public UserCollectionStateEnum State { get; set; }

        /// <summary>
        /// 收藏类型
        /// </summary>
        public EntityTypeEnum EntityType { get; set; }

        public DateTime CreateTime { set; get; }
        public DateTime UpdateTime { set; get; }
    }
}
