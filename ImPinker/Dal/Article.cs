using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DBUtility;
using ImModel;

namespace ImDal
{
	/// <summary>
	/// 数据访问类:Article
	/// </summary>
	public class ArticleDal
	{
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(long Id)
		{
			var strSql = new StringBuilder();
			strSql.Append("select count(1) from Article");
			strSql.Append(" where Id=@Id ");
			SqlParameter[] parameters =
			{
				new SqlParameter("@Id", SqlDbType.BigInt, 8)
			};
			parameters[0].Value = Id;

			return DbHelperSQL.Exists(strSql.ToString(), parameters);
		}


		/// <summary>
		/// 增加一条数据
		/// </summary>
		public bool Add(Article model)
		{
			var strSql = new StringBuilder();
			strSql.Append("insert into Article(");
            strSql.Append("ArticleName,Url,CoverImage,UserId,KeyWords,Description,State,CreateTime,UpdateTime,PublishTime,Company)");
			strSql.Append(" values (");
            strSql.Append("@ArticleName,@Url,@CoverImage,@UserId,@KeyWords,@Description,@State,@CreateTime,@UpdateTime,@Company)");
			SqlParameter[] parameters =
			{
				new SqlParameter("@ArticleName", SqlDbType.NVarChar, 100){Value =model.ArticleName },
				new SqlParameter("@Url", SqlDbType.VarChar, 200){Value =model.Url },
				new SqlParameter("@CoverImage", SqlDbType.VarChar, 100){Value =model.CoverImage },
				new SqlParameter("@UserId", SqlDbType.Int, 4){Value = model.UserId},
				new SqlParameter("@KeyWords", SqlDbType.NVarChar, 100){Value =model.KeyWords },
				new SqlParameter("@Description", SqlDbType.NVarChar, 200){Value =model.Description },
				new SqlParameter("@State", SqlDbType.TinyInt, 1){Value =model.State },
				new SqlParameter("@CreateTime", SqlDbType.DateTime){Value =model.CreateTime },
				new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime},
				new SqlParameter("@PublishTime", SqlDbType.DateTime){Value = model.PublishTime},
                new SqlParameter("@Company",SqlDbType.Char){Value = model.Company}, 
			};
			
			int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
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
		/// 更新一条数据
		/// </summary>
		public bool Update(Article model)
		{
			var strSql = new StringBuilder();
			strSql.Append("update Article set ");
			strSql.Append("ArticleName=@ArticleName,");
			strSql.Append("Url=@Url,");
			strSql.Append("CoverImage=@CoverImage,");
			strSql.Append("UserId=@UserId,");
			strSql.Append("KeyWords=@KeyWords,");
			strSql.Append("Description=@Description,");
			strSql.Append("State=@State,");
			strSql.Append("UpdateTime=@UpdateTime");
			strSql.Append(" where Id=@Id ");
			SqlParameter[] parameters =
			{
				new SqlParameter("@ArticleName", SqlDbType.NVarChar, 100),
				new SqlParameter("@Url", SqlDbType.VarChar, 200),
				new SqlParameter("@CoverImage", SqlDbType.VarChar, 100),
				new SqlParameter("@UserId", SqlDbType.Int, 4),
				new SqlParameter("@KeyWords", SqlDbType.NVarChar, 100),
				new SqlParameter("@Description", SqlDbType.NVarChar, 200),
				new SqlParameter("@State", SqlDbType.TinyInt, 1),
				new SqlParameter("@UpdateTime", SqlDbType.DateTime),
				new SqlParameter("@Id", SqlDbType.BigInt, 8)
			};
			parameters[0].Value = model.ArticleName;
			parameters[1].Value = model.Url;
			parameters[2].Value = model.CoverImage;
			parameters[3].Value = model.UserId;
			parameters[4].Value = model.KeyWords;
			parameters[5].Value = model.Description;
			parameters[6].Value = model.State;
			parameters[7].Value = model.CreateTime;
			parameters[8].Value = model.UpdateTime;
			parameters[9].Value = model.Id;

			int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
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
		public bool Delete(long Id)
		{

			var strSql = new StringBuilder();
			strSql.Append("delete from Article ");
			strSql.Append(" where Id=@Id ");
			SqlParameter[] parameters =
			{
				new SqlParameter("@Id", SqlDbType.BigInt, 8)
			};
			parameters[0].Value = Id;

			int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
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
		public bool DeleteList(string Idlist)
		{
			var strSql = new StringBuilder();
			strSql.Append("delete from Article ");
			strSql.Append(" where Id in (" + Idlist + ")  ");
			int rows = DbHelperSQL.ExecuteSql(strSql.ToString());
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
		public Article GetModel(long Id)
		{

			StringBuilder strSql = new StringBuilder();
			strSql.Append(
                "select  top 1 Id,ArticleName,Url,CoverImage,UserId,KeyWords,Description,Company,State,CreateTime,UpdateTime,PublishTime from Article ");
			strSql.Append(" where Id=@Id ");
			SqlParameter[] parameters =
			{
				new SqlParameter("@Id", SqlDbType.BigInt, 8)
			};
			parameters[0].Value = Id;

			Article model = new Article();
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


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public Article DataRowToModel(DataRow row)
		{
			var model = new Article();
			if (row != null)
			{
				if (row["Id"] != null && row["Id"].ToString() != "")
				{
					model.Id = long.Parse(row["Id"].ToString());
				}
				if (row["ArticleName"] != null)
				{
					model.ArticleName = row["ArticleName"].ToString();
				}
				if (row["Url"] != null)
				{
					model.Url = row["Url"].ToString();
				}
				if (row["CoverImage"] != null)
				{
					model.CoverImage = row["CoverImage"].ToString();
				}
				if (row["UserId"] != null && row["UserId"].ToString() != "")
				{
					model.UserId = int.Parse(row["UserId"].ToString());
				}
				if (row["KeyWords"] != null)
				{
					model.KeyWords = row["KeyWords"].ToString();
				}
				if (row["Description"] != null)
				{
					model.Description = row["Description"].ToString();
				}
				if (row["State"] != null && row["State"].ToString() != "")
				{
					model.State = int.Parse(row["State"].ToString());
				}
				if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
				{
					model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
				}
				if (row["UpdateTime"] != null && row["UpdateTime"].ToString() != "")
				{
					model.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
				}
                if (row["PublishTime"] != null && row["PublishTime"].ToString() != "")
                {
                    model.PublishTime = DateTime.Parse(row["PublishTime"].ToString());
                }
                if (row["Company"] != null && row["Company"].ToString() != "")
                {
                    model.Company = row["Company"].ToString();
                }
			}
			return model;
		}

		/// <summary>
		/// 获得数据列表
		/// </summary>
		public DataSet GetList(string strWhere)
		{
			StringBuilder strSql = new StringBuilder();
            strSql.Append("select Id,ArticleName,Url,CoverImage,UserId,KeyWords,Description,State,CreateTime,UpdateTime ");
			strSql.Append(" FROM Article ");
			if (strWhere.Trim() != "")
			{
				strSql.Append(" where " + strWhere);
			}
			return DbHelperSQL.Query(strSql.ToString());
		}

		/// <summary>
		/// 获取记录总数
		/// </summary>
		public int GetRecordCount(string strWhere)
		{
			StringBuilder strSql = new StringBuilder();
			strSql.Append("select count(1) FROM Article ");
			if (strWhere.Trim() != "")
			{
				strSql.Append(" where " + strWhere);
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
		public DataSet GetMyListByPage(int userid, int pageNum, int count)
		{
			var strSql = new StringBuilder();
			strSql.Append("SELECT * FROM ( ");
			strSql.Append(" SELECT ROW_NUMBER() OVER (");
			strSql.Append("order by T.CreateTime desc");
			strSql.Append(")AS Row, T.*  from Article T ");
            strSql.Append(" WHERE UserId=@UserId");
			strSql.Append(" ) TT");
			strSql.AppendFormat(" WHERE TT.Row between @startIndex and @endIndex");
		    var startIndex = (pageNum - 1)*count+1;
		    var endIndex = pageNum*count;
            var paras = new SqlParameter[]
		    {
                new SqlParameter("@UserId",SqlDbType.Int){Value = userid},
                new SqlParameter("@startIndex",SqlDbType.Int){Value = startIndex},
                new SqlParameter("@endIndex",SqlDbType.Int){Value = endIndex},
		    };
			return DbHelperSQL.Query(strSql.ToString(),paras);
		}

        /// <summary>
        /// 获取首页的文章
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
	    public DataSet GetIndexListByPage(int pageNum, int count)
	    {
            var strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            strSql.Append("order by T.CreateTime desc");
            strSql.Append(")AS Row, T.*  from Article T ");
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between @startIndex and @endIndex");
            var startIndex = (pageNum - 1) * count + 1;
            var endIndex = pageNum * count;
            var paras = new SqlParameter[]
		    {
                new SqlParameter("@startIndex",SqlDbType.Int){Value = startIndex},
                new SqlParameter("@endIndex",SqlDbType.Int){Value = endIndex},
		    };
            return DbHelperSQL.Query(strSql.ToString(), paras);
	    }
	}
}

