using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using Bita.Common;

namespace GetCarDataService.CarsDataCountTask
{
    class ExportHelper
    {
        private static List<ModelSeoUv> _modelSeoUvs;

        public static List<ModelSeoUv> GetModelSeoUvs()
        {
            if (_modelSeoUvs == null) _modelSeoUvs = GetResponseContent();
            return _modelSeoUvs;
        }

        private static List<ModelSeoUv> GetResponseContent()
        {
            try
            {
                string uri = ConfigHelper.GetConfigString("seoUv");
                var request = (HttpWebRequest)WebRequest.Create(uri);
                var response = (HttpWebResponse)request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.Load(stream);
                        return GetSeoUv(xmlDocument);
                    }
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog(e.Message);
            }
            return null;
        }

        private static List<ModelSeoUv> GetSeoUv(XmlDocument xmlDocument)
        {
            var xmlNodeList = xmlDocument.SelectNodes("SerialSort/Serial");
            if (xmlNodeList.Count > 0)
            {
                var modelSeoUvs = new List<ModelSeoUv>();
                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    var modelSeoUv = new ModelSeoUv();
                    modelSeoUv.ModelId = int.Parse(xmlNode.Attributes["ID"].Value);
                    modelSeoUv.Name = xmlNode.Attributes["Name"].Value;
                    modelSeoUv.SeoName = xmlNode.Attributes["SEOName"].Value;
                    modelSeoUv.Uv = int.Parse(xmlNode.Attributes["UV"].Value);
                    modelSeoUvs.Add(modelSeoUv);
                }
                return modelSeoUvs;
            }
            return null;
        }

        public static List<ModelSeoUv> GetLocalXml()
        {
            string xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ResourceFile\\" + "SerialUvInfo.xml");
            var doc = new XmlDocument();
            doc.Load(xmlPath);
            return GetSeoUv(doc);
        }

        private HttpResponse httpRes;

        public ExportHelper()
        {
            httpRes = HttpContext.Current.Response;
        }

        /// <summary>
        /// 开始导出
        /// </summary>
        /// <param name="fileName"></param>
        public void StartExport(string fileName)
        {
            if (HttpContext.Current.Request.Browser.Browser != "IE") httpRes.AppendHeader("Content-Disposition", "attachment;filename=" + fileName);
            else httpRes.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
            httpRes.Charset = "UTF-8";
            httpRes.ContentType = "text/csv";
            httpRes.ContentEncoding = Encoding.GetEncoding("GB2312");
        }

        /// <summary>
        /// 导出标题行
        /// </summary>
        /// <param name="colNames"></param>
        public void ExportHeader(string[] colNames)
        {
            ExportLine(colNames);
        }

        /// <summary>
        /// 导出一行数据
        /// </summary>
        /// <param name="colContents"></param>
        public void ExportLine(string[] colContents)
        {
            string lineStr = "";
            foreach (string colContent in colContents)
            {
                lineStr += colContent + ",";
            }
            lineStr += "\n";
            httpRes.Write(lineStr);
        }

        public void End()
        {
            httpRes.End();
        }

        /// <summary>
        /// 导出Excel表格
        /// </summary>
        /// <param name="excelname"></param>
        /// <param name="ds"></param>
        /// <param name="colNames"></param>
        public void ExportExcel(string excelname, DataSet ds, string[] colNames)
        {
            var excelExporter = new ExportHelper();
            excelExporter.StartExport(excelname);
            excelExporter.ExportHeader(colNames);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                var rowItems = new List<string>();
                DataRow row = ds.Tables[0].Rows[i];
                for (int j = 0; j < colNames.Length; j++)
                {
                    var colName = colNames[j];
                    string rowValue = row[colName].ToString();
                    if (!string.IsNullOrEmpty(rowValue))
                    {
                        rowValue = rowValue.Replace("\r\n", "");
                    }
                    rowItems.Add("\"" + rowValue + "\"");
                }
                if (row != null)
                    excelExporter.ExportLine(rowItems.ToArray());
            }
            excelExporter.End();
        }
    }

    public class ModelSeoUv
    {
        public int ModelId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string SeoName
        {
            get;
            set;
        }

        public int Uv
        {
            get;
            set;
        }
    }
}
