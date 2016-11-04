using System;
using AhDal;
using AhModel;

namespace GetCarDataService.AutoHomeCountTask
{
    public class AddAndUpdateStatist
    {
        static readonly AhModelDal ModelDal=new AhModelDal();
        static readonly AhStyleDal AHStyleDal=new AhStyleDal();
        static readonly AHEntityRecordDal EntityRecordDal=new AHEntityRecordDal();

        public static void Stat()
        {
            StatModels();
            StatStyles();
            //记录日志
        }

        private static void StatModels()
        {
            int countAdd = CountAddModels();
            int countUpdate = CountUpdateModels();
            var record=new AHEntityRecord
            {
                EntityType = (int) AHEntityTypeEnum.Model,
                NewAddCount = countAdd,
                UpdateCount = countUpdate,
                CreateTime = DateTime.Now
            };
            var model = EntityRecordDal.GetAhEntityRecord(record.EntityType, DateTime.Now);
            if (model==null)  //保证每天只产生一条记录
            {
                EntityRecordDal.Add(record); 
            }
        }

        private static void StatStyles()
        {
            int countAdd = CountAddStyles();
            int countUpdate = CountUpdateStyles();
            var record = new AHEntityRecord
            {
                EntityType = (int)AHEntityTypeEnum.Sytle,
                NewAddCount = countAdd,
                UpdateCount = countUpdate,
                CreateTime = DateTime.Now
            };
            var model = EntityRecordDal.GetAhEntityRecord(record.EntityType, DateTime.Now);
            if (model == null)  //保证每天只产生一条记录
            {
                EntityRecordDal.Add(record);
            }
        }

        private static int CountUpdateStyles()
        {
            var yesterday = DateTime.Now.AddDays(-1);  //从当天0点开始计算
            int count = AHStyleDal.CountUpdateEveryday(yesterday);
            return count;
        }

        private static int CountAddStyles()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            int count = AHStyleDal.CountAddEveryday(yesterday);
            return count;
        }

        private static int CountAddModels()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            int count=ModelDal.CountAddEveryday(yesterday);
            return count;
        }

        private static int CountUpdateModels()
        {
            var yesterday = DateTime.Now.AddDays(-1);
            int count = ModelDal.CountUpdateEveryday(yesterday);
            return count;
        }

    }
}
