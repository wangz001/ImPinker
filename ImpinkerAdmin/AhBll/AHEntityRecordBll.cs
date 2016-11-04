using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AhDal;
using AhModel;

namespace AhBll
{
    public class AHEntityRecordBll
    {
        public static AHEntityRecordDal AhEntityRecordDal=new AHEntityRecordDal();

        public static AHEntityRecord GetAhEntityRecord(int entityType, DateTime updateTime)
        {
            return AhEntityRecordDal.GetAhEntityRecord(entityType,updateTime);
        }

        public static List<AHEntityRecord> GetModels(int entityType, int year, int month)
        {
            return AhEntityRecordDal.GetModels(entityType, year, month);
        }
    }
}
