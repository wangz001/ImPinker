using System;
using AhDal;
using AhModel;
using HtmlAgilityPack;

namespace GetCarDataService.GetAutoHomeCarsData
{
    //从网络上请求参数配置分组
    class GetStyleProperty
    {
        static readonly AHStylePropertyGroupDal AhStylePropertyGroupDal = new AHStylePropertyGroupDal();
        static readonly AhStylePropertyDal AhStylePropertyDal = new AhStylePropertyDal();
        private const string DuibiUrl = "http://car.autohome.com.cn/duibi/chexing/carids=18111,0,0,0"; //给定一个车型id，解决无id时某些参配获取不到的问题

        /// <summary>
        /// 插入分组和动态属性
        /// </summary>
        public static void Get()
        {
            try
            {
                HtmlDocument htmlDocument = Common.GetHtmlDocument(DuibiUrl);
                var groupNodes = htmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class,'js-title')]");
                var paraNodes = htmlDocument.DocumentNode.SelectNodes("//table[starts-with(@class,'js-titems')]");
                if (groupNodes != null && paraNodes != null && (groupNodes.Count == paraNodes.Count))
                {
                    AnalysGroups(groupNodes,paraNodes);
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
                string groupName = groupNodes[i].ChildNodes[1].InnerText.Trim();
                if (string.IsNullOrEmpty(groupName))
                {
                    continue;
                }
                var model = new AhStylePropertyGroup {Name = groupName};

                var stylePropertyGroup = AhStylePropertyGroupDal.Exists(groupName);

                if (stylePropertyGroup == null) AhStylePropertyGroupDal.Add(model);

                var ahStylePropertyGroup = AhStylePropertyGroupDal.GetModel(groupName);

                if (ahStylePropertyGroup != null)
                {
                    int groupId = ahStylePropertyGroup.ID;
                    if (paraNodes[i].HasChildNodes)
                    {
                        AnalysePara(groupId,paraNodes[i]);
                    }
                }
                else
                {
                    Console.WriteLine("参配分组'{0}'在数据库中不存在，请核对！", groupName);
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
                    int propertyId=AnalyzePropertyId(trNode);
                    if (propertyId==0)
                    {
                        continue;
                    }

                    string stylePropertyName = trNode.FirstChild.FirstChild.InnerText.Trim();
                    if (string.IsNullOrEmpty(stylePropertyName))
                    {
                        continue;
                    }
                    var ahStyleProperty = new AhStyleProperty
                    {
                        ID = propertyId,
                        Name = stylePropertyName,
                        PropertyGroupID = groupId
                    };
                    bool exist = AhStylePropertyDal.Exists(ahStyleProperty.ID);
                    if (!exist)
                    {
                        AhStylePropertyDal.Add(ahStyleProperty);
                    }
                    else
                    {
                        AhStylePropertyDal.Update(ahStyleProperty);
                    }
                }
            }
        }

        public static int AnalyzePropertyId(HtmlNode trNode)
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
                propertyId = Int32.Parse(hrefValue.Substring(startIndex, endIndex - startIndex).Trim());
            }
            return propertyId;
        }
    }
}
