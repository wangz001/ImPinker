using System;
using System.Collections.Generic;
using System.Data;
using ImModel;
using Maticsoft.Common;

namespace ImBLL
{
	/// <summary>
	/// ArticleVote
	/// </summary>
	public class ArticleVoteBll
	{
		private readonly ImDal.ArticleVote dal=new ImDal.ArticleVote();
		public ArticleVoteBll()
		{}
        /// <summary>
        /// 添加记录。一个人只能对一篇文章投票一次
        /// </summary>
        /// <param name="vote"></param>
        /// <returns></returns>
	    public bool AddVote(ArticleVote vote)
	    {
            if (Exists(vote.ArticleId,vote.UserId))
            {
                return false;
            }
            else
            {
                vote.CreateTime = DateTime.Now;
                vote.UpdateTime = DateTime.Now;
                return Add(vote);
            }
	    }

		#region  BasicMethod
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(long articleId,int userId)
		{
			return dal.Exists(articleId,userId);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(ArticleVote model)
		{
			return dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(ArticleVote model)
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
		public ArticleVote GetModel(long Id)
		{
			
			return dal.GetModel(Id);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public ArticleVote GetModelByCache(long Id)
		{
			
			string CacheKey = "ArticleVoteModel-" + Id;
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
			return (ArticleVote)objModel;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			return dal.GetList(strWhere);
		}
		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public DataSet GetList(int Top,string strWhere,string filedOrder)
		{
			return dal.GetList(Top,strWhere,filedOrder);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<ArticleVote> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<ArticleVote> DataTableToList(DataTable dt)
		{
			List<ArticleVote> modelList = new List<ArticleVote>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				ArticleVote model;
				for (int n = 0; n < rowsCount; n++)
				{
					model = dal.DataRowToModel(dt.Rows[n]);
					if (model != null)
					{
						modelList.Add(model);
					}
				}
			}
			return modelList;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetAllList()
		{
			return GetList("");
		}

		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			return dal.GetRecordCount(strWhere);
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			return dal.GetListByPage( strWhere,  orderby,  startIndex,  endIndex);
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		//public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		//{
			//return dal.GetList(PageSize,PageIndex,strWhere);
		//}

		#endregion  BasicMethod
		
	}
}

