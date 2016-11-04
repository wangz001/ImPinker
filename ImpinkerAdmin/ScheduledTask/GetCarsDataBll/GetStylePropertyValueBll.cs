using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using AhBll;
using AhModel;
using GetCarDataService.GetPcAutoCarsData;

namespace GetCarDataService.GetCarsDataBll
{
    public class GetStylePropertyValueBll
    {
        private static List<BasicStyle> _styles;
        private static List<BasicStyleProperty> _basicStyleProperties;
        private static List<BasicStylePropertyGroup> _basicStylePropertyGroups;

        private static Queue<DataTable> _dataTableQueue; //处理datatable的队列
        private int _counterGetHtml; //计数器，标记已经从网页获取到的车型个数
        private int _counterIntoDb; //计数器，标记已经从队列里更新到数据库的车型个数
        private int _countGetDataThreadEnd; //记录已经结束的获取数据的线程个数，总共是5个

        private static int _companyId;

        public GetStylePropertyValueBll(int companyId)
        {
            _companyId = companyId;
            _dataTableQueue = new Queue<DataTable>();
            _styles = BasicStyleBll.GetList(_companyId);
            _basicStyleProperties = BasicStylePropertyBll.GetList(_companyId);
            _basicStylePropertyGroups = BasicStylePropertyGroupBll.GetList(_companyId);
        }
        # region 共用
        static readonly WaitHandle[] WaitHandles =
        {
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false),
            new AutoResetEvent(false)
        };


        public void Start()
        {
            int count = _styles.Count;
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
                    WaitHandle = WaitHandles[i - 1]
                };
                ThreadPool.QueueUserWorkItem(GetDataM, obj);
            }
            ThreadPool.QueueUserWorkItem(SaveData, WaitHandles[5]);

            WaitHandle.WaitAll(WaitHandles);
            //检查参配值，判断车型是否更新,发送邮件等
        }

        /// <summary>
        /// 将数据保存到数据库
        /// </summary>
        private void SaveData(Object state)
        {
            var are = (AutoResetEvent)state;

            var flag = true;
            while (flag)
            {
                if (_dataTableQueue.Count > 0)
                {
                    var dt = _dataTableQueue.Dequeue();
                    if (dt == null)
                    {
                        continue;
                    }
                    try
                    {
                        BasicStylePropertyValueBll.InitWithTvp(dt);  //用表值参数方式批量导入数据
                    }
                    catch (Exception e)
                    {
                        Common.WriteErrorLog("车型入库错误!" + e.ToString());
                    }
                    _counterIntoDb++;
                    Console.WriteLine("已获取{2}个车型数据；有{0}个车型已入库；队列中待处理数据数量：{1}", _counterIntoDb, _dataTableQueue.Count, _counterGetHtml);
                }
                else
                {
                    if (_counterIntoDb == _styles.Count || _countGetDataThreadEnd == 5)
                    {
                        var text = "所有车型更新完毕！" + "共更新" + _counterIntoDb + "个车型的参配信息";
                        Common.WriteInfoLog("[" + Common.GetDateTimeStr() + "]" + "结束->⑧获取车型参配值信息。" + text);
                        flag = false;
                    }
                    else
                    {
                        //Console.WriteLine("队列里没有数据");
                        Thread.Sleep(3000);
                    }
                }
            }
            are.Set();
        }

        #endregion

        /// <summary>
        /// 请求汽车参配值数据的方法
        /// </summary>
        /// <param name="args">WaitHandleObject</param>
        private void GetDataM(object args)
        {
            var obj = args as WaitHandleObject;
            if (obj == null)
                return;
            var are = (AutoResetEvent)obj.WaitHandle;
            try
            {
                for (int i = obj.StartIndex; i <= obj.EndIndex; i++)
                {
                    int styleId = _styles[i].ID;
                    switch (_companyId)
                    {
                        case (int)CompanyEnum.PCauto: GetPcAutoStylePropertyValues(styleId);
                            break;
                        case (int)CompanyEnum.QQauto: GetQQAutoStylePropertyValues(styleId);
                            break;
                        case (int)CompanyEnum.SHauto: GetSHAutoStylePropertyValues(styleId);
                            break;
                        case (int)CompanyEnum.XCauto: GetXCAutoStylePropertyValues(styleId);
                            break;
                        case (int)CompanyEnum.AHauto: GetAHAutoStylePropertyValues(styleId);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + e.ToString());
            }
            _countGetDataThreadEnd++;
            are.Set();
        }

        private void GetPcAutoStylePropertyValues(int styleId)
        {
            var dt = GetStylePropertyValues.GetStylePropertyValue(styleId
                , _basicStyleProperties, _basicStylePropertyGroups);
            if (dt == null||dt.Rows.Count==0)
            {
                return;
            }
            _dataTableQueue.Enqueue(dt);  //加入队列
            _counterGetHtml++;
        }

        private void GetSHAutoStylePropertyValues(int styleId)
        {
            var dt = GetCarDataService.GetSHAutoCarsData.GetStylePropertyValues.GetStylePropertyValue(styleId
                , _basicStyleProperties, _basicStylePropertyGroups);
            if (dt == null)
            {
                return;
            }
            _dataTableQueue.Enqueue(dt);  //加入队列
            _counterGetHtml++;
        }

        private void GetQQAutoStylePropertyValues(int styleId)
        {
            var dt = GetCarDataService.GetQQAutoCarsData.GetStylePropertyValues.GetStylePropertyValue(styleId
                , _basicStyleProperties, _basicStylePropertyGroups);
            if (dt == null)
            {
                return;
            }
            _dataTableQueue.Enqueue(dt);  //加入队列
            _counterGetHtml++;
        }

        private void GetXCAutoStylePropertyValues(int styleId)
        {
            var dt = GetCarDataService.GetXCAutoCarsData.GetStylePropertyValues.GetStylePropertyValue(styleId
                , _basicStyleProperties, _basicStylePropertyGroups);
            if (dt == null)
            {
                return;
            }
            _dataTableQueue.Enqueue(dt);  //加入队列
            _counterGetHtml++;
        }

        private void GetAHAutoStylePropertyValues(int styleId)
        {
            var dt = GetAHAutoCarsData.GetStylePropertyValues.GetStylePropertyValue(styleId
                , _basicStyleProperties, _basicStylePropertyGroups);
            if (dt == null)
            {
                return;
            }
            _dataTableQueue.Enqueue(dt);  //加入队列
            _counterGetHtml++;
        }
    }

    public class WaitHandleObject
    {
        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public WaitHandle WaitHandle { get; set; }
    }
}
