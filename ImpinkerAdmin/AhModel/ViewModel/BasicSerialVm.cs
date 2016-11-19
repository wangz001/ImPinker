using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AhModel.ViewModel
{
    public class BasicSerialVm:BasicSerial
    {
        public string MakeName { get; set; }

        public int MasterBrandId { get; set; }

        public string MasterBrandName { get; set; }
    }
}
