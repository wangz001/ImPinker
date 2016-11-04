using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AhModel;
using GetCarDataService.GetCarsDataBll;
using HtmlAgilityPack;

namespace GetCarDataService.GetXCAutoCarsData
{
    class GetStylePropertyValues
    {
        private const int CompanyId = (int)CompanyEnum.XCauto;
        private static GetStylePropertyValueBll _basicStylePropertyValueBll;
        private static List<BasicStyleProperty> _basicStyleProperties;
        private const string Url = "http://newcar.xcar.com.cn/m{0}/config.htm";

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
            DataTable dtNew = GetTableSchema();
            try
            {
                HtmlDocument htmlDocument = Common.HtmlRequest(uri);
                var table = htmlDocument.GetElementbyId("Table1");
                var trLists = table.SelectNodes("child::tr");
                foreach (var trList in trLists)
                {
                    var td = trList.SelectNodes("child::td");
                    if (td != null) //参配
                    {
                        var propertyValue = td[1].InnerText.Trim();
                        var engName = td[0].GetAttributeValue("id", "");
                        if (!string.IsNullOrEmpty(propertyValue) && !string.IsNullOrEmpty(engName))
                        {
                            var property = _basicStyleProperties.FirstOrDefault(m => m.EnglishName == engName);
                            if (property != null)
                            {
                                DataRow r = dtNew.NewRow();
                                r[0] = 100;
                                r[1] = CompanyId;
                                r[2] = property.ID;
                                r[3] = styleId;
                                r[4] = propertyValue;
                                r[5] = DateTime.Now;
                                r[6] = DateTime.Now;
                                dtNew.Rows.Add(r);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
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

		//private static HtmlDocument HtmlRequest(string uri)
		//{
		//	HtmlDocument htmlDocument = null;
		//	Boolean flag = true;
		//	while (flag)//此处用一个循环，如果网络连接超时，则重新执行
		//	{
		//		try
		//		{
		//			htmlDocument = Common.GetHtmlDocument(uri);
		//			flag = false;
		//		}
		//		catch (Exception)
		//		{
		//			//连接网络超时！;
		//			Thread.Sleep(3000);
		//		}
		//	}
		//	return htmlDocument;
		//}
    }
}
