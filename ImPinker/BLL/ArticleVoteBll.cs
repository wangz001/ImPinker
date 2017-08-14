using System;
using System.Collections.Generic;
using System.Data;
using ImModel;
using ImModel.Enum;
using Maticsoft.Common;

namespace ImBLL
{
	/// <summary>
	/// ArticleVote
	/// </summary>
	public class ArticleVoteBll
	{
		private readonly ImDal.ArticleVote _dal=new ImDal.ArticleVote();
        readonly NotifyBll _notifyBll = new NotifyBll();
		
        /// <summary>
        /// 添加记录。一个人只能对一篇文章投票一次
        /// </summary>
        /// <param name="voteModel"></param>
        /// <returns></returns>
	    public bool AddVote(ArticleVote voteModel)
	    {
            if (Exists(voteModel.ArticleId,voteModel.UserId))
            {
                return false;
            }
            voteModel.CreateTime = DateTime.Now;
            voteModel.UpdateTime = DateTime.Now;
            var flag= Add(voteModel);
            if (!flag)
            {
                return false;
            }
            //通知信息
            flag = _notifyBll.NewNotify(NotifyTypeEnum.Remind, (int)voteModel.ArticleId, TargetTypeEnum.Article, ActionEnum.Vote, voteModel.UserId,"文章点赞");
            return flag;
	    }

		#region  BasicMethod
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(long articleId,int userId)
		{
			return _dal.Exists(articleId,userId);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		private bool Add(ArticleVote model)
		{
			return _dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(ArticleVote model)
		{
			return _dal.Update(model);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(long Id)
		{
			
			return _dal.Delete(Id);
		}
		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool DeleteList(string Idlist )
		{
			return _dal.DeleteList(Idlist );
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public ArticleVote GetModel(long Id)
		{
			
			return _dal.GetModel(Id);
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
					objModel = _dal.GetModel(Id);
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
			return _dal.GetList(strWhere);
		}
		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public DataSet GetList(int Top,string strWhere,string filedOrder)
		{
			return _dal.GetList(Top,strWhere,filedOrder);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<ArticleVote> GetModelList(string strWhere)
		{
			DataSet ds = _dal.GetList(strWhere);
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
					model = _dal.DataRowToModel(dt.Rows[n]);
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
			return _dal.GetRecordCount(strWhere);
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			return _dal.GetListByPage( strWhere,  orderby,  startIndex,  endIndex);
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

