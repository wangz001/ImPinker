using System;
using ImModel.Enum;

namespace ImModel
{
    public class Notify
    {
        public int Id { get; set; }
        /// <summary>
        /// 消息的内容
        /// </summary>
        public string ContentStr { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public NotifyTypeEnum NotifyType { get; set; }
        /// <summary>
        /// 目标的ID
        /// </summary>
        public int Target { get; set; }
        /// <summary>
        /// 目标类型
        /// </summary>
        public TargetTypeEnum TargetType { get; set; }
        /// <summary>
        /// 动作类型
        /// </summary>
        public ActionEnum Action { get; set; }
        /// <summary>
        /// 消息发送者
        /// </summary>
        public int Sender { get; set; }
        /// <summary>
        /// 消息接受者
        /// </summary>
        public int Receiver { get; set; }
        /// <summary>
        /// 消息是否查看
        /// </summary>
        public bool IsRead { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CrerateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpDateTime { get; set; }
    }
    /// <summary>
    /// 通知实体枚举
    /// </summary>
    public enum TargetTypeEnum
    {
        /// <summary>
        /// 文章
        /// </summary>
        Article = 1,
        /// <summary>
        /// 微博
        /// </summary>
        Weibo = 2
    }
    /// <summary>
    /// 动作类型
    /// </summary>
    public enum ActionEnum
    {
        /// <summary>
        /// 评论
        /// </summary>
        Comment = 1,
        /// <summary>
        /// 点赞
        /// </summary>
        Vote = 2,
        /// <summary>
        /// 收藏
        /// </summary>
        Collect = 3,
        /// <summary>
        /// 回复评论
        /// </summary>
        ReComment = 4
    }
}
