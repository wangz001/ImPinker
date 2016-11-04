using System;
using AhDal;
using AhModel;

namespace GetCarDataService.AutoHomeCountTask
{

    /// <summary>
    /// 通过判断车型参配值是否有更新的，以此判断车型是否更新（更新完车型参配值后执行）
    /// </summary>
    class CheckStyleUpdateByPropertyValue
    {
        static readonly AhStyleDal AHStyleDal = new AhStyleDal();
        static readonly AhStylePropertyValueDal AhStylePropertyValueDal = new AhStylePropertyValueDal();


        public static int Check()
        {
            Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "开始(线程内)->⑨汽车之家，根据车型参配值是否更新判断车型是否更新");

            int count = 0;
            var allAhStyles = AHStyleDal.GetLists("");
            foreach (AhStyle style in allAhStyles)
            {
                var time = DateTime.Now;
                var exists = AhStylePropertyValueDal.CheckStyleUpdate(style.ID, time);

                if (exists)
                {
                    AHStyleDal.ReplaceUpdateTime(style.ID, DateTime.Now);
                    count++;
                }
            }
            var str = string.Format("检查完毕,本次共有{0}个车型更新", count);
            Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + str);
            Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "结束（线程内）->⑨汽车之家，根据车型参配值是否更新判断车型是否更新");
            return count;
        }
    }
}
