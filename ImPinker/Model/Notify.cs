using System;
using ImModel.Enum;
using System.ComponentModel;

namespace ImModel
{
    public class Notify
    {
        public long Id { get; set; }
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
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
    /// <summary>
    /// 通知实体枚举
    /// </summary>
    public enum TargetTypeEnum
    {
        /// <summary>
        /// 文章
        /// </summary>
        [Description("文章")]
        Article = 1,
        /// <summary>
        /// 微博
        /// </summary>
        [Description("微博")]
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
        [Description("评论")]
        Comment = 1,
        /// <summary>
        /// 点赞
        /// </summary>
        [Description("点赞")]
        Vote = 2,
        /// <summary>
        /// 收藏
        /// </summary>
        [Description("收藏")]
        Collect = 3,
        /// <summary>
        /// 回复评论
        /// </summary>
        [Description("回复评论")]
        ReComment = 4
    }
}
