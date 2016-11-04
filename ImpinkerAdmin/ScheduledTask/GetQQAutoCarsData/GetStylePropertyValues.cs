using System;
using System.Collections.Generic;
using System.Data;
using AhModel;
using GetCarDataService.GetCarsDataBll;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetCarDataService.GetQQAutoCarsData
{
    class GetStylePropertyValues
    {
        private const int CompanyId = (int)CompanyEnum.QQauto;
        private static GetStylePropertyValueBll _basicStylePropertyValueBll;
        private static List<BasicStyleProperty> _basicStyleProperties;
        private const string Url = "http://js.data.auto.qq.com/car_models/{0}/new_info.js?v1";

        public static void Start()
        {
            _basicStylePropertyValueBll = new GetStylePropertyValueBll(CompanyId);
            _basicStylePropertyValueBll.Start();
        }

        /// <summary>
        /// _basicStylePropertyValueBll中调用，返回解析好的参配值
        /// </summary>
        /// <param name="styleId"></param>
        /// <param name="basicStyleProperties"></param>
        /// <param name="basicStylePropertyGroups"></param>
        /// <returns></returns>
        public static DataTable GetStylePropertyValue(int styleId,
            List<BasicStyleProperty> basicStyleProperties, List<BasicStylePropertyGroup> basicStylePropertyGroups)
        {
            _basicStyleProperties = basicStyleProperties;
            string uri = string.Format(Url, styleId);
            var dt = GetWebContent(uri, styleId);
            return dt;
        }


        /// <summary>
        /// 解析返回的html页面，获取其中的车型属性值，保存到数据库
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="styleId"></param>
        public static DataTable GetWebContent(string uri, int styleId)
        {
            var dt = new DataTable();
            try
            {
                string strResult = Common.GetResponseContent(uri).Trim();
                if (strResult.Length > 0)
                {
                    var start = strResult.IndexOf("] = {", StringComparison.Ordinal);
                    strResult = strResult.Substring(start + 4);
                    var end = strResult.LastIndexOf("}", StringComparison.Ordinal);
                    strResult = strResult.Substring(0, end+1 );
                    
                    var propertyValue = (JObject)JsonConvert.DeserializeObject(strResult);
                    if (propertyValue.Count > 0)
                    {
                        dt = AnalysePropertyValues(styleId, propertyValue);
                        return dt;
                    }
                }
            }
            catch
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]获取腾讯车型参配值错误:"+uri );
            }
            return null;
        }


        private static DataTable AnalysePropertyValues(int styleId, JObject propertyValues)
        {
            DataTable dtNew = GetTableSchema();
            foreach (var basicStyleProperty in _basicStyleProperties)
            {
                var engName = basicStyleProperty.EnglishName;
                var propertyId = basicStyleProperty.ID;
                var value = propertyValues.GetValue(engName);
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    if (value.ToString() == "--")
                    {
                        value = "-";
                    }
                    if (value.ToString() == "⊙")
                    {
                        value = "○";
                    }
                    var row = dtNew.NewRow();
                    row[0] = 100;  //该参数没有用
                    row[1] = CompanyId;
                    row[2] = propertyId;
                    row[3] = styleId;
                    row[4] = value;
                    row[5] = DateTime.Now;
                    row[6] = DateTime.Now;
                    dtNew.Rows.Add(row);
                }
            }
            return dtNew;
        }

        private static DataTable GetTableSchema()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new[]{  
            new DataColumn("Id",typeof(int)), 
            new DataColumn("CompanyId",typeof(int)),
            new DataColumn("PropertyID",typeof(int)),  
            new DataColumn("StyleID",typeof(int)),  
            new DataColumn("Value",typeof(string)),
            new DataColumn("CreateTime",typeof(DateTime)),
            new DataColumn("UpdateTime",typeof(DateTime)) 
            });
            return dt;
        }
    }
}
