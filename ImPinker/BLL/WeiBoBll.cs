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

        public long AddWeiBo(ImModel.WeiBo model)
        {
            long id=weiBoDal.AddWeiBo(model);
            return id;
        }
    }
}
