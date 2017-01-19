﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Common.DateTimeUtil;
using ImDal;
using ImModel;
using ImModel.ViewModel;
using Maticsoft.Common;

namespace ImBLL
{
    /// <summary>
    /// Article
    /// </summary>
    public class ArticleBll
    {
        private readonly ArticleDal dal = new ArticleDal();
        private readonly ArticleSnapsBll articleSnapsBll = new ArticleSnapsBll();

        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(long Id)
        {
            return dal.Exists(Id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(Article model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Article model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(long Id)
        {
            return dal.Delete(Id);
        }
        /// <summary>
        /// 用户删除帖子
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool DeleteThread(int userid, long Id)
        {
            return dal.DeleteThread(userid, Id);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string Idlist)
        {
            return dal.DeleteList(Idlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Article GetModel(long Id)
        {
            return dal.GetModel(Id);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Article GetModelByCache(long Id)
        {
            string CacheKey = "ArticleModel-" + Id;
            object objModel = DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = dal.GetModel(Id);
                    if (objModel != null)
                    {
                        int ModelCache = ConfigHelper.GetConfigInt("ModelCache");
                        DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
                    }
                }
                catch { }
            }
            return (Article)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Article> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Article> DataTableToList(DataTable dt)
        {
            var modelList = new List<Article>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                for (int n = 0; n < rowsCount; n++)
                {
                    Article model = dal.DataRowToModel(dt.Rows[n]);
                    if (model != null)
                    {
                        modelList.Add(model);
                    }
                }
            }
            return modelList;
        }


        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            return dal.GetRecordCount(strWhere);
        }
        /// <summary>
        /// 分页获取用户数据列表,包括已发布的,待审核,审核未通过d 
        /// </summary>
        public List<Article> GetMyListByPage(int userid, int pageNum, int count, out int totalaCount)
        {
            totalaCount = 0;
            var ds = dal.GetMyListByPage(userid, pageNum, count);
            var list = new List<Article>();
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                list = DataTableToList(ds.Tables[0]);
            }
            if (ds != null && ds.Tables[1] != null)
            {
                int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out totalaCount);
            }
            return list;
        }

        /// <summary>
        /// 分页获取首页数据列表,coverimage 不为空
        /// </summary>
        public List<ArticleViewModel> GetIndexListByPage(int pageNum, int count)
        {
            var articleNameLists = new List<string>();
            var listResult = new List<ArticleViewModel>();
            var ds = dal.GetIndexListByPage(pageNum, count);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = new ArticleViewModel();
                    if (row.Table.Columns.Contains("Id") && row["Id"] != null && row["Id"].ToString() != "")
                    {
                        model.Id = row["Id"].ToString();
                    }
                    if (row.Table.Columns.Contains("ArticleName") && row["ArticleName"] != null)
                    {
                        model.ArticleName = row["ArticleName"].ToString();
                    }
                    if (model.ArticleName.Length > 25)
                    {
                        model.ArticleName = model.ArticleName.Substring(0, 25) + "……";
                    }
                    if (row.Table.Columns.Contains("Url") && row["Url"] != null)
                    {
                        model.Url = row["Url"].ToString();
                    }
                    if (row.Table.Columns.Contains("CoverImage") && row["CoverImage"] != null)
                    {
                        model.CoverImage = row["CoverImage"].ToString();
                    }
                    if (row.Table.Columns.Contains("KeyWords") && row["KeyWords"] != null)
                    {
                        model.KeyWords = row["KeyWords"].ToString();
                    }
                    //keywords只去一个，首页及搜索页显示用
                    var keyStr = model.KeyWords;
                    var keyArr = keyStr.Split(',');
                    if (keyArr.Length > 1)
                    {
                        model.KeyWords = keyArr[1];
                    }
                    else
                    {
                        model.KeyWords = "";
                    }
                    if (row.Table.Columns.Contains("CreateTime") && row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                    {
                        model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                    }
                    if (row.Table.Columns.Contains("Company") && row["Company"] != null && row["Company"].ToString() != "")
                    {
                        model.Company = row["Company"].ToString();
                    }
                    if (row.Table.Columns.Contains("voteCount") && row["voteCount"] != null && row["voteCount"].ToString() != "")
                    {
                        model.VoteCount = int.Parse(row["voteCount"].ToString());
                    }
                    listResult.Add(model);
                }
            }
            return listResult;
        }


        #endregion  BasicMethod

        /// <summary>
        /// 更新封面图地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="newUrl"></param>
        /// <returns></returns>
        public bool UpdateCoverImage(long articleId, string newUrl)
        {
            var article = GetModelByCache(articleId);
            if (article != null)
            {
                article.CoverImage = newUrl;
                article.UpdateTime = DateTime.Now;
                return dal.Update(article);
            }
            return false;
        }
        /// <summary>
        /// 获取所有还未生成封面图oss的记录
        /// </summary>
        /// <returns></returns>
        public List<Article> GetArticlesWithoutCoverImage()
        {
            const string whereStr = " State =1 AND CoverImage IS NULL OR DATALENGTH(CoverImage)=0 ";
            var list = GetModelList(whereStr);
            return list;
        }

        /// <summary>
        /// 获取model，包含snap表的content
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArticleViewModel GetModelWithContent(long id)
        {
            var article = dal.GetModel(id);
            var snap = articleSnapsBll.GetModel(id);
            
            var vm = new ArticleViewModel()
            {
                Id = id.ToString(),
                ArticleName = article.ArticleName,
                Url = article.Url,
                Userid = article.UserId.ToString(),
                CoverImage = article.CoverImage,
                KeyWords = article.KeyWords,
                Description = article.Description,
                Company = article.Company,
                CreateTime = article.CreateTime,
                Content =snap==null? null: new List<Object>() { snap.Content }
            };
            return vm;
        }
        /// <summary>
        /// 发布新帖子。操作article和articlesnap表，使用事务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddThread(CreateThreadVm vm)
        {
            return dal.AddThread(vm);
        }

        public bool UpdateThread(CreateThreadVm vm)
        {
            return dal.UpdateThread(vm);
        }
    
        /// <summary>
        /// 修改文章状态
        /// </summary>
        /// <param name="articleStateEnum"></param>
        public bool UpdateState(long articleId,ArticleStateEnum articleState)
        {
            var article = this.GetModelByCache(articleId);
            article.State = (int)articleState;
            return Update(article);
        }
    }
}

