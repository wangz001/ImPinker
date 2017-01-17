using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DBUtility;
using ImModel;

namespace ImDal
{
	/// <summary>
	/// 数据访问类:User
	/// </summary>
	public partial class User
	{
		public User()
		{}
		#region  BasicMethod

		/// <summary>
		/// 得到最大ID
		/// </summary>
		public int GetMaxId()
		{
		return DbHelperSQL.GetMaxID("Id", "Users"); 
		}

		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int Id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from User");
			strSql.Append(" where Id=@Id");
			SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int,4)
			};
			parameters[0].Value = Id;

			return DbHelperSQL.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public int Add(Users model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("insert into Users(");
			strSql.Append("UserName,ShowName,PassWord,Sex,PhoneNum,Email,Age,ImgUrl,IsEnable,CreateTime,UpdateTime,AspNetId)");
			strSql.Append(" values (");
			strSql.Append("@UserName,@ShowName,@PassWord,@Sex,@PhoneNum,@Email,@Age,@ImgUrl,@IsEnable,@CreateTime,@UpdateTime,@AspNetId)");
			strSql.Append(";select @@IDENTITY");
			SqlParameter[] parameters = {
					new SqlParameter("@UserName", SqlDbType.VarChar,100){Value = model.UserName},
					new SqlParameter("@ShowName", SqlDbType.VarChar,100){Value = model.ShowName},
					new SqlParameter("@PassWord", SqlDbType.VarChar,20){Value = model.PassWord},
					new SqlParameter("@Sex", SqlDbType.Bit,1){Value = model.Sex},
					new SqlParameter("@PhoneNum", SqlDbType.VarChar,20){Value = model.PhoneNum},
					new SqlParameter("@Email", SqlDbType.VarChar,50){Value = model.Email},
					new SqlParameter("@Age", SqlDbType.Int,4){Value = model.Age},
					new SqlParameter("@ImgUrl", SqlDbType.VarChar,-1){Value = model.ImgUrl},
					new SqlParameter("@IsEnable", SqlDbType.Bit,1){Value = model.IsEnable},
					new SqlParameter("@CreateTime", SqlDbType.DateTime){Value = model.CreateTime},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime},
					new SqlParameter("@AspNetId", SqlDbType.NVarChar,128){Value = model.AspNetId}};
			
			object obj = DbHelperSQL.GetSingle(strSql.ToString(),parameters);
			if (obj == null)
			{
				return 0;
			}
			else
			{
				return Convert.ToInt32(obj);
			}
		}
		/// <summary>
		/// 更新一条数据
		/// </summary>
		public bool Update(Users model)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("update Users set ");
			strSql.Append("UserName=@UserName,");
			strSql.Append("ShowName=@ShowName,");
			strSql.Append("PassWord=@PassWord,");
			strSql.Append("Sex=@Sex,");
			strSql.Append("PhoneNum=@PhoneNum,");
			strSql.Append("Email=@Email,");
			strSql.Append("Age=@Age,");
			strSql.Append("ImgUrl=@ImgUrl,");
			strSql.Append("IsEnable=@IsEnable,");
			strSql.Append("CreateTime=@CreateTime,");
			strSql.Append("UpdateTime=@UpdateTime");
			strSql.Append(" where Id=@Id");
			SqlParameter[] parameters = {
					new SqlParameter("@UserName", SqlDbType.VarChar,100),
					new SqlParameter("@ShowName", SqlDbType.VarChar,100),
					new SqlParameter("@PassWord", SqlDbType.VarChar,20),
					new SqlParameter("@Sex", SqlDbType.Bit,1),
					new SqlParameter("@PhoneNum", SqlDbType.VarChar,20),
					new SqlParameter("@Email", SqlDbType.VarChar,50),
					new SqlParameter("@Age", SqlDbType.Int,4),
					new SqlParameter("@ImgUrl", SqlDbType.VarChar,-1),
					new SqlParameter("@IsEnable", SqlDbType.Bit,1),
					new SqlParameter("@CreateTime", SqlDbType.DateTime),
					new SqlParameter("@UpdateTime", SqlDbType.DateTime),
					new SqlParameter("@Id", SqlDbType.Int,4)};
			parameters[0].Value = model.UserName;
			parameters[1].Value = model.ShowName;
			parameters[2].Value = model.PassWord;
			parameters[3].Value = model.Sex;
			parameters[4].Value = model.PhoneNum;
			parameters[5].Value = model.Email;
			parameters[6].Value = model.Age;
			parameters[7].Value = model.ImgUrl;
			parameters[8].Value = model.IsEnable;
			parameters[9].Value = model.CreateTime;
			parameters[10].Value = model.UpdateTime;
			parameters[11].Value = model.Id;

			int rows=DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 删除一条数据
		/// </summary>
		public bool Delete(int Id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from Users ");
			strSql.Append(" where Id=@Id");
			SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int,4)
			};
			parameters[0].Value = Id;

			int rows=DbHelperSQL.ExecuteSql(strSql.ToString(),parameters);
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// 批量删除数据
		/// </summary>
		public bool DeleteList(string Idlist )
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("delete from Users ");
			strSql.Append(" where Id in ("+Idlist + ")  ");
			int rows=DbHelperSQL.ExecuteSql(strSql.ToString());
			if (rows > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Users GetModel(int Id)
		{
			
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select  top 1 Id,UserName,ShowName,PassWord,Sex,PhoneNum,Email,Age,ImgUrl,IsEnable,CreateTime,UpdateTime from Users ");
			strSql.Append(" where Id=@Id");
			SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int,4)
			};
			parameters[0].Value = Id;

			Users model=new Users();
			DataSet ds=DbHelperSQL.Query(strSql.ToString(),parameters);
			if(ds.Tables[0].Rows.Count>0)
			{
				return DataRowToModel(ds.Tables[0].Rows[0]);
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Users DataRowToModel(DataRow row)
		{
			Users model=new Users();
			if (row != null)
			{
				if(row["Id"]!=null && row["Id"].ToString()!="")
				{
					model.Id=int.Parse(row["Id"].ToString());
				}
				if(row["UserName"]!=null)
				{
					model.UserName=row["UserName"].ToString();
				}
				if(row["ShowName"]!=null)
				{
					model.ShowName=row["ShowName"].ToString();
				}
				if(row["PassWord"]!=null)
				{
					model.PassWord=row["PassWord"].ToString();
				}
				if(row["Sex"]!=null && row["Sex"].ToString()!="")
				{
					if((row["Sex"].ToString()=="1")||(row["Sex"].ToString().ToLower()=="true"))
					{
						model.Sex=true;
					}
					else
					{
						model.Sex=false;
					}
				}
				if(row["PhoneNum"]!=null)
				{
					model.PhoneNum=row["PhoneNum"].ToString();
				}
				if(row["Email"]!=null)
				{
					model.Email=row["Email"].ToString();
				}
				if(row["Age"]!=null && row["Age"].ToString()!="")
				{
					model.Age=int.Parse(row["Age"].ToString());
				}
				if(row["ImgUrl"]!=null)
				{
					model.ImgUrl=row["ImgUrl"].ToString();
				}
				if(row["IsEnable"]!=null && row["IsEnable"].ToString()!="")
				{
					if((row["IsEnable"].ToString()=="1")||(row["IsEnable"].ToString().ToLower()=="true"))
					{
						model.IsEnable=true;
					}
					else
					{
						model.IsEnable=false;
					}
				}
				if(row["CreateTime"]!=null && row["CreateTime"].ToString()!="")
				{
					model.CreateTime=DateTime.Parse(row["CreateTime"].ToString());
				}
				if(row["UpdateTime"]!=null && row["UpdateTime"].ToString()!="")
				{
					model.UpdateTime=DateTime.Parse(row["UpdateTime"].ToString());
				}
                if (row["AspNetId"] != null)
                {
                    model.AspNetId = row["AspNetId"].ToString();
                }
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select Id,UserName,ShowName,PassWord,Sex,PhoneNum,Email,Age,ImgUrl,IsEnable,CreateTime,UpdateTime ");
			strSql.Append(" FROM Users ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperSQL.Query(strSql.ToString());
		}

		/// <summary>
		/// 获得前几行数据
		/// </summary>
		public DataSet GetList(int Top,string strWhere,string filedOrder)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select ");
			if(Top>0)
			{
				strSql.Append(" top "+Top.ToString());
			}
			strSql.Append(" Id,UserName,ShowName,PassWord,Sex,PhoneNum,Email,Age,ImgUrl,IsEnable,CreateTime,UpdateTime ");
			strSql.Append(" FROM Users ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			strSql.Append(" order by " + filedOrder);
			return DbHelperSQL.Query(strSql.ToString());
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) FROM Users ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			object obj = DbHelperSQL.GetSingle(strSql.ToString());
			if (obj == null)
			{
				return 0;
			}
			else
			{
				return Convert.ToInt32(obj);
			}
		}
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			if (!string.IsNullOrEmpty(orderby.Trim()))
			{
				strSql.Append("order by T." + orderby );
			}
			else
			{
				strSql.Append("order by T.Id desc");
			}
			strSql.Append(")AS Row, T.*  from Users T ");
			if (!string.IsNullOrEmpty(strWhere.Trim()))
			{
				strSql.Append(" WHERE " + strWhere);
			}
			strSql.Append(" ) TT");
			strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
			return DbHelperSQL.Query(strSql.ToString());
		}

		/*
		/// <summary>
		/// 分页获取数据列表
		/// </summary>
		public DataSet GetList(int PageSize,int PageIndex,string strWhere)
		{
			SqlParameter[] parameters = {
					new SqlParameter("@tblName", SqlDbType.VarChar, 255),
					new SqlParameter("@fldName", SqlDbType.VarChar, 255),
					new SqlParameter("@PageSize", SqlDbType.Int),
					new SqlParameter("@PageIndex", SqlDbType.Int),
					new SqlParameter("@IsReCount", SqlDbType.Bit),
					new SqlParameter("@OrderType", SqlDbType.Bit),
					new SqlParameter("@strWhere", SqlDbType.VarChar,1000),
					};
			parameters[0].Value = "User";
			parameters[1].Value = "Id";
			parameters[2].Value = PageSize;
			parameters[3].Value = PageIndex;
			parameters[4].Value = 0;
			parameters[5].Value = 0;
			parameters[6].Value = strWhere;	
			return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
		}*/

		#endregion  BasicMethod
		#region  ExtensionMethod

		#endregion  ExtensionMethod

		public Users GetModelByAspNetId(string aspnetid)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select  top 1 Id,UserName,ShowName,PassWord,Sex,PhoneNum,Email,Age,ImgUrl,IsEnable,CreateTime,UpdateTime,AspNetId from Users ");
			strSql.Append(" where AspNetId=@AspNetId");
			SqlParameter[] parameters = {
					new SqlParameter("@AspNetId", SqlDbType.NVarChar){Value = aspnetid}
			};
			Users model = new Users();
			DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
			if (ds.Tables[0].Rows.Count > 0)
			{
				return DataRowToModel(ds.Tables[0].Rows[0]);
			}
			else
			{
				return null;
			}
		}

	    public Users GetModelByPhoneNum(string phoneNum)
	    {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 Id,UserName,ShowName,PassWord,Sex,PhoneNum,Email,Age,ImgUrl,IsEnable,CreateTime,UpdateTime,AspNetId from Users ");
            strSql.Append(" where PhoneNum=@PhoneNum");
            SqlParameter[] parameters = {
					new SqlParameter("@PhoneNum", SqlDbType.VarChar){Value = phoneNum}
			};
            Users model = new Users();
            DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
	    }
	}
}

