using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel.Enum
{
    public enum NotifyTypeEnum
    {
        [Description("公告")]
        Announce=1,
        [Description("提醒")]
        Remind=2
    }
}
