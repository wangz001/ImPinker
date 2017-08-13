using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel.ViewModel
{
    public class NotifyVm:Notify
    {
        public string SenderName { get; set; }

        public string ActionName { get; set; }

        public string TargetName { get; set; }

        public string TargetTypeName { get; set; }

    }
}
