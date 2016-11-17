using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DBUtility;

namespace ImDal
{
	/// <summary>
	/// 数据访问类:ArticleSnaps
	/// </summary>
	public class ArticleSnaps
	{
		
		/// <summary>
		/// 是否存在该记录
		/// </summary>
		public bool Exists(int Id)
		{
			StringBuilder strSql=new StringBuilder();
			strSql.Append("select count(1) from ArticleSnaps");
			strSql.Append(" where Id=@Id ");
			SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int,4)			};
			parameters[0].Value = Id;

			return DbHelperSQL.Exists(strSql.ToString(),parameters);
		}


		/// <summary>
		/// 得到一个对象实体
		/// </summary>
		public ImModel.ArticleSnaps GetModel(long Id)
		{
			
			var strSql=new StringBuilder();
			strSql.Append("select top 1 ArticleId,FirstImageUrl,KeyWords,Description,ConTent,CreateTime from ArticleSnaps ");
            strSql.Append(" where ArticleId=@ArticleId ");
			SqlParameter[] parameters = {
					new SqlParameter("@ArticleId",SqlDbType.BigInt){Value = Id}	};

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
		public ImModel.ArticleSnaps DataRowToModel(DataRow row)
		{
			var model=new ImModel.ArticleSnaps();
			if (row != null)
			{
                if (row["ArticleId"] != null && row["ArticleId"].ToString() != "")
                {
                    model.ArticleId = long.Parse(row["ArticleId"].ToString());
                }
                if (row["FirstImageUrl"] != null)
                {
                    model.FirstImageUrl = row["FirstImageUrl"].ToString();
                }
                if (row["KeyWords"] != null)
                {
                    model.KeyWords = row["KeyWords"].ToString();
                }
                if (row["Description"] != null && row["Description"].ToString() != "")
                {
                    model.Description =row["Description"].ToString();
                }
                if (row["ConTent"] != null)
                {
                    model.Content = row["ConTent"].ToString();
                }
                if (row["Description"] != null)
                {
                    model.Description = row["Description"].ToString();
                }
                if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
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
			strSql.Append("select Id,ArticleId,text ");
			strSql.Append(" FROM ArticleSnaps ");
			if(strWhere.Trim()!="")
			{
				strSql.Append(" where "+strWhere);
			}
			return DbHelperSQL.Query(strSql.ToString());
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
			strSql.Append(")AS Row, T.*  from ArticleSnaps T ");
			if (!string.IsNullOrEmpty(strWhere.Trim()))
			{
				strSql.Append(" WHERE " + strWhere);
			}
			strSql.Append(" ) TT");
			strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
			return DbHelperSQL.Query(strSql.ToString());
		}

	}
}

