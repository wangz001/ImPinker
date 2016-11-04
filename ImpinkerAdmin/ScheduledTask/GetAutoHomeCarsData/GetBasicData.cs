using System;
using System.Collections.Generic;
using System.Linq;
using AhDal;
using AhModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetCarDataService.GetAutoHomeCarsData
{
    internal class GetBasicData
    {
        static readonly AhMasterBrandDal AhMbdDal = new AhMasterBrandDal();
        static readonly AHManufacturerDal AhManufacturerDal = new AHManufacturerDal();
        static readonly AHMfJoinMbDal AHMfJoinMbDal = new AHMfJoinMbDal();
        static readonly AhModelDal AhModelDal = new AhModelDal();
        static readonly AhStyleDal AhStyleDal = new AhStyleDal();

        private static List<AhMasterBrand> _listMasterBrands;
        private static List<AhManufacturer> _listManufacturers;
        private static List<AHModel> _listModels;
        private static List<AhStyle> _listsStyles;

        /// <summary>
        /// 添加主品牌
        /// </summary>
        public static void Get()
        {
            InitLists();

            const string uri = "http://car.autohome.com.cn/javascript/NewSpecCompare.js";
            string strResult = Common.GetResponseContent(uri).Trim();
            strResult = strResult.Substring(21); //去掉var listCompare$100=
            strResult = strResult.Substring(0, strResult.Length - 1); //去掉分号
            var ja = (JArray)JsonConvert.DeserializeObject(strResult);
            foreach (JObject jObject in ja)
            {
                HandleMasterBrand(jObject);
                //品牌
                var mbId = jObject["I"].ToString();
                JToken jToken = jObject["List"];
                HaldleManufacturers(jToken, int.Parse(mbId));
                Console.WriteLine("主品牌：" + mbId);
            }

            //DeleteEntities();
        }

        private static void InitLists() //实体集合此处作用是，检查是否有删除的数据
        {
            _listMasterBrands = AhMbdDal.GetAllMasterBrands();
            _listManufacturers = AhManufacturerDal.GetAllAhManufacturers();
            _listModels = AhModelDal.GetAllAhModels();
            _listsStyles = AhStyleDal.GetLists("");
        }


        private static void HandleMasterBrand(JObject jObject)
        {
            var mbId = jObject["I"].ToString();
            var mbName = jObject["N"].ToString();
            var mbInitial = jObject["L"].ToString();

            var mb = new AhMasterBrand();
            mb.ID = int.Parse(mbId);
            mb.MasterBrandName = mbName;
            mb.Initial = mbInitial;
            mb.CreateTime = DateTime.Now;
            mb.UpdateTime = DateTime.Now;
            mb.IsRemoved = 0;
            try
            {
                if (AhMbdDal.IsExit(mb))
                {
                    if (AhMbdDal.NeedUpdate(mb))
                    {
                        AhMbdDal.Update(mb);
                    }
                }
                else
                {
                    AhMbdDal.Insert(mb);
                }
            }
            catch (Exception exception)
            {
                _listMasterBrands.Remove(_listMasterBrands.FirstOrDefault(m => m.ID == mb.ID));
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "主品牌出错:" + exception.ToString());
            }
            _listMasterBrands.Remove(_listMasterBrands.FirstOrDefault(m => m.ID == mb.ID));
        }

        /// <summary>
        /// 添加厂商
        /// </summary>
        /// <param name="jToken"></param>
        /// <param name="mbId"></param>
        public static void HaldleManufacturers(JToken jToken, int mbId)
        {
            foreach (JObject jObject in jToken)
            {
                int manufacturerId = HandleManufacturer(jObject);
                HandleMbJoinMf(mbId,manufacturerId);
                //车系
                JToken jTokenModel = jObject["List"];
                GetModels(mbId,manufacturerId, jTokenModel);
            }
        }

        private static int HandleManufacturer(JObject jObject)
        {

            var manufacturerId = jObject["I"].ToString();
            var manufacturerName = jObject["N"].ToString();

            var manufacturer = new AhManufacturer
            {
                ID = int.Parse(manufacturerId),
                Initial = Common.GetSpell(manufacturerName) ?? "-",
                ManufacturerName = manufacturerName,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsRemoved = 0
            };
            try
            {
                if (AhManufacturerDal.Exists(manufacturer.ID))
                {
                    if (AhManufacturerDal.NeedUpdate(manufacturer))
                    {
                        AhManufacturerDal.Update(manufacturer);
                        Console.WriteLine("修改厂商{0}", manufacturer.ManufacturerName);
                    }
                }
                else
                {
                    AhManufacturerDal.Insert(manufacturer);
                    Console.WriteLine("添加厂商{0}", manufacturer.ManufacturerName);
                }

            }
            catch (Exception e)
            {
                _listManufacturers.Remove(_listManufacturers.FirstOrDefault(m => m.ID == manufacturer.ID));
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "ERROR:" + e.ToString());
            }
            _listManufacturers.Remove(_listManufacturers.FirstOrDefault(m => m.ID == manufacturer.ID));
            return manufacturer.ID;
        }

        private static void HandleMbJoinMf(int masterBrandId,int manufacturerId)
        {
            var mfJoinMb = new AHMfJoinMb();

            mfJoinMb.ManufacturerID =manufacturerId;
            mfJoinMb.MasterBrandID = masterBrandId;
            mfJoinMb.CreateTime = DateTime.Now;
            mfJoinMb.UpdateTime = DateTime.Now;
            try
            {
                if (!AHMfJoinMbDal.Exists(mfJoinMb.MasterBrandID, mfJoinMb.ManufacturerID))
                {
                    AHMfJoinMbDal.Insert(mfJoinMb);
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "ERROR:" + e.ToString());
            }
        }


        /// <summary>
        /// 添加车系
        /// </summary>
        /// <param name="masterBrandId"></param>
        /// <param name="jToken"></param>
        private static void GetModels(int masterBrandId,int manufacturerId, JToken jToken)
        {
            foreach (JObject innJObject in jToken)
            {
                HandleModel(innJObject, masterBrandId,manufacturerId);
                var modelId = innJObject["I"].ToString();
                HandleStyles(int.Parse(modelId));
            }
        }

        private static void HandleModel(JObject innJObject, int masterBrandId,int manufacturerId)
        {
            var modelId = innJObject["I"].ToString();
            var modelName = innJObject["N"].ToString();
            var modelInitial = Common.GetSpell(modelName) ?? "-";

            var model = new AHModel
            {
                ID = int.Parse(modelId),
                MasterBrandID = masterBrandId,
                ManufacturerID = manufacturerId,
                ModelName = modelName,
                Initial = modelInitial,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            try
            {
                if (AhModelDal.Exists(model.ID))
                {
                    if (AhModelDal.NeedUpdate(model))
                    {
                        AhModelDal.Update(model);
                        Console.WriteLine("修改车系{0}", model.ModelName);
                    }
                }
                else
                {
                    AhModelDal.Insert(model);
                    Console.WriteLine("添加车系{0}", model.ModelName);
                }
            }
            catch (Exception e)
            {
                _listModels.Remove(_listModels.FirstOrDefault(m => m.ID == model.ID));
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "ERROR:" + e.ToString());
            }
            _listModels.Remove(_listModels.FirstOrDefault(m => m.ID == model.ID));
        }

        /// <summary>
        /// 添加车型
        /// </summary>
        /// <param name="modelId"></param>
        private static void HandleStyles(int modelId)
        {
            string uri = string.Format("http://car.autohome.com.cn/duibi/ashx/SpecCompareHandler.ashx?type=1&seriesid={0}&isie6=0", modelId);
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
            var ahStyle = new AhStyle();
            try
            {
                string styleId = styleJObject["I"].ToString();
                string styleName = styleJObject["N"].ToString();
                string yearName = "";
                if (styleName.Length > 4)
                {
                    yearName = styleName.Substring(0, 4);
                }

                string price = styleJObject["P"].ToString();

                ahStyle.ID = int.Parse(styleId);
                ahStyle.StyleName = styleName;
                ahStyle.SaleStatus = saleStatus;
                ahStyle.Price = decimal.Parse(price);
                ahStyle.ModelID = modelId;
                ahStyle.Year = yearName;
                ahStyle.CreateTime = DateTime.Now;
                ahStyle.UpdateTime = DateTime.Now;
                ahStyle.IsRemoved = 0;
                if (AhStyleDal.Exists(ahStyle.ID))
                {
                    if (AhStyleDal.NeedUpdate(ahStyle))
                    {
                        AhStyleDal.Update(ahStyle);
                        Console.WriteLine("修改车型{0}", ahStyle.StyleName);
                    }
                }
                else
                {
                    AhStyleDal.Insert(ahStyle);
                    Console.WriteLine("添加车型{0}", ahStyle.StyleName);
                }
                _listsStyles.Remove(_listsStyles.FirstOrDefault(m => m.ID == ahStyle.ID));

            }
            catch (Exception e)
            {
                _listsStyles.Remove(_listsStyles.FirstOrDefault(m => m.ID == ahStyle.ID));
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "ERROR:" + e.ToString());
            }
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        private static void DeleteEntities()
        {
            var deleteMasterBrandIds = _listMasterBrands.Select(m => m.ID).ToList();
            var deleteManufacturerIds = _listManufacturers.Select(m => m.ID).ToList();
            var deleteModelIds = _listModels.Select(m => m.ID).ToList();
            var deleteStyleIds = _listsStyles.Select(m => m.ID).ToList();

            if (_listMasterBrands.Count > 0)
            {
                foreach (int masterBrandId in deleteMasterBrandIds)
                {

                }
            }

            if (_listManufacturers.Count > 0)
            {
                foreach (int makeId in deleteManufacturerIds)
                {
                    var ahModels = AhModelDal.GetAhModels(makeId);
                    deleteModelIds.AddRange(ahModels.Select(m => m.ID));
                }
            }

            if (_listModels.Count > 0)
            {
                foreach (int modelId in deleteModelIds)
                {
                    var ahStyles = AhStyleDal.GetAhStyles(modelId);
                    deleteStyleIds.AddRange(ahStyles.Select(m => m.ID));
                }
            }
            int a = 0;
            int b = 0;
            int c = 0;
            int d = 0;
            if (deleteStyleIds.Count > 0)
            {
                d = AhStyleDal.DeleteList(string.Join(",", deleteStyleIds));
            }
            if (deleteModelIds.Count > 0)
            {
                c = AhModelDal.DeleteList(string.Join(",", deleteModelIds));
            }
            if (deleteManufacturerIds.Count > 0)
            {
                b = AhManufacturerDal.DeleteList(string.Join(",", deleteManufacturerIds));
            }
            if (deleteMasterBrandIds.Count > 0)
            {
                a = AhMbdDal.DeleteList(string.Join(",", deleteMasterBrandIds));
            }
            var str = string.Format("删除主品牌{0}个；品牌{1}个；车系{2}个；车型{3}个", a, b, c, d);
            Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + str);
        }
    }
}
