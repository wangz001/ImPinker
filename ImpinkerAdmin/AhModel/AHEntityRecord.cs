using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AhModel
{
    public class AHEntityRecord
    {
        public int Id { set; get; }

        public int EntityType { set; get; }

        public int NewAddCount { set; get; }

        public int UpdateCount { set; get; }

        public DateTime CreateTime { get; set; }

    }
}
