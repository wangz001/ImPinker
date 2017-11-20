using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel.Enum
{
    /// <summary>
    ///ArticleTag 表 tag前台展示状态
    /// </summary>
    public enum UserCollectionStateEnum
    {
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete=0,
        /// <summary>
        /// 收藏
        /// </summary>
        [Description("收藏")]
        Collect=1,
        /// <summary>
        /// 取消收藏
        /// </summary>
        [Description("取消收藏")]
        UnCollect=2
    }
}
