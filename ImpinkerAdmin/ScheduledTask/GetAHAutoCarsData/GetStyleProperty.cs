using System;
using AhBll;
using AhModel;
using HtmlAgilityPack;

namespace GetCarDataService.GetAHAutoCarsData
{
    //从网络上请求参数配置分组
    class GetStyleProperty
    {
        private const int CompanyId = (int)CompanyEnum.AHauto;
        private const string DuibiUrl = "http://car.autohome.com.cn/duibi/chexing/carids=26768,0,0,0"; //给定一个车型id，解决无id时某些参配获取不到的问题

        /// <summary>
        /// 插入分组和动态属性
        /// </summary>
        public static void GetProperty()
        {
            try
            {
                HtmlDocument htmlDocument = Common.GetHtmlDocument(DuibiUrl);
                var groupNodes = htmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class,'js-title')]");
                var paraNodes = htmlDocument.DocumentNode.SelectNodes("//table[starts-with(@class,'js-titems')]");
                var xuanzhuangbao = htmlDocument.DocumentNode.SelectSingleNode("//div[starts-with(@data-title,'选装包')]");
                if (xuanzhuangbao != null && groupNodes.Contains(xuanzhuangbao))
                {
                    groupNodes.Remove(xuanzhuangbao);
                }

                if (groupNodes != null && paraNodes != null && (groupNodes.Count == paraNodes.Count))
                {
                    AnalysGroups(groupNodes, paraNodes);
                }
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "获取汽车之家参配名称ERROR:" + e.ToString());
            }
        }

        private static void AnalysGroups(HtmlNodeCollection groupNodes, HtmlNodeCollection paraNodes)
        {
            for (int i = 0; i < groupNodes.Count; i++)
            {
                var groupId = i + 1;
                string groupName = groupNodes[i].ChildNodes[1].InnerText.Trim();
                if (string.IsNullOrEmpty(groupName))
                {
                    continue;
                }
                bool flag = BasicStylePropertyGroupBll.AddStylePropertyGroup(groupId, groupName, CompanyId);
                if (flag && paraNodes[i].HasChildNodes)
                {
                    AnalysePara(groupId, paraNodes[i]);
                }
            }
        }

        private static void AnalysePara(int groupId, HtmlNode paraNode)
        {
            HtmlNodeCollection tbodyNodes = paraNode.ChildNodes;
            if (tbodyNodes.Count > 0)
            {
                foreach (HtmlNode trNode in tbodyNodes)
                {
                    int propertyId = AnalyzePropertyId(trNode);
                    if (propertyId == 0)
                    {
                        continue;
                    }
                    string stylePropertyName = trNode.FirstChild.FirstChild.InnerText.Trim();
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
            try
            {
                string hrefValue = trNode.FirstChild.FirstChild.GetAttributeValue("href", "");
                if (hrefValue.Length == 0)
                {
                    return 0;
                }
                int propertyId;
                int startIndex;
                if (hrefValue.Contains("?lang=")) //特殊处理，获取  href="/shuyu/detail_18_53_462.html?lang=459类型的参配id
                {
                    startIndex = hrefValue.LastIndexOf("=", StringComparison.Ordinal) + 1;
                    propertyId = Int32.Parse(hrefValue.Substring(startIndex).Trim());
                }
                else
                {
                    startIndex = hrefValue.LastIndexOf("_", StringComparison.Ordinal) + 1;
                    int endIndex = hrefValue.LastIndexOf(".", StringComparison.Ordinal);
                    if (endIndex < startIndex)
                    {
                        return 0;
                    }
                    propertyId = Int32.Parse(hrefValue.Substring(startIndex, endIndex - startIndex).Trim());
                }
                return propertyId;
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "获取汽车之家参配名称ID ERROR:" + e.ToString());
                return 0;
            }
        }

        /// <summary>
        /// 获取车型参配值时，将之前没获取到的参配分组和参配添加到数据库中
        /// </summary>
        /// <param name="groupNode"></param>
        /// <param name="paraNode"></param>
        public static void AnalysGroups(HtmlNode groupNode, HtmlNode paraNode)
        {
            var groupId = BasicStylePropertyGroupBll.GetMaxGroupId(CompanyId) + 1;
            string groupName = groupNode.ChildNodes[1].InnerText.Trim();
            if (string.IsNullOrEmpty(groupName))
            {
                return;
            }
            bool flag = BasicStylePropertyGroupBll.AddStylePropertyGroup(groupId, groupName, CompanyId);
            if (flag && paraNode.HasChildNodes)
            {
                AnalysePara(groupId, paraNode);
            }
        }
    }
}
