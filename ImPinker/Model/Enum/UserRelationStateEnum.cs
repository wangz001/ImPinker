﻿using System.ComponentModel;

namespace ImModel.Enum
{
    /// <summary>
    ///ArticleTag 表 tag前台展示状态
    /// </summary>
    public enum UserRelationStateEnum
    {
        [Description("删除")]
        Delete=0,
        [Description("关注")]
        Follow=1,
        [Description("取消关注")]
        UnFollow=2
    }
}
