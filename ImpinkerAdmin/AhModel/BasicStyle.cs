using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AhModel
{
    public class BasicStyle:BasicEntityBase
    {
        public int SerialId { get; set; }

        public string Year { get; set; }

        public decimal Price { get; set; }

        public string SaleStatus { get; set; }

    }
}
