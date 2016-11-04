using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AhModel
{
    public class BasicMasterBrandJoinMake : BasicEntityBase
    {
        public int MasterBrandId { set; get; }

        public int MakeId { set; get; }
    }
}
