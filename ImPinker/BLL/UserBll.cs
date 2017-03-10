using System;
using System.Collections.Generic;
using System.Data;
using ImDal;
using ImModel;
using Maticsoft.Common;

namespace ImBLL
{
	/// <summary>
	/// User
	/// </summary>
	public partial class UserBll
	{
		private readonly User dal=new User();
		public UserBll()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
			return dal.GetMaxId();
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int Id)
		{
			return dal.Exists(Id);
		}

		/// <summary>
		/// 增加一条数据
		/// </summary>
		public int  Add(Users model)
		{
		    if (string.IsNullOrEmpty(model.AspNetId))
		    {
                //api 手机验证码登录
		        model.AspNetId = "";
		    }
		    model.CreateTime = DateTime.Now;
		    model.UpdateTime = DateTime.Now;
			return dal.Add(model);
		}

		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Users model)
		{
			return dal.Update(model);
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int Id)
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
		public Users GetModel(int Id)
		{
			
			return dal.GetModel(Id);
		}

		/// <summary>
		/// 得到一个对象实体，从缓存中
		/// </summary>
		public Users GetModelByCache(int Id)
		{
			string CacheKey = "UserModel-" + Id;
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
				catch{
                
                }
			}
			return (Users)objModel;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspnetId">系统生成的md5  id</param>
        /// <returns></returns>
		public Users GetModelByAspNetId(string aspnetId)
		{
			string CacheKey = "UserModel-" + aspnetId;
			object objModel = DataCache.GetCache(CacheKey);
			if (objModel == null)
			{
				try
				{
					objModel = dal.GetModelByAspNetId(aspnetId);
					if (objModel != null)
					{
						int ModelCache = ConfigHelper.GetConfigInt("ModelCache");
						DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
					}
				}
				catch { }
			}
			return (Users)objModel;
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
		public List<Users> GetModelList(string strWhere)
		{
			DataSet ds = dal.GetList(strWhere);
			return DataTableToList(ds.Tables[0]);
		}
		/// <summary>
		/// 获得数据列表
		/// </summary>
		public List<Users> DataTableToList(DataTable dt)
		{
			List<Users> modelList = new List<Users>();
			int rowsCount = dt.Rows.Count;
			if (rowsCount > 0)
			{
				Users model;
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
		
        /// <summary>
        /// 修改用户头像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newimgUrl"></param>
        /// <returns></returns>
	    public bool UpdateHeadImg(int userId, string newimgUrl)
		{
		    var user = dal.GetModel(userId);
		    user.ImgUrl = newimgUrl;
		    user.UpdateTime = DateTime.Now;
		    return dal.Update(user);
		}
        /// <summary>
        /// 判断该电话号码是否被注册
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
	    public Users GetModelByPhoneNum(string phoneNum)
	    {
            if (string.IsNullOrEmpty(phoneNum))
            {
                return null;
            }
            var user = dal.GetModelByPhoneNum(phoneNum);
            return user;
	    }
        /// <summary>
        /// 根据用户名获取用户（登录用）
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
	    public Users GetModelByUserName(string username)
	    {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            var user = dal.GetModelByUserName(username);
            return user;
	    }
	}
}

