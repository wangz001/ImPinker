using ImDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImBLL
{
    public class WeiBoBll
    {
        private WeiBoDal weiBoDal = new WeiBoDal();

        public bool AddWeiBo(ImModel.WeiBo model)
        {
            var flag=weiBoDal.AddWeiBo(model);
            return flag;
        }
    }
}
