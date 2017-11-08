using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel.ViewModel
{
    public class ArticleWeiboVm
    {
        public int EntityId { get; set; }

        public int EntityType { get; set; }

        public int Userid { get; set; }

        public DateTime CreateTime { get; set; }

        public ArticleViewModel ArticleVm { get; set; }

        public WeiboVm WeiboVm { get; set; }
    }
}
