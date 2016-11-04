using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using AhDal;
using AhModel;

namespace AhBll
{
    public class BasicStylePropertyValueBll
    {
        static readonly BasicStylePropertyValueDal BasicStylePropertyValueDal = new BasicStylePropertyValueDal();
        
        public static void InitWithTvp(DataTable dt)
        {
            BasicStylePropertyValueDal.InitWithTvp(dt);
        }


        public static DataSet GetPropertyValues(int basicStyleId,int companyId)
        {

            return BasicStylePropertyValueDal.GetPropertyValues(basicStyleId,companyId);
        }
    }

}
