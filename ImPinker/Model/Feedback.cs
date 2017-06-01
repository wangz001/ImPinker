using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel
{
    public class Feedback
    {
        public int Id { get; set; }

        /// <summary>
        /// 用户id 或者手机型号码等能证明用户身份的东西
        /// </summary>
        public string UserIdentity { get; set; }

        /// <summary>
        /// 联系方式
        /// </summary>
        public string ContactWay { get; set; }
        /// <summary>
        /// 问题描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 问题截图等
        /// </summary>
        public string ContentStr { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public FeedBackStateEnum FeedBackState { get; set; }
    }

    public enum FeedBackStateEnum
    {
        /// <summary>
        /// 已删除
        /// </summary>
        Deleted=0,
        /// <summary>
        /// 待处理
        /// </summary>
        BeSolve=1,
        /// <summary>
        /// 处理中
        /// </summary>
        Solving=2,
        /// <summary>
        /// 已处理
        /// </summary>
        Solved=3
    }
}
