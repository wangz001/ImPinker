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
    public enum FrountShowStateEnum
    {
        [Description("待审核")]
        Default=0,
        [Description("前台可显示")]
        FrountShow=1,
        [Description("禁止显示")]
        NotShow=2
    }
}
