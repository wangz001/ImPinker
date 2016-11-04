using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using AhBll;
using AhModel;
using GetCarDataService.GetCarsDataBll;
using HtmlAgilityPack;

namespace GetCarDataService.GetXCAutoCarsData
{
    /// <summary>
    /// 爱卡汽车
    /// </summary>
    public class GetBasicStyle
    {
        private const int CompanyId = (int)CompanyEnum.XCauto;
        const string Pattern = "^[0-9]*.[0-9]*$";  //车型售价
        private static List<int> _allSerialIds;

        static readonly WaitHandle[] WaitHandles =
        {
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false)
        };

        /// <summary>
        /// 采用多线程方式获取车型信息
        /// </summary>
        public static void Get()
        {
            _allSerialIds = BasicSerialBll.GetSerial(CompanyId).Select(m => m.ID).ToList();

            int count = _allSerialIds.Count();
            int dataCount = count / 4;
            int remainder = count % 4;
            for (int i = 1; i <= 5; i++) //定为5个线程异步请求数据并处理
            {
                int startIndex;
                int endIndex;
                if (i == 5)
                {
                    startIndex = (i - 1) * dataCount;
                    endIndex = (i - 1) * dataCount + remainder - 1;
                }
                else
                {
                    startIndex = (i - 1) * dataCount;
                    endIndex = i * dataCount - 1;
                }

                var obj = new WaitHandleObject
                {
                    StartIndex = startIndex,
                    EndIndex = endIndex,
                    WaitHandle = WaitHandles[i - 1]
                };
                ThreadPool.QueueUserWorkItem(GetDataM, obj);
            }
            WaitHandle.WaitAll(WaitHandles);
            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "-->获取爱卡车型数据结束");
        }

        private static void GetDataM(object args)
        {
            var obj = args as WaitHandleObject;
            if (obj == null)
                return;
            var are = (AutoResetEvent)obj.WaitHandle;

            for (int i = obj.StartIndex; i <= obj.EndIndex; i++)
            {
                int serialId = _allSerialIds[i];
                try
                {
                    RequestUrl(serialId);
                    Console.WriteLine(serialId);
                }
                catch (Exception e)
                {
                    Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + e.ToString());
                }
            }
            Console.WriteLine("线程{0}执行完毕", obj.WaitHandle);
            are.Set();
        }

        private static void RequestUrl(int serialId)
        {
            try
            {
                string serialUrl = string.Format("http://newcar.xcar.com.cn/{0}/", serialId);
                HtmlDocument strResult = Common.GetHtmlDocument(serialUrl);
                if (strResult != null)
                {
                    var years = GetYears(strResult);
                    if (years.Count > 0)
                    {
                        foreach (var year in years)
                        {
                            GetStyle(serialId, year);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("该车系不存在或未上市");
            }
        }

        private static List<int> GetYears(HtmlDocument document)
        {
            var years = new List<int>();
            var div = document.DocumentNode.SelectNodes("//div[starts-with(@class,'t612_brand')]")[0];
            var ems = div.SelectNodes("child::em");
            if (ems != null)
            {
                var aa = ems[0].SelectNodes("child::a");
                foreach (var a in aa)
                {
                    var href = a.GetAttributeValue("href", "");
                    if (href != null && href.Length > 5)
                    {
                        var year = href.Substring(href.Length - 5, 4);
                        years.Add(Int32.Parse(year));
                    }
                }
            }
            var dls = div.SelectNodes("child::dl");
            if (dls != null)
            {
                var dds = dls[0].SelectNodes("child::dd");
                if (dds != null)
                {
                    var aa = dds[0].SelectNodes("child::a");
                    foreach (var a in aa)
                    {
                        var href = a.GetAttributeValue("href", "");
                        if (href != null && href.Length > 5)
                        {
                            var year = href.Substring(href.Length - 5, 4);
                            years.Add(Int32.Parse(year));
                        }
                    }
                }
            }
            return years;
        }

        public static void GetStyle(int serialId, int year)
        {
            string yearUrl = string.Format("http://newcar.xcar.com.cn/{0}/{1}/", serialId, year);
            HtmlDocument strResult = Common.GetHtmlDocument(yearUrl);
            var div = strResult.DocumentNode.SelectNodes("//div[starts-with(@class,'cx_quote')]");
            if (div != null)
            {
                var tables = div[0].SelectNodes("child::table");
                if (tables != null)
                {
                    foreach (var tbody in tables)
                    {
                        var trs = tbody.SelectNodes("child::tr");
                        foreach (var tr in trs)
                        {
                            var tds = tr.SelectNodes("child::td");
                            if (tds != null)
                            {
                                var href = tds[0].FirstChild.GetAttributeValue("href", "");
                                var id = href.Substring(2);
                                id = id.Substring(0, id.Length - 1);
                                var name = tds[0].InnerText;
                                var price = tds[1].InnerText.Trim();
                                price = price.Replace("万", "");
                                if (!CheckRegex(Pattern, price))
                                {
                                    price = "0";
                                }
                                var style = new BasicStyle
                                {
                                    ID = Int32.Parse(id),
                                    CompanyId = CompanyId,
                                    Name = name,
                                    SerialId = serialId,
                                    SaleStatus = "",
                                    Price = decimal.Parse(price),
                                    Year = year.ToString(CultureInfo.InvariantCulture),
                                    CreateTime = DateTime.Now,
                                    UpdateTime = DateTime.Now
                                };
                                BasicStyleBll.AddStyle(style);
                            }
                        }
                    }
                }
            }
        }


        public static bool CheckRegex(string pattern, string value)
        {
            var regex = new Regex(pattern);
            if (regex.IsMatch(value))
            {
                return true;
            }
            return false;
        }
    }
}
