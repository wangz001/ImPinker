using System;
using System.Text.RegularExpressions;
using AhBll;
using AhModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetCarDataService.GetPcAutoCarsData
{
    public class GetBasicData
    {
        private const int CompanyId = (int) CompanyEnum.PCauto;

        public static void GetBrand()
        {
            const string masterUrl = "http://price.pcauto.com.cn/interface/parameter/brand_json_chooser.jsp?isShowLetter=0";
            string strResult = Common.GetResponseContent(masterUrl).Trim();
            strResult = strResult.Substring((strResult.IndexOf("{\"brands\":", StringComparison.Ordinal)) + 10); //去掉var listCompare$100=
            strResult = strResult.Substring(0, strResult.Length - 2).Trim(); //去掉分号
            var ja = (JArray)JsonConvert.DeserializeObject(strResult);
            foreach (JObject jObject in ja)
            {
                var id = jObject["id"].ToString();
                const string pattern = "^[0-9]*$";
                if (CheckRegex(pattern, id))
                {
                    var title = jObject["title"];
                    var text = jObject["text"];
                    var brand = new BasicMasterBrand { 
                        ID = Int32.Parse(id),
                        CompanyId = CompanyId, 
                        Name = title.ToString(), 
                        CreateTime = DateTime.Now, 
                        UpdateTime = DateTime.Now };
                    BasicMasterBrandBll.AddBrand(brand);
                    Console.WriteLine(text);
                    try
                    {
                        GetSerial(Int32.Parse(id));
                    }
                    catch (Exception e)
                    {
                        Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
                    }
                    
                }
            }
        }

        public static void GetSerial(int brandId)
        {
            string serialUrl = "http://price.pcauto.com.cn/interface/parameter/serial_json_chooser.jsp?brand=" + brandId;
            string strResult = Common.GetResponseContent(serialUrl).Trim();
            strResult = strResult.Substring((strResult.IndexOf("{\"firms\":", StringComparison.Ordinal)) + 9); //去掉var listCompare$100=
            strResult = strResult.Substring(0, strResult.Length - 2).Trim(); //去掉分号
            var ja = (JArray)JsonConvert.DeserializeObject(strResult);
            var makeId = 0;
            foreach (JObject jObject in ja)
            {
                var id = jObject["id"].ToString();
                var title = jObject["title"];
                var text = jObject["text"];
                if (id.StartsWith("+")) // 品牌
                {
                    makeId = Int32.Parse(id.Substring(1));
                    var make = new BasicMake
                    {
                        ID = makeId,
                        CompanyId = CompanyId,
                        Name = title.ToString(),
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    BasicMakeBll.AddMake(make);

                    var brandJoinManufacturer = new BasicMasterBrandJoinMake
                    {
                        MasterBrandId = brandId,
                        MakeId = makeId,
                        CompanyId = CompanyId,
                        IsRemoved = 0,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    BasicMakeBll.AddMasterBrandJoinMake(brandJoinManufacturer);
                }

                const string pattern = "^[0-9]*$";
                if (CheckRegex(pattern, id)) //车系
                {
                    BasicSerial serial = new BasicSerial
                    {
                        ID = Int32.Parse(id),
                        CompanyId = CompanyId,
                        Name = title.ToString(),
                        ManufacturerId = makeId,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    BasicSerialBll.AddSerial(serial);

                    var brandJoinSerial = new BasicMasterBrandJoinSerial
                    {
                        CompanyId = CompanyId,
                        MasterBrandId = brandId,
                        SerialId = Int32.Parse(id),
                        IsRemoved = 0,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };

                    BasicSerialBll.AddBrandJoinSerial(brandJoinSerial);
                    try
                    {
                        GetStyle(Int32.Parse(id));
                    }
                    catch (Exception e)
                    {
                        Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
                    }
                }
            }
        }

        //{"id":"30499","title":"2014款 2.0 TFSI 手动基本型","text":"2014款 2.0 TFSI 手动基本型","firm":"1" ,"price":"38.3万" }
        public static void GetStyle(int serialId)
        {
            string styleUrl = "http://price.pcauto.com.cn/interface/parameter/model_json_chooser.jsp?serial=" + serialId;
            string strResult = Common.GetResponseContent(styleUrl).Trim();
            strResult = strResult.Substring((strResult.IndexOf("{\"cars\":", StringComparison.Ordinal)) + 8); //去掉var listCompare$100=
            strResult = strResult.Substring(0, strResult.Length - 2).Trim(); //去掉分号
            var ja = (JArray)JsonConvert.DeserializeObject(strResult);
            foreach (JObject jObject in ja)
            {
                var id = jObject["id"].ToString();
                const string pattern = "^[0-9]*$";
                if (CheckRegex(pattern, id))
                {
                    var title = jObject["title"];
                    var text = jObject["text"];
                    var firm = jObject["firm"].ToString();
                    var price = jObject["price"].ToString();
                    var year = title.ToString().Substring(0, 4);
                    const string patternPrise = "^[0-9]*.[0-9]*万$";  //去除”暂无报价“ 等
                    if (CheckRegex(patternPrise,price))
                    {
                        price = price.Replace("万", "").Trim();
                    }
                    else
                    {
                        price = "0";
                    }
                    var style = new BasicStyle
                    {
                        ID = Int32.Parse(id),
                        CompanyId = CompanyId,
                        Name = title.ToString(),
                        SerialId = serialId,
                        SaleStatus = firm.Equals("1") ? "在销" : "停销",
                        Price = decimal.Parse(price),
                        Year = year,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    BasicStyleBll.AddStyle(style);
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
            else
            {
                return false;
            }
        }
    }
}
