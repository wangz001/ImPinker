using System;
using System.Collections.Generic;
using System.Diagnostics;
using AhBll;
using AhModel;
using ImBLL;
using ImModel;
using ImModel.Enum;
using Quartz;

namespace GetCarDataService.ArticleTagManage
{
    /// <summary>
    /// ArticleTag  添加，用于标签前台展示用
    /// </summary>
    public class ArticleTagAdd : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            InsertArticleKeyWordTag();
            InsertCarDataTag();
        }
        /// <summary>
        /// 把article表中，keyword字段中的关键词，取出分析后插入表中
        /// </summary>
        private void InsertArticleKeyWordTag()
        {
            var dicList = new List<string>(); //词典
            var articleBll = new ArticleBll();
            var dtNow = DateTime.Now;//后期做成定时增量更新
            var articleLists = articleBll.GetModelList(" CreateTime > '" + dtNow.AddDays(-12)+"'");
            foreach (var article in articleLists)
            {
                var keyWordArr = article.KeyWords.Split(',');
                foreach (var s in keyWordArr)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        AddToDicList(ref dicList, s);
                    }
                }
            }
            SaveToDb(dicList);
        }

        /// <summary>
        /// 插入车型相关的标签，
        /// </summary>
        private void InsertCarDataTag()
        {
            var dicList = new List<string>(); //词典
            var list = new BasicSerialBll().GetAllSerials((int)CompanyEnum.AHauto);
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
            SaveToDb(dicList);
        }

        private void SaveToDb(List<string> dicList)
        {
            var articleTagBll = new ArticleTagBll();
            foreach (var tag in dicList)
            {
                var model = new ArticleTag
                {
                    UserId = 2,
                    TagName = tag,
                    FrountShowState = FrountShowStateEnum.FrountShow,
                    IsDelete = false,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                if (!articleTagBll.Exists(model.TagName))
                {
                    articleTagBll.Add(model);
                }
                Console.WriteLine(tag);
            }

        }


        /// <summary>
        /// 向词典里添加内容
        /// </summary>
        /// <param name="dicList"></param>
        /// <param name="nameStr"></param>
        /// <returns></returns>
        private bool AddToDicList(ref List<string> dicList, string nameStr)
        {
            if (dicList != null && !string.IsNullOrEmpty(nameStr) && !dicList.Contains(nameStr))
            {
                dicList.Add(nameStr);
                return true;
            }
            return false;
        }
    }
}
