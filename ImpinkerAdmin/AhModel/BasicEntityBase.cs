using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AhModel
{
    public class BasicEntityBase
    {

        public int CompanyId { get; set; }

        public int ID { set; get; }

        public string Name { set; get; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public int IsRemoved { get; set; }
    }
}
