using System;
using System.Collections.Generic;
using System.Data;
using ImModel;
using Maticsoft.Common;

namespace ImBLL
{
	/// <summary>
	/// ArticleSnaps
	/// </summary>
	public class ArticleSnapsBll
	{
		private readonly ImDal.ArticleSnaps dal=new ImDal.ArticleSnaps();

        /// <summary>
        /// 添加草稿（发帖子用）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
	    public bool AddDraft(ArticleSnaps model)
	    {
            if (Exists(model.ArticleId))
            {
                var snap = GetModel(model.ArticleId);
                snap.Content = model.Content;
                snap.UpdateTime = DateTime.Now;
                return dal.Update(model);
            }
            else
            {
                return dal.Add(model);
            }
	    }

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(long Id)
		{
			return dal.Exists(Id);
		}

		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public ArticleSnaps GetModel(long Id)
		{
			return dal.GetModel(Id);
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
		public List<ArticleSnaps> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		private List<ArticleSnaps> DataTableToList(DataTable dt)
		{
			List<ArticleSnaps> modelList = new List<ArticleSnaps>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				ArticleSnaps model;
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
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			return dal.GetListByPage( strWhere,  orderby,  startIndex,  endIndex);
		}
		
	}
}

