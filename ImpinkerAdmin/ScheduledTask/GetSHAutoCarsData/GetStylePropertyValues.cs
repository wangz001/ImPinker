using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using AhModel;
using GetCarDataService.GetCarsDataBll;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetCarDataService.GetSHAutoCarsData
{
    class GetStylePropertyValues
    {
        private const int CompanyId = (int)CompanyEnum.SHauto;
        private static GetStylePropertyValueBll _basicStylePropertyValueBll;
        private static List<BasicStyleProperty> _basicStyleProperties;
        private const string Url = "http://db.auto.sohu.com/PARA/TRIMDATA/trim_data_{0}.json";

        public static void Start()
        {
            try
            {
                _basicStylePropertyValueBll = new GetStylePropertyValueBll(CompanyId);
                _basicStylePropertyValueBll.Start();
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }
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
            try
            {
                var dt = new DataTable();
                string strResult = Common.GetResponseContent(uri).Trim();
                if (strResult.Length > 0)
                {
                    var propertyValue = (JObject)JsonConvert.DeserializeObject(strResult);
                    if (propertyValue.Count > 0)
                    {
                        dt = AnalysePropertyValues(styleId, propertyValue);
                    }
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    return dt;
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }
            return null;
        }

        private static DataTable AnalysePropertyValues(int styleId, JObject propertyValues)
        {
            DataTable dtNew = GetTableSchema();
            foreach (var property in _basicStyleProperties)
            {
                var engName = property.EnglishName;
                var propertyId = property.ID;
                var value = propertyValues.GetValue(engName); //utf-8格式
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    var str = HttpUtility.UrlDecode(value.ToString(), Encoding.UTF8);
                    if (str.Length > 500)
                    {
                        str = str.Substring(0, 500);
                    }
                    DataRow r = dtNew.NewRow();
                    r[0] = 100;  //该参数没有用
                    r[1] = CompanyId;
                    r[2] = propertyId;
                    r[3] = styleId;
                    r[4] = str;
                    r[5] = DateTime.Now;
                    r[6] = DateTime.Now;
                    dtNew.Rows.Add(r);
                    //Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]SH:Value:" + styleId+"   "+propertyId+"   "+value+str);
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
