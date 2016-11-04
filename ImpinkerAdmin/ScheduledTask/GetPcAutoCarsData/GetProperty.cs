using System;
using AhBll;
using AhModel;
using HtmlAgilityPack;

namespace GetCarDataService.GetPcAutoCarsData
{
    public class GetProperty
    {
        private const string Url = "http://price.pcauto.com.cn/choose.jsp";

        private const int CompanyId = (int)CompanyEnum.PCauto;

        public static void GetStyleProperty()
        {
            try
            {
                HtmlDocument htmlDocument = Common.GetHtmlDocument(Url);
                var groupNodes = htmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class,'dSubTit')]");
                var paraNodes = htmlDocument.DocumentNode.SelectNodes("//table[starts-with(@class,'tbParam')]");
                paraNodes.Remove(0);
                paraNodes.Remove(0);
                paraNodes.Remove(0);
                paraNodes.Remove(paraNodes.Count - 1);
                paraNodes.Remove(paraNodes.Count - 1);
                if (groupNodes != null && paraNodes != null && (groupNodes.Count == paraNodes.Count))
                {
                    AnalysGroups(groupNodes, paraNodes);
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "获取太平洋汽车参配名称ERROR:" + e.ToString());
            }
        }

        public static void AnalysGroups(HtmlNodeCollection groupNodes, HtmlNodeCollection paraNodes)
        {
            for (int i = 0; i < groupNodes.Count; i++)
            {
                string groupName = groupNodes[i].InnerText.Trim();
                if (string.IsNullOrEmpty(groupName))
                {
                    continue;
                }
                var groupId = i+1;
                var flag = BasicStylePropertyGroupBll.AddStylePropertyGroup(groupId, groupName, CompanyId);
                if (flag)
                {
                    AnalysePara(groupId, paraNodes[i]);
                }
            }
        }

        public static void AnalysePara(int groupId, HtmlNode paraNode)
        {
            ////table[starts-with(@class,'tbParam')]
            HtmlNodeCollection trNodes = paraNode.ChildNodes[1].SelectNodes("child::tr");
            if (trNodes.Count > 0)
            {
                foreach (HtmlNode trNode in trNodes)
                {
                    int propertyId = AnalyzePropertyId(trNode);
                    if (propertyId == 0)
                    {
                        continue;
                    }

                    string stylePropertyName = trNode.SelectNodes("child::th[1]")[0].ChildNodes[1].InnerText.Trim();
                    if (string.IsNullOrEmpty(stylePropertyName))
                    {
                        continue;
                    }
                    var model = new BasicStyleProperty
                    {
                        CompanyId = CompanyId,
                        ID = propertyId,
                        Name = stylePropertyName,
                        PropertyGroupId = groupId,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    BasicStylePropertyBll.AddStyleProperty(model);
                }
            }
        }

        public static int AnalyzePropertyId(HtmlNode trNode)
        {
            //string hrefValue = trNode.SelectNodes("child::th[1]")[0].ChildNodes[0].ChildNodes[0].GetAttributeValue("href", "");// trNode.SelectNodes("child::a")[0].GetAttributeValue("href", "");
            var aNode = trNode.SelectNodes("child::th[1]/div/a");
            if (aNode == null) return 0;
            var hrefValue = aNode[0].GetAttributeValue("href", "");
            if (hrefValue.Length == 0)
            {
                return 0;
            }
            int startIndex = hrefValue.LastIndexOf("/", StringComparison.Ordinal) + 1;
            int endIndex = hrefValue.LastIndexOf(".", StringComparison.Ordinal);
            int propertyId = Int32.Parse(hrefValue.Substring(startIndex, endIndex - startIndex).Trim());
            return propertyId;
        }
    }
}
