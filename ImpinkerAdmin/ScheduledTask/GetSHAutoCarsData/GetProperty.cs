using System;
using AhBll;
using AhModel;
using HtmlAgilityPack;

namespace GetCarDataService.GetSHAutoCarsData
{
    public class GetProperty
    {
        private const string Url = "http://db.auto.sohu.com/pk-trim.shtml#0,0,0,0,0";
        private const int CompanyId = (int)CompanyEnum.SHauto;
        
        public static void GetStyleProperty()
        {
            try
            {
                HtmlDocument htmlDocument = Common.GetHtmlDocument(Url,true);
                var groupNodes = htmlDocument.DocumentNode.SelectNodes("//table[starts-with(@id,'trimArglist')]");
                if (groupNodes == null)
                {
                    Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "] 搜狐车型参配，格式错误，请重新制定解析规则" );
                    return;
                }
                var trLists = groupNodes[0].SelectNodes("child::tr");
                var groupid = 0;
                foreach (var trList in trLists)
                {
                    var td = trList.SelectNodes("child::td[1]")[0];
                    var className = td.GetAttributeValue("class", "");
                    if (className.Equals("Argtitle")) //参配分组
                    {
                        groupid++;
                        var flag = AnalysGroup(groupid, td);
                        if (!flag)
                        {
                            Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "] 搜狐车型参配，格式错误，请重新制定解析规则");
                            break;
                        }
                    }
                    if (className.Equals("Argname")) //参配
                    {
                        var engName = trList.GetAttributeValue("id", "");
                        var propertyName = td.InnerText;
                        if (!string.IsNullOrEmpty(engName) && !string.IsNullOrEmpty(propertyName))
                        {
                            AnalysProperty(groupid, engName, propertyName);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "获取搜狐汽车参配名称ERROR:" + e.ToString());
            }
        }

        public static bool AnalysGroup(int groupId, HtmlNode htmlNode)
        {
            var groupName = htmlNode.LastChild.InnerText;
            return BasicStylePropertyGroupBll.AddStylePropertyGroup(groupId, groupName, CompanyId);
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
