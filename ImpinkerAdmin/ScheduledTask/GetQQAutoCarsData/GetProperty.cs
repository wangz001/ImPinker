using System;
using AhBll;
using AhModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetCarDataService.GetQQAutoCarsData
{
    public class GetProperty
    {
        private const string Url = "http://mat1.gtimg.com/auto/js/datalib/compare/compare11_v09.js";

        private const int CompanyId = (int)CompanyEnum.QQauto;

        public static void GetStyleProperty()
        {
            try
            {
                string strResult = Common.GetResponseContent(Url).Trim();
                var index1 = strResult.IndexOf("g_arrConfigType=", StringComparison.Ordinal) + 16;
                var index2 = strResult.IndexOf(";var g_arrConfigField=", StringComparison.Ordinal);
                var index3 = strResult.IndexOf("}]];", StringComparison.Ordinal);
                var groupStr = strResult.Substring(index1, index2 - index1);
                var parasStr = strResult.Substring(index2 + 22, index3 - (index2 + 22) + 3);
                var groupNames = (JArray)JsonConvert.DeserializeObject(groupStr);
                var paras = (JArray)JsonConvert.DeserializeObject(parasStr);
                if (groupNames.Count == paras.Count)
                {
                    AnalysGroups(groupNames, paras);
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "获取腾讯汽车参配名称ERROR:" + e.ToString());
            }
        }

        private static void AnalysGroups(JArray groupNames, JArray paras)
        {
            for (var i = 1; i <= groupNames.Count; i++)
            {
                var groupId = i;
                string groupName = groupNames[i - 1].ToString();
                if (string.IsNullOrEmpty(groupName))
                {
                    continue;
                }

                var falg = BasicStylePropertyGroupBll.AddStylePropertyGroup(groupId, groupName, CompanyId);

                if (falg)
                {
                    foreach (JObject para in paras[i - 1])
                    {
                        var engName = para["sFieldName"].ToString();
                        var name = para["sFielTitle"].ToString();
                        var sref = para["sRef"];
                        if (!string.IsNullOrEmpty(engName) && !string.IsNullOrEmpty(name))
                        {
                            AnalysePara(groupId, engName, name, sref.ToString());
                        }
                    }
                }
            }
        }

        public static void AnalysePara(int groupId, string engName, string name, string sref)
        {
            var propertyId = -100;
            if (string.IsNullOrEmpty(sref))  //没有ID的参配，生成id，用负数
            {
                var minId = BasicStylePropertyBll.GetMinId();
                if (minId <= -100)
                {
                    propertyId = minId - 1;
                }
            }
            else
            {
                var startIndex = sref.LastIndexOf("/", StringComparison.Ordinal) + 1;
                var endIndex = sref.IndexOf(".htm", StringComparison.Ordinal);
                var idStr = sref.Substring(startIndex, endIndex - startIndex);
                propertyId = Int32.Parse(idStr);
            }

            var model = new BasicStyleProperty
            {
                ID = propertyId,
                Name = name,
                EnglishName = engName,
                PropertyGroupId = groupId,
                CompanyId = CompanyId,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            if (model.ID>0)
            {//sRef:"http://auto.qq.com/a/20071012/000237.htm" 类型的参配，能分析出id
                BasicStylePropertyBll.AddStyleProperty(model);
            }
            else
            {
                BasicStylePropertyBll.AddStylePropertyWithEnglishName(model);
            }
        }
    }
}
