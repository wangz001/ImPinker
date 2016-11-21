using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AhBll;
using AhModel;
using Quartz;

namespace GetCarDataService.SolrDicGenerate
{
    /// <summary>
    /// 生成车型相关词典。solr索引和查询使用
    /// </summary>
    public class GenerateCarDataDic : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Generate();
        }

        private void Generate()
        {
            var dicList = new List<string>(); //词典
            var list = new BasicSerialBll().GetAllSerials((int)CompanyEnum.AHauto);
            if (list != null && list.Count > 0)
            {
                foreach (var basicSerialVm in list)
                {
                    var serialName = basicSerialVm.Name;
                    var makeName = basicSerialVm.MakeName;
                    var masterName = basicSerialVm.MasterBrandName;
                    if (serialName.Contains("停售"))
                    {
                        continue; 
                    }
                    //单一方式
                    AddToDicList(ref dicList, serialName);
                    AddToDicList(ref dicList, makeName);
                    AddToDicList(ref dicList, masterName);
                    //组合方式1
                    AddToDicList(ref dicList, makeName + serialName);
                    AddToDicList(ref dicList, masterName + serialName);
                }

                var listStylePropertyGroup = BasicStylePropertyGroupBll.GetList((int) CompanyEnum.AHauto);
                foreach (var basicStylePropertyGroup in listStylePropertyGroup)
                {
                    AddToDicList(ref dicList, basicStylePropertyGroup.Name);
                }
                var listStyleProperty = BasicStylePropertyBll.GetList((int) CompanyEnum.AHauto);
                foreach (var basicStyleProperty in listStyleProperty)
                {
                    AddToDicList(ref dicList, basicStyleProperty.Name);
                }
                SaveDic(dicList);
            }
        }

        private void SaveDic(List<string> dicList)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "dic";
            string fileName = string.Format("cardatadic_{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
            //检查日期目录是否存在
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            fileName = Path.Combine(path, fileName);
            var m_fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
            foreach (var nameStr in dicList)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(nameStr+"\r\n");
                m_fileStream.Write(bytes, 0, bytes.Length);
            }
            m_fileStream.Flush();
            m_fileStream.Close();
        }

        /// <summary>
        /// 向词典里添加内容
        /// </summary>
        /// <param name="dicList"></param>
        /// <param name="nameStr"></param>
        /// <returns></returns>
        private bool AddToDicList(ref List<string> dicList, string nameStr)
        {
            if (dicList!=null&&!string.IsNullOrEmpty(nameStr)&&!dicList.Contains(nameStr))
            {
                dicList.Add(nameStr);
                return true;
            }
            return false;
        }
    }
}
