using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using AhDal;
using AhModel;
using GetCarDataService.AutoHomeCountTask;
using HtmlAgilityPack;

namespace GetCarDataService.GetAutoHomeCarsData
{
    public class GetStylePropertyValues
    {
        private static readonly AhStyleDal AhStyleDal = new AhStyleDal();
        private static readonly AHStylePropertyGroupDal AhStylePropertyGroupDal = new AHStylePropertyGroupDal();
        private static readonly AhStylePropertyDal AhStylePropertyDal = new AhStylePropertyDal();
        private static readonly AhStylePropertyValueDal AhStylePropertyValueDal = new AhStylePropertyValueDal();
        private static readonly List<AhStyle> AhStyles;
        private static readonly List<AhStyleProperty> AhStyleProperties;
        private static readonly List<AhStylePropertyGroup> AhStylePropertyGroups;

        private static Queue<DataTable> _dataTableQueue; //处理datatable的队列
        private static int _counterGetHtml; //计数器，标记已经从网页获取到的车型个数
        private static int _counterIntoDb; //计数器，标记已经从队列里更新到数据库的车型个数
        private static int _countGetDataThreadEnd;//记录已经结束的获取数据的线程个数，总共是5个

        static GetStylePropertyValues()
        {
            _dataTableQueue = new Queue<DataTable>();
            AhStyles = DataSetTransferHelper.DataTableToList(AhStyleDal.GetList("").Tables[0]);
            //AhStyles = AhStyleBll.GetModelListNotHavePropertyValue("");   //获取还未获取参配的车型
            AhStyleProperties = DataSetTransferHelper.DataTableToListStyleProperty(AhStylePropertyDal.GetList("").Tables[0]);
            AhStylePropertyGroups = DataSetTransferHelper.DataTableToListsGroups(AhStylePropertyGroupDal.GetList("").Tables[0]);
        }


        static WaitHandle[] waitHandles = new WaitHandle[] 
        {
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false)
         };


        public static void Start()
        {
            int count = AhStyles.Count;
            int dataCount = count / 4;
            int remainder = count % 4;
            for (int i = 1; i <= 5; i++) //定为5个线程异步请求数据并处理
            {
                int startIndex;
                int endIndex;

                if (i == 5)
                {
                    startIndex = (i - 1) * dataCount;
                    endIndex = (i - 1) * dataCount + remainder - 1;
                }
                else
                {
                    startIndex = (i - 1) * dataCount;
                    endIndex = i * dataCount - 1;
                }

                var obj = new WaitHandleObject
                {
                    StartIndex = startIndex,
                    EndIndex = endIndex,
                    WaitHandle = waitHandles[i - 1]
                };
                ThreadPool.QueueUserWorkItem(new WaitCallback(GetDataM), obj);
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveData), waitHandles[5]);

            WaitHandle.WaitAll(waitHandles);

            CheckStyleUpdateByPropertyValue.Check();


        }

        /// <summary>
        /// 将数据保存到数据库
        /// </summary>
        private static void SaveData(Object state)
        {
            var are = (AutoResetEvent)state;

            var flag = true;
            while (flag)
            {
                if (_dataTableQueue.Count > 0)
                {
                    var dt = _dataTableQueue.Dequeue();
                    try
                    {
                        AhStylePropertyValueDal.InitWithTvp(dt);  //用表值参数方式批量导入数据
                    }
                    catch (Exception)
                    {
                        Common.WriteErrorLog("车型入库错误!");
                    }
                    _counterIntoDb++;
                    Console.WriteLine("有{0}个车型已入库；队列中待处理数据数量：{1}", _counterIntoDb, _dataTableQueue.Count);
                }
                else
                {
                    if (_counterIntoDb == AhStyles.Count || _countGetDataThreadEnd == 5)
                    {
                        var text = "所有车型更新完毕！" + "共更新" + _counterIntoDb + "个车型的参配信息";
                        Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "结束->⑧获取车型参配值信息。" + text);
                        flag = false;
                    }
                    else
                    {
                        Console.WriteLine("队列里没有数据");
                        Thread.Sleep(3000); //如果没有数据，休眠3秒
                    }
                }
            }
            are.Set();
        }


        /// <summary>
        /// 请求汽车之家参配值数据的方法
        /// </summary>
        /// <param name="args">WaitHandleObject</param>
        static void GetDataM(object args)
        {
            var obj = args as WaitHandleObject;
            if (obj == null)
                return;
            var are = (AutoResetEvent)obj.WaitHandle;

            for (int i = obj.StartIndex; i <= obj.EndIndex; i++)
            {
                int styleId = AhStyles[i].ID;
                string uri = string.Format("http://car.autohome.com.cn/duibi/chexing/carids={0},0,0,0", styleId);
                GetWebContent(uri, styleId);
            }
            Console.WriteLine("线程{0}执行完毕", obj.WaitHandle);
            _countGetDataThreadEnd++;
            are.Set();
        }

        /// <summary>
        /// 解析返回的html页面，获取其中的车型属性值，保存到数据库
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="styleId"></param>
        private static void GetWebContent(string uri, int styleId)
        {

            HtmlDocument htmlDocument = Common.HtmlRequest(uri);
            
            if (htmlDocument != null)
            {
                try
                {
                    var groupNodes = htmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class,'js-title')]");
                    var paraNodes = htmlDocument.DocumentNode.SelectNodes("//table[starts-with(@class,'js-titems')]");
                    if (groupNodes != null && paraNodes != null && (groupNodes.Count == paraNodes.Count))
                    {
                        AnalysePropertyValues(styleId, groupNodes, paraNodes);
                    }
                }
                catch (Exception e)
                {
                    Common.WriteErrorLog(e.ToString());
                }
            }
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

        private static void AnalysePropertyValues(int styleId, HtmlNodeCollection groupNodes, HtmlNodeCollection paraNodes)
        {
            DataTable dtNew = GetTableSchema();
            for (int i = 0; i < groupNodes.Count; i++)
            {
                string groupName = groupNodes[i].ChildNodes[1].InnerText.Trim();
                if (string.IsNullOrEmpty(groupName))
                {
                    continue;
                }
                var ahStylePropertyGroup = AhStylePropertyGroups.Find(m => m.Name == groupName);
                if (ahStylePropertyGroup != null)
                {
                    int groupId = ahStylePropertyGroup.ID;
                    FillDataTable(paraNodes[i], styleId, groupId, dtNew);
                }
            }
            _dataTableQueue.Enqueue(dtNew);  //加入队列
            _counterGetHtml++;
            Console.WriteLine("html：已获取{0}个车型参配信息。车型ID：{1}", _counterGetHtml, styleId);

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
                        var property = AhStyleProperties.Find(m => m.ID==propertyId && m.PropertyGroupID == groupId);
                        if (property == null)
                        {
                            continue;
                        }
                        string stylePropertyValue = trNode.ChildNodes[1].InnerText.Trim();
                        DataRow r = dt.NewRow();
                        r[0] = tbodyNodes.Count;
                        r[1] = propertyId;
                        r[2] = styleId;
                        r[3] = stylePropertyValue;
                        r[4] = DateTime.Now;
                        r[5] = DateTime.Now;
                        dt.Rows.Add(r);
                    }
                }
            }
        }

        private static DataTable GetTableSchema()
        {
            var dt = new DataTable();
            dt.Columns.AddRange(new[]{  
            new DataColumn("Id",typeof(int)),  
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
