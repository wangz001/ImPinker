using System;
using AhBll;
using AhModel;
using HtmlAgilityPack;

namespace GetCarDataService.GetXCAutoCarsData
{
    public class GetProperty
    {
        private const string Url = "http://newcar.xcar.com.cn/m391/config.htm";
        private const int CompanyId = (int)CompanyEnum.XCauto;

        public static void GetStyleProperty()
        {
            try
            {
                HtmlDocument htmlDocument = Common.GetHtmlDocument(Url);
                var table = htmlDocument.GetElementbyId("Table1");
                if (table == null)
                {
                    Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "格式错误，请重新制定解析规则" + Url);
                    return;
                }
                var trLists = table.SelectNodes("child::tr");
                var groupid = 0;
                foreach (var trList in trLists)
                {
                    var th = trList.SelectNodes("child::th");
                    if (th != null)  //参配分组
                    {
                        groupid++;
                        var groupName = th[0].InnerText;
                        BasicStylePropertyGroupBll.AddStylePropertyGroup(groupid, groupName, CompanyId);
                        continue;
                    }
                    var td = trList.SelectNodes("child::td");
                    if (td != null) //参配
                    {
                        var propertyName = td[0].InnerText;
                        var engName = td[0].GetAttributeValue("id", "");
                        if (!string.IsNullOrEmpty(propertyName) && !string.IsNullOrEmpty(engName))
                        {
                            AnalysProperty(groupid, engName, propertyName);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "获取爱卡汽车参配名称ERROR:" + e.ToString());
            }
        }

        public static void AnalysProperty(int groupId, string engName, string propertyName)
        {
            var propertyId = -100;
            var minId = BasicStylePropertyBll.GetMinId();
            if (minId <= -100)
            {
                propertyId = minId - 1;
            }
            var model = new BasicStyleProperty
            {
                ID = propertyId,
                Name = propertyName,
                EnglishName = engName,
                PropertyGroupId = groupId,
                CompanyId = CompanyId,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            BasicStylePropertyBll.AddStylePropertyWithEnglishName(model);
        }
    }
}
