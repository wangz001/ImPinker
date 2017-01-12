using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImModel.Enum;

namespace ImModel
{
    /// <summary>
    /// 用户关系表（关注）
    /// </summary>
    public class UserRelation
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 被关注的用户id
        /// </summary>
        public int ToUserId { get; set; }
        /// <summary>
        /// 状态：0：删除  1:关注   2：取消关注
        /// </summary>
        public UserRelationStateEnum State { get; set; }

        public DateTime CreateTime { set; get; }
        public DateTime UpdateTime { set; get; }
    }
}
