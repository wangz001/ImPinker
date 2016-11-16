﻿using System;
using System.Collections.Generic;
using System.Data;
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
		private readonly ArticleDal dal=new ArticleDal();
		
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
		/// 删除一条数据
		/// </summary>
		public bool DeleteList(string Idlist )
		{
			return dal.DeleteList(Idlist );
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
				catch{}
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
		/// 分页获取数据列表
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			return dal.GetRecordCount(strWhere);
		}
		/// <summary>
		/// 分页获取用户数据列表
		/// </summary>
		public DataSet GetMyListByPage( int userid, int pageNum, int count)
		{
			return dal.GetMyListByPage( userid,pageNum,count);
		}

        /// <summary>
        /// 分页获取首页数据列表
        /// </summary>
        public List<ArticleViewModel> GetIndexListByPage(int pageNum, int count)
        {
            var articleNameLists = new List<string>();
            var listResult = new List<ArticleViewModel>();
            var ds= dal.GetIndexListByPage(pageNum, count);
            List<Article> articles = DataTableToList(ds.Tables[0]);
            if (articles != null && articles.Count > 0)
            {
                foreach (var article in articles)
                {
                    if (article.ArticleName.Length > 25)
                    {
                        article.ArticleName = article.ArticleName.Substring(0, 25) + "……";
                    }
                    if (articleNameLists.Contains(article.ArticleName))
                    {//去除标题重复的数据,解决fblife 同一文章发在不同域名的问题
                        continue;
                    }
                    articleNameLists.Add(article.ArticleName);
                    listResult.Add(new ArticleViewModel()
                    {
                        Id = article.Id.ToString(),
                        ArticleName = article.ArticleName,
                        Url = article.Url,
                        Description = article.Description,
                        KeyWords = article.KeyWords,
                        CoverImage = article.CoverImage,
                        Company = article.Company,
                        CreateTime = article.CreateTime
                    });
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
	    public bool UpdateCoverImage(long articleId,string newUrl)
	    {
            var article = GetModelByCache(articleId);
	        if (article!=null)
	        {
	            article.CoverImage = newUrl;
	            article.UpdateTime = DateTime.Now;
	            return dal.Update(article);
	        }
	        return false;
	    }
	}
}

