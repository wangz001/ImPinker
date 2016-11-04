using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AhModel;
using GetCarDataService.GetCarsDataBll;
using HtmlAgilityPack;

namespace GetCarDataService.GetPcAutoCarsData
{
    public class GetStylePropertyValues
    {
        private  const int CompanyId = (int)CompanyEnum.PCauto;
        private static GetStylePropertyValueBll _basicStylePropertyValueBll ;
        private static  List<BasicStyleProperty> _basicStyleProperties;
        private static  List<BasicStylePropertyGroup> _basicStylePropertyGroups;

        private const string Url = "http://price.pcauto.com.cn/choose.jsp?mid={0}";
       
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
            _basicStylePropertyGroups = basicStylePropertyGroups;
            string uri = string.Format(Url, styleId);
            var dt=GetWebContent(uri, styleId);
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
                HtmlDocument htmlDocument = Common.HtmlRequest(uri);
                var groupNodes = htmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class,'dSubTit')]");
                var paraNodes = htmlDocument.DocumentNode.SelectNodes("//table[starts-with(@class,'tbParam')]");
                paraNodes.Remove(0);
                paraNodes.Remove(0);
                paraNodes.Remove(0);
                paraNodes.Remove(paraNodes.Count - 1);
                paraNodes.Remove(paraNodes.Count - 1);
                if (groupNodes != null && paraNodes != null && (groupNodes.Count == paraNodes.Count))
                {
                  dt=  AnalysePropertyValues(styleId, groupNodes, paraNodes);
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
            }
            return dt;
        }
      
        private static DataTable AnalysePropertyValues(int styleId, HtmlNodeCollection groupNodes, HtmlNodeCollection paraNodes)
        {
            DataTable dtNew = GetTableSchema();
            for (int i = 0; i < groupNodes.Count; i++)
            {
                string groupName = groupNodes[i].InnerText.Trim();
                if (string.IsNullOrEmpty(groupName))
                {
                    continue;
                }
                var ahStylePropertyGroup = _basicStylePropertyGroups.Find(m => m.Name == groupName);
                if (ahStylePropertyGroup != null)
                {
                    int groupId = ahStylePropertyGroup.ID;
                    FillDataTable(paraNodes[i], styleId, groupId, dtNew);
                }
            }
            return dtNew;
        }

        /// <summary>
        /// 解析出group对应的paraNodes属性 得到属性和属性值
        /// </summary>
        private static void FillDataTable(HtmlNode paraNode, int styleId, int groupId, DataTable dt)
        {
            if (paraNode.HasChildNodes)
            {
                HtmlNodeCollection tbodyNodes = paraNode.ChildNodes[1].SelectNodes("child::tr");
                if (tbodyNodes.Count > 0)
                {
                    foreach (HtmlNode trNode in tbodyNodes)
                    {
                        int propertyId = GetProperty.AnalyzePropertyId(trNode);
                        if (propertyId == 0)
                        {
                            continue;
                        }
                        var property = _basicStyleProperties.Find(m => m.ID == propertyId && m.PropertyGroupId == groupId);
                        if (property == null)
                        {
                            continue;
                        }
                        string stylePropertyValue = trNode.SelectNodes("child::td[1]")[0].InnerText.Trim();
                        DataRow r = dt.NewRow();
                        r[0] = tbodyNodes.Count;
                        r[1] = CompanyId;
                        r[2] = propertyId;
                        r[3] = styleId;
                        r[4] = stylePropertyValue;
                        r[5] = DateTime.Now;
                        r[6] = DateTime.Now;
                        if (!dt.Select("PropertyID='"+propertyId+"'").Any())
                        {
                            dt.Rows.Add(r);
                        }
                    }
                }
            }
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
		//			Console.WriteLine("连接网络超时！");
		//			Thread.Sleep(3000);  //休眠3秒
		//		}
		//	}
		//	return htmlDocument;
		//}
    }
}
