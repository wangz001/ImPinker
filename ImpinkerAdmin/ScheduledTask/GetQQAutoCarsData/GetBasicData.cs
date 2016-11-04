using System;
using System.Text.RegularExpressions;
using AhBll;
using AhModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetCarDataService.GetQQAutoCarsData
{
    public class GetBasicData
    {
        private const int CompanyId = (int)CompanyEnum.QQauto;

        public static void GetBrand()
        {
            try
            {
                GetMasterBrand();
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }

        }

        private static void GetMasterBrand()
        {
            const string masterUrl = "http://js.data.auto.qq.com/car_public/1/manufacturer_list_json.js?_=1407146214055";
            string strResult = Common.GetResponseContent(masterUrl).Trim();
            strResult = strResult.Substring((strResult.IndexOf("arrManufacturer :", StringComparison.Ordinal)) + 17);
            strResult = strResult.Substring(0, strResult.Length - 2).Trim();
            strResult = strResult.Substring(0, (strResult.LastIndexOf("}]") + 2));
            var ja = (JArray)JsonConvert.DeserializeObject(strResult);
            foreach (JObject jObject in ja)
            {
                var id = jObject["ID"].ToString();
                const string pattern = "^[0-9]*$";
                if (CheckRegex(pattern, id))
                {
                    var name = jObject["Name"];
                    var brand = new BasicMasterBrand
                    {
                        ID = Int32.Parse(id),
                        CompanyId = CompanyId,
                        Name = name.ToString(),
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    BasicMasterBrandBll.AddBrand(brand);
                    Console.WriteLine(name);
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

        private static void GetSerial(int masterBrandId)
        {
            string serialUrl = string.Format("http://js.data.auto.qq.com/car_manufacturer/{0}/serial_list_json.js", masterBrandId);
            string strResult = Common.GetResponseContent(serialUrl).Trim();
            strResult = strResult.Substring((strResult.IndexOf("arrSerial :", StringComparison.Ordinal)) + 11).Trim();
            strResult = strResult.Substring(0, (strResult.LastIndexOf("]") + 1)).Trim();
            var ja = (JArray)JsonConvert.DeserializeObject(strResult);
            var makeId = 0;
            foreach (JObject jObject in ja)
            {
                var serialId = jObject["ID"].ToString();
                var serialName = jObject["Name"].ToString().Trim();
                var brandId = jObject["BrandId"].ToString();
                var brandName = jObject["BrandName"];

                const string pattern = "^[0-9]*$";
                if (CheckRegex(pattern, brandId)) //品牌
                {
                    makeId = Int32.Parse(brandId);
                    var make = new BasicMake
                    {
                        ID = makeId,
                        CompanyId = CompanyId,
                        Name = brandName.ToString(),
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    BasicMakeBll.AddMake(make);

                    var brandJoinMake = new BasicMasterBrandJoinMake
                    {
                        MasterBrandId = masterBrandId,
                        MakeId = makeId,
                        CompanyId = CompanyId,
                        IsRemoved = 0,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };

                    BasicMakeBll.AddMasterBrandJoinMake(brandJoinMake);
                }
                if (CheckRegex(pattern, serialId)) //车系
                {
                    var serial = new BasicSerial
                    {
                        ID = Int32.Parse(serialId),
                        CompanyId = CompanyId,
                        Name = serialName,
                        ManufacturerId = makeId,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    BasicSerialBll.AddSerial(serial);

                    var brandJoinSerial = new BasicMasterBrandJoinSerial
                    {
                        CompanyId = CompanyId,
                        MasterBrandId = masterBrandId,
                        SerialId = Int32.Parse(serialId),
                        IsRemoved = 0,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    BasicSerialBll.AddBrandJoinSerial(brandJoinSerial);
                    try
                    {
                        GetStyle(Int32.Parse(serialId));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]抓取腾讯车型出错：" + e.ToString());
                    }
                }
            }
        }

        private static void GetStyle(int serialId)
        {
            string styleUrl = string.Format("http://js.data.auto.qq.com/car_serial/{0}/model_list.js", serialId);
            string strResult = Common.GetResponseContent(styleUrl).Trim();
            strResult = strResult.Substring((strResult.IndexOf("arrModel :", StringComparison.Ordinal)) + 10).Trim();
            strResult = strResult.Substring(0, strResult.LastIndexOf("]")+1).Trim();
            var ja = (JArray)JsonConvert.DeserializeObject(strResult);
            foreach (JObject jObject in ja)
            {
                var id = jObject["ID"].ToString();
                const string pattern = "^[0-9]*$";
                if (CheckRegex(pattern, id))
                {
                    var name = jObject["Name"];
                    var proState = jObject["ProState"].ToString();
                    var price = jObject["Price"].ToString();
                    var year = jObject["Year"].ToString().Substring(0, 4);
                    const string patternPrise = "^[0-9]*.[0-9]*$";  //去除”暂无报价“ 等
                    if (!CheckRegex(patternPrise, price))
                    {
                        price = "0";
                    }
                    var style = new BasicStyle
                    {
                        ID = Int32.Parse(id),
                        CompanyId = CompanyId,
                        Name = name.ToString(),
                        SerialId = serialId,
                        SaleStatus = proState,
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
            return false;
        }
    }
}
