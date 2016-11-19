using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AhBll;
using AhModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;

namespace GetCarDataService.GetAHAutoCarsData
{
    public class GetBasicData:IJob
    {
        private const int CompanyId = (int)CompanyEnum.AHauto;

        public void Execute(IJobExecutionContext context)
        {
            Get();
        }


        /// <summary>
        /// 添加主品牌、品牌、车系、车型。参配名称信息
        /// </summary>
        public static void Get()
        {
            try
            {
                //车型参配
                GetStyleProperty.GetProperty();
                GetMasterBrand();
                
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }
        }

        private static void GetMasterBrand()
        {
            const string uri = "http://car.autohome.com.cn/javascript/NewSpecCompare.js";
            string strResult = Common.GetResponseContent(uri).Trim();
            strResult = strResult.Substring(21); //去掉var listCompare$100=
            strResult = strResult.Substring(0, strResult.Length - 1); //去掉分号
            var ja = (JArray)JsonConvert.DeserializeObject(strResult);
            foreach (JObject jObject in ja)
            {
                HandleMasterBrand(jObject);
                var mbId = jObject["I"].ToString();
                JToken jToken = jObject["List"];
                GetMakeAndSerial(jToken, int.Parse(mbId));
            }
        }

        private static void HandleMasterBrand(JObject jObject)
        {
            var mbId = jObject["I"].ToString();
            var mbName = jObject["N"].ToString();
            //var mbInitial = jObject["L"].ToString();
            var mb = new BasicMasterBrand
            {
                CompanyId = CompanyId,
                ID = int.Parse(mbId),
                Name = mbName,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsRemoved = 0
            };
            BasicMasterBrandBll.AddBrand(mb);
            Console.WriteLine(mbName);
        }

        /// <summary>
        /// 添加厂商
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="mbId"></param>
        public static void GetMakeAndSerial(JToken jToken, int mbId)
        {
            foreach (JObject jObject in jToken)
            {
                int makeId = GetMake(mbId, jObject);
                //车系
                JToken jTokenModel = jObject["List"];
                GetSerials(mbId, makeId, jTokenModel);
            }
        }

        private static int GetMake(int masterBrandId, JObject jObject)
        {
            var makeId = jObject["I"].ToString();
            var makeName = jObject["N"].ToString();
            var make = new BasicMake
            {
                CompanyId = CompanyId,
                ID = int.Parse(makeId),
                Name = makeName,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsRemoved = 0
            };
            BasicMakeBll.AddMake(make);
            var mbJoinMake = new BasicMasterBrandJoinMake
            {
                CompanyId = CompanyId,
                MasterBrandId = masterBrandId,
                MakeId = int.Parse(makeId),
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsRemoved = 0
            };
            BasicMakeBll.AddMasterBrandJoinMake(mbJoinMake);
            return make.ID;
        }

        /// <summary>
        /// 添加车系
        /// </summary>
        /// <param name="masterBrandId"></param>
        /// <param name="manufacturerId"></param>
        /// <param name="jToken"></param>
        private static void GetSerials(int masterBrandId, int manufacturerId, IEnumerable<JToken> jToken)
        {
            foreach (JObject innJObject in jToken)
            {
                GetSerial(innJObject, masterBrandId, manufacturerId);
                var modelId = innJObject["I"].ToString();
                try
                {
                    //HandleStyles(int.Parse(modelId));
                }
                catch (Exception e)
                {
                    Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
                }
            }
        }

        private static void GetSerial(JObject innJObject, int masterBrandId, int manufacturerId)
        {
            var modelId = innJObject["I"].ToString();
            var modelName = innJObject["N"].ToString();
            var model = new BasicSerial
            {
                CompanyId = CompanyId,
                ID = int.Parse(modelId),
                ManufacturerId = manufacturerId,
                Name = modelName,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsRemoved = 0
            };
            BasicSerialBll.AddSerial(model);
            var mbJoinModel = new BasicMasterBrandJoinSerial
            {
                CompanyId = CompanyId,
                MasterBrandId = masterBrandId,
                SerialId = int.Parse(modelId),
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsRemoved = 0
            };
            BasicSerialBll.AddBrandJoinSerial(mbJoinModel);
        }

        /// <summary>
        /// 添加车型
        /// </summary>
        /// <param name="modelId"></param>
        private static void HandleStyles(int modelId)
        {
            string uri =
                string.Format(
                    "http://car.autohome.com.cn/duibi/ashx/SpecCompareHandler.ashx?type=1&seriesid={0}&isie6=0", modelId);
            string strResult = "";
            try
            {
                strResult = Common.GetResponseContent(uri);
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "根据车系ID（" + modelId + "）获取车型信息。ERROR:" + e.ToString());
            }
            if (string.IsNullOrEmpty(strResult))
            {
                return;
            }
            var ja = (JObject)JsonConvert.DeserializeObject(strResult);
            JToken jToken = ja["List"];
            foreach (JObject jObject in jToken)
            {
                var saleStatus = jObject["N"].ToString();
                JToken styleJToken = jObject["List"];
                if (styleJToken == null) continue;
                foreach (JObject styleJObject in styleJToken)
                {
                    HandleStyle(styleJObject, modelId, saleStatus);
                }
            }
        }

        private static void HandleStyle(JObject styleJObject, int modelId, string saleStatus)
        {
            string styleId = styleJObject["I"].ToString();
            string styleName = styleJObject["N"].ToString();
            string yearName = "";
            if (styleName.Length > 4)
            {
                yearName = styleName.Substring(0, 4);
            }

            string price = styleJObject["P"].ToString();
            if (price.Length > 0)
            {
                const string pattern = "^[0-9]*$";
                if (CheckRegex(pattern, price))
                {
                    //转化成单位为万
                    decimal result = Math.Round((decimal)(Double.Parse(price) / 10000), 2);
                    price = result.ToString("0.00");
                }
            }
            var style = new BasicStyle
            {
                CompanyId = CompanyId,
                ID = int.Parse(styleId),
                Name = styleName,
                SaleStatus = saleStatus,
                Price = decimal.Parse(price),
                SerialId = modelId,
                Year = yearName,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsRemoved = 0
            };
            BasicStyleBll.AddStyle(style);
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
