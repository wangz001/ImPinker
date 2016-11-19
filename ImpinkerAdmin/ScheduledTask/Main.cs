using System;
using Bita.Common;
using GetCarDataService.AutoHomeCountTask;
using GetCarDataService.GetAHAutoCarsData;
using GetCarDataService.GetXCAutoCarsData;
using GetCarDataService.ImArticleFirstImage;
using GetCarDataService.SolrDicGenerate;
using GetBasicData = GetCarDataService.GetXCAutoCarsData.GetBasicData;
using GetStylePropertyValues = GetCarDataService.GetXCAutoCarsData.GetStylePropertyValues;

namespace GetCarDataService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //GetAHAutoCarsData.GetBasicData.Get();
            new GenerateCarDataDic().Execute(null);

            log4net.Config.XmlConfigurator.Configure();
            new QuartzMain().Run();
            Console.ReadLine();
            //生成article封面图计划任务
            ArticleFirstImageUpload.Start();
            Console.WriteLine("检查并上传oss图片结束");
            Console.ReadLine();

            
            Console.WriteLine("汽车之家数据获取完毕");
            Console.ReadLine();
            //SwitchCompany();

            # region 2014-10-20计划任务配置
            try
            {
                if (args.Length > 0)
                {
                    if (args[0].ToLower().Equals("everyday"))
                    {
                        StartEvervyDay();
                        StartEveryDay1();
                    }
                    if (args[0].ToLower().Equals("everyweek"))
                    {
                        StartEveryWeek();
                    }
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }

            #endregion
        }

        private static void SwitchCompany()
        {
            while (true)
            {
                Console.WriteLine("请选择编号");
                Console.WriteLine("1：太平洋汽车");
                Console.WriteLine("2：腾讯汽车");
                Console.WriteLine("3：搜狐汽车");
                Console.WriteLine("4：爱卡汽车");
                Console.WriteLine("5：汽车之家");
                string idstr = Console.ReadLine();
                int id = Int32.Parse(string.IsNullOrEmpty(idstr) ? "100" : idstr);
                switch (id)
                {
                    case 1:
                        TestPcAuto();
                        break;
                    case 2:
                        TestQQAuto();
                        break;
                    case 3:
                        TestSHAuto();
                        break;
                    case 4:
                        TestXCAuto();
                        break;
                    case 5: TestAHAuto();
                        break;
                    default:
                        Console.WriteLine("输入错误，请重新输入");
                        break;
                }
            }
        }

        private static void TestPcAuto()
        {
            while (true)
            {
                Console.WriteLine("请选择操作");
                Console.WriteLine("1：初始化品牌、车系、年款、车型信息");
                Console.WriteLine("2：初始化车型参配类型信息");
                Console.WriteLine("3：初始化车型参配值信息（该操作比较耗时）");
                Console.WriteLine("0：返回上一级");
                string idstr = Console.ReadLine();
                int id = Int32.Parse(string.IsNullOrEmpty(idstr) ? "100" : idstr);
                switch (id)
                {
                    case 0: SwitchCompany();
                        break;
                    case 1: GetPcAutoCarsData.GetBasicData.GetBrand();
                        break;
                    case 2: GetPcAutoCarsData.GetProperty.GetStyleProperty();
                        break;
                    case 3: GetPcAutoCarsData.GetStylePropertyValues.Start();
                        break;
                }
            }
        }

        private static void TestQQAuto()
        {
            while (true)
            {
                Console.WriteLine("请选择操作");
                Console.WriteLine("1：初始化品牌、车系、年款、车型信息");
                Console.WriteLine("2：初始化车型参配类型信息");
                Console.WriteLine("3：初始化车型参配值信息（该操作比较耗时）");
                Console.WriteLine("0：返回上一级");
                string idstr = Console.ReadLine();
                int id = Int32.Parse(string.IsNullOrEmpty(idstr) ? "100" : idstr);
                switch (id)
                {
                    case 0: SwitchCompany();
                        break;
                    case 1: GetQQAutoCarsData.GetBasicData.GetBrand();
                        break;
                    case 2: GetQQAutoCarsData.GetProperty.GetStyleProperty();
                        break;
                    case 3: GetQQAutoCarsData.GetStylePropertyValues.Start();
                        break;
                }
            }
        }

        private static void TestSHAuto()
        {
            while (true)
            {
                Console.WriteLine("请选择操作");
                Console.WriteLine("1：初始化品牌、车系、年款、车型信息");
                Console.WriteLine("2：初始化车型参配类型信息");
                Console.WriteLine("3：初始化车型参配值信息（该操作比较耗时）");
                Console.WriteLine("0：返回上一级");
                string idstr = Console.ReadLine();
                int id = Int32.Parse(string.IsNullOrEmpty(idstr) ? "100" : idstr);
                switch (id)
                {
                    case 0: SwitchCompany();
                        break;
                    case 1: GetSHAutoCarsData.GetBasicData.GetBrand();
                        break;
                    case 2: GetSHAutoCarsData.GetProperty.GetStyleProperty();
                        break;
                    case 3:
                        GetSHAutoCarsData.GetStylePropertyValues.Start();
                        break;
                }
            }
        }

        private static void TestXCAuto()
        {
            while (true)
            {
                Console.WriteLine("请选择操作");
                Console.WriteLine("1：初始化品牌、车系、年款、车型信息");
                Console.WriteLine("2：初始化车型参配类型信息");
                Console.WriteLine("3：初始化车型参配值信息（该操作比较耗时）");
                Console.WriteLine("0：返回上一级");
                string idstr = Console.ReadLine();
                int id = Int32.Parse(string.IsNullOrEmpty(idstr) ? "100" : idstr);
                switch (id)
                {
                    case 0: SwitchCompany();
                        break;
                    case 1: GetBasicData.Get();
                        break;
                    case 2: GetProperty.GetStyleProperty();
                        break;
                    case 3: GetStylePropertyValues.Start();
                        break;
                }
            }
        }

        private static void TestAHAuto()
        {
            while (true)
            {
                Console.WriteLine("请选择操作");
                Console.WriteLine("1：初始化品牌、车系、年款、车型信息");
                Console.WriteLine("2：初始化车型参配类型信息");
                Console.WriteLine("3：初始化车型参配值信息（该操作比较耗时）");
                Console.WriteLine("0：返回上一级");
                string idstr = Console.ReadLine();
                int id = Int32.Parse(string.IsNullOrEmpty(idstr) ? "100" : idstr);
                switch (id)
                {
                    case 0: SwitchCompany();
                        break;
                    case 1: GetAHAutoCarsData.GetBasicData.Get();
                        break;
                    case 2: GetStyleProperty.GetProperty();
                        break;
                    case 3: GetAHAutoCarsData.GetStylePropertyValues.Start();
                        break;
                }
            }
        }



        /// <summary>
        /// 每天执行一次的计划任务
        /// </summary>
        private static void StartEvervyDay()
        {
            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "开始->③统计汽车之家每天新增和修改（车系、车型）");
            AddAndUpdateStatist.Stat();
            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "结束->③统计汽车之家每天新增和修改（车系、车型）");
            
            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "开始->⑥获取汽车之家主品牌、品牌、车系、车型信息");
            GetBasicData.Get();
            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "结束->⑥获取汽车之家主品牌、品牌、车系、车型信息");
        }

        /// <summary>①②③④⑤⑥⑦⑧⑨⑩
        /// 每周执行一次的计划任务
        /// </summary>
        private static void StartEveryWeek()
        {
            //2014-10-20
            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "1->获取汽车之家车型参配值");
            GetStyleProperty.GetProperty();
            GetAHAutoCarsData.GetStylePropertyValues.Start();

            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "2->获取太平洋车型参配值");
            GetPcAutoCarsData.GetProperty.GetStyleProperty();
            GetPcAutoCarsData.GetStylePropertyValues.Start();

            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "3->获取腾讯车型参配值");
            GetQQAutoCarsData.GetProperty.GetStyleProperty();
            GetQQAutoCarsData.GetStylePropertyValues.Start();

            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "4->获取搜狐车型参配值");
            GetSHAutoCarsData.GetProperty.GetStyleProperty();
            GetSHAutoCarsData.GetStylePropertyValues.Start();

            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "5->获取爱卡车型参配值");
            GetProperty.GetStyleProperty();
            GetStylePropertyValues.Start();

            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "6->获取车型参配值结束");
        }

        //获取5家竞品的主品牌、品牌、车系、车型数据
        private static void StartEveryDay1()
        {
            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "1->获取太平洋车型数据");
            try
            {
                GetPcAutoCarsData.GetBasicData.GetBrand();
                GetPcAutoCarsData.GetProperty.GetStyleProperty();
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }

            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "2->获取腾讯车型数据");
            try
            {
                GetQQAutoCarsData.GetBasicData.GetBrand();
                GetQQAutoCarsData.GetProperty.GetStyleProperty();
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }
            try
            {
                Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "3->获取搜狐车型数据");
                GetSHAutoCarsData.GetBasicData.GetBrand();
                GetSHAutoCarsData.GetProperty.GetStyleProperty();
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }

            try
            {
                Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "4->获取汽车之家车型数据");
                GetAHAutoCarsData.GetBasicData.Get();
                GetStyleProperty.GetProperty();
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }

            try
            {
                Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "5->获取爱卡车型数据");
                GetBasicData.Get();
                GetProperty.GetStyleProperty();
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }

            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "6->获取全部车型数据结束");
        }
    }
}
