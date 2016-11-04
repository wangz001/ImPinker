using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AhModel
{
    public class BasicStylePropertyValue : BasicEntityBase
    {

        public int PropertyId { get; set; }

        public int StyleId { get; set; }

        public string Value { get; set; }

    }
}
