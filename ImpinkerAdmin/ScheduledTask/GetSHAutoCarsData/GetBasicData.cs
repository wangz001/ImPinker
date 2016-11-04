using System;
using System.Text.RegularExpressions;
using AhBll;
using AhModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetCarDataService.GetSHAutoCarsData
{
    /// <summary>
    /// 搜狐汽车
    /// </summary>
    public class GetBasicData
    {
        private const int CompanyId = (int) CompanyEnum.SHauto;

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
            const string masterUrl = "http://db.auto.sohu.com/attachment/js/new_model.js";
            string strResult = Common.GetResponseContent(masterUrl).Trim();
            strResult = strResult.Substring((strResult.IndexOf("brandMods =", StringComparison.Ordinal)) + 11);
            //strResult = strResult.Substring(0, strResult.Length - 2).Trim();
            var ja = (JArray)JsonConvert.DeserializeObject(strResult);
            foreach (JObject jObject in ja)
            {
                var id = jObject["i"].ToString();
                const string pattern = "^[0-9]*$";
                if (CheckRegex(pattern, id))
                {
                    var name = jObject["n"];
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
                    JToken jToken = jObject["s"];
                    GetManufacturer(Int32.Parse(id), jToken);
                }
            }
        }

        public static void GetManufacturer(int brandId, JToken jToken)
        {
            foreach (JObject jObject in jToken)
            {
                var makeName = jObject["n"].ToString();
                JToken jTokenSerial = jObject["b"];

                var makeId = BasicMakeBll.GetShMakeId(makeName, CompanyId);
                var model = new BasicMake
                {
                    ID = makeId,
                    Name = makeName,
                    CompanyId = CompanyId,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                BasicMakeBll.AddMake(model);
                var brandJoinMake = new BasicMasterBrandJoinMake
                {
                    MasterBrandId = brandId,
                    MakeId = makeId,
                    CompanyId = CompanyId,
                    IsRemoved = 0,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                BasicMakeBll.AddMasterBrandJoinMake(brandJoinMake);
                GetSerial(brandId, makeId, jTokenSerial);
            }
        }

        public static void GetSerial(int brandId, int makeId, JToken jToken)
        {
            foreach (JObject jObject in jToken)
            {
                var serialName = jObject["n"].ToString().Trim();
                var serialId = jObject["i"].ToString();
                const string pattern = "^[0-9]*$";
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
                        MasterBrandId = brandId,
                        SerialId = Int32.Parse(serialId),
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        IsRemoved = 0
                    };
                    BasicSerialBll.AddBrandJoinSerial(brandJoinSerial);
                    try
                    {
                        GetStyle(Int32.Parse(serialId));
                    }
                    catch (Exception e)
                    {
                        Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
                    }
                }
            }
        }

        public static void GetStyle(int serialId)
        {
            string styleUrl = string.Format("http://db.auto.sohu.com/api/model/select/trims_{0}.js", serialId);
            string strResult = Common.GetResponseContent(styleUrl).Trim();
            strResult = strResult.Substring((strResult.IndexOf("datas =", StringComparison.Ordinal)) + 7);
            var ja = (JArray)JsonConvert.DeserializeObject(strResult);
            if (ja.Count == 0)
            {
                return;
            }
            JToken jToken = ja[0]["trimyears"];
            foreach (JObject jObject in jToken)
            {
                var year = jObject["y"].ToString();
                JToken jlists = jObject["trims"];
                foreach (var jlist in jlists)
                {
                    var id = jlist["tid"].ToString();
                    const string pattern = "^[0-9]*$";
                    if (CheckRegex(pattern, id))
                    {
                        var name = jlist["tname"];
                        var proState = jlist["status"].ToString() == "1" ? "在销" : "停销";
                        var price = jlist["price"].ToString();
                        year = year.Substring(0, 4);
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
