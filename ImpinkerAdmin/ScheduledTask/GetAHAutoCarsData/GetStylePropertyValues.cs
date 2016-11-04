using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using AhModel;
using GetCarDataService.GetCarsDataBll;
using HtmlAgilityPack;

namespace GetCarDataService.GetAHAutoCarsData
{
	public class GetStylePropertyValues
	{
		private const int CompanyId = (int)CompanyEnum.AHauto;
		private static GetStylePropertyValueBll _basicStylePropertyValueBll;
		private static List<BasicStyleProperty> _basicStyleProperties;
		private static List<BasicStylePropertyGroup> _basicStylePropertyGroups;
		private const string Url = "http://car.autohome.com.cn/duibi/chexing/carids={0},0,0,0";

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
			var dt = GetWebContent(uri, styleId);
			return dt;
		}

		/// <summary>
		/// 解析返回的html页面，获取其中的车型属性值，保存到数据库
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="styleId"></param>
		private static DataTable GetWebContent(string uri, int styleId)
		{
			DataTable dt = GetTableSchema();
			HtmlDocument htmlDocument = Common.HtmlRequest(uri);
			if (htmlDocument != null)
			{
				try
				{
					var groupNodes = htmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class,'js-title')]");
					var paraNodes = htmlDocument.DocumentNode.SelectNodes("//table[starts-with(@class,'js-titems')]");
					var xuanzhuangNode = htmlDocument.DocumentNode.SelectSingleNode("//div[starts-with(@data-title,'选装包')]");
					if (xuanzhuangNode != null)
					{
						groupNodes.Remove(xuanzhuangNode);
					}
					if (groupNodes != null && paraNodes != null && (groupNodes.Count == paraNodes.Count))
					{
						dt = AnalysePropertyValues(styleId, groupNodes, paraNodes);
					}
				}
				catch (Exception e)
				{
					Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]ERROR:" + e.ToString());
				}
			}
			return dt;
		}

		//private static HtmlDocument HtmlRequest(string uri)
		//{
		//	HtmlDocument htmlDocument = null;
		//	Exception err = null;
		//	int count = 0;
		//	while (count < 3)//此处用一个循环，如果网络连接超时，则重新执行
		//	{
		//		count++;
		//		try
		//		{
		//			htmlDocument = Common.GetHtmlDocument(uri);
		//			err = null;
		//			break;
		//		}
		//		catch (Exception ex)
		//		{
		//			err = ex;
		//			Thread.Sleep(1000);
		//		}
		//	}
		//	if (err != null)
		//		Common.WriteErrorLog(err.ToString());
		//	return htmlDocument;
		//}

		private static DataTable AnalysePropertyValues(int styleId, HtmlNodeCollection groupNodes, HtmlNodeCollection paraNodes)
		{
			DataTable dtNew = GetTableSchema();
			for (int i = 0; i < groupNodes.Count; i++)
			{
				string groupName = groupNodes[i].ChildNodes[1].InnerText.Trim();
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
				//else
				//{
				//    GetStyleProperty.AnalysGroups(groupNodes[i], paraNodes[i]);
				//}
			}
			return dtNew;
		}

		/// <summary>
		/// 解析出group对应的paraNodes属性 得到属性和属性值入库
		/// </summary>
		private static void FillDataTable(HtmlNode paraNode, int styleId, int groupId, DataTable dt)
		{
			if (paraNode.HasChildNodes)
			{
				HtmlNodeCollection tbodyNodes = paraNode.ChildNodes;
				if (tbodyNodes.Count > 0)
				{
					foreach (HtmlNode trNode in tbodyNodes)
					{
						int propertyId = GetStyleProperty.AnalyzePropertyId(trNode);
						if (propertyId == 0)
						{
							continue;
						}
						var property = _basicStyleProperties.Find(m => m.ID == propertyId && m.PropertyGroupId == groupId);
						if (property == null)
						{
							continue;
						}
						string stylePropertyValue = trNode.ChildNodes[1].InnerText.Trim();
						DataRow r = dt.NewRow();
						r[0] = tbodyNodes.Count;
						r[1] = CompanyId;
						r[2] = propertyId;
						r[3] = styleId;
						r[4] = stylePropertyValue;
						r[5] = DateTime.Now;
						r[6] = DateTime.Now;
						if (!dt.Select("PropertyID='" + propertyId + "'").Any())
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
	}

	public class WaitHandleObject
	{
		public int StartIndex { get; set; }

		public int EndIndex { get; set; }

		public WaitHandle WaitHandle { get; set; }
	}
}
