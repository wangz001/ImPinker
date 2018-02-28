using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using DBUtility;

namespace ImDal
{
    /// <summary>
    /// 数据访问类:ArticleVote
    /// </summary>
    public partial class ArticleVoteDal
    {
        public ArticleVoteDal()
        { }
        #region  BasicMethod

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(long articleId, int userid)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from ArticleVote");
            strSql.Append(" where articleid=@articleid and userid=@userid ");
            SqlParameter[] parameters = {
					new SqlParameter("@articleid", SqlDbType.BigInt,8){Value = articleId},
					new SqlParameter("@userid", SqlDbType.Int,8){Value = userid}
                                        };
            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(ImModel.ArticleVote model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ArticleVote(");
            strSql.Append("ArticleId,UserId,Vote,CreateTime,UpdateTime)");
            strSql.Append(" values (");
            strSql.Append("@ArticleId,@UserId,@Vote,@CreateTime,@UpdateTime)");
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleId", SqlDbType.BigInt,8){Value = model.ArticleId},
					new SqlParameter("@UserId", SqlDbType.Int,4){Value = model.UserId},
					new SqlParameter("@Vote", SqlDbType.Bit,1){Value = model.Vote},
					new SqlParameter("@CreateTime", SqlDbType.DateTime){Value = model.CreateTime},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime}};
            
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
        public bool Update(ImModel.ArticleVote model)
        {
            var strSql = new StringBuilder();
            strSql.Append("update ArticleVote set ");
            strSql.Append("Vote=@Vote,");
            strSql.Append("UpdateTime=@UpdateTime");
            strSql.Append(" where ArticleId=@ArticleId and UserId=@UserId ");
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleId", SqlDbType.BigInt,8){Value = model.ArticleId},
					new SqlParameter("@UserId", SqlDbType.Int,4){Value = model.UserId},
					new SqlParameter("@Vote", SqlDbType.Bit,1){Value = model.Vote},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime}};
           
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

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ArticleVote ");
            strSql.Append(" where Id=@Id ");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt,8)			};
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from ArticleVote ");
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
        public ImModel.ArticleVote GetModel(long Id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 Id,ArticleId,UserId,Vote,CreateTime,UpdateTime from ArticleVote ");
            strSql.Append(" where Id=@Id ");
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.BigInt,8)			};
            parameters[0].Value = Id;

            ImModel.ArticleVote model = new ImModel.ArticleVote();
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
        public ImModel.ArticleVote DataRowToModel(DataRow row)
        {
            ImModel.ArticleVote model = new ImModel.ArticleVote();
            if (row != null)
            {
                if (row["Id"] != null && row["Id"].ToString() != "")
                {
                    model.Id = long.Parse(row["Id"].ToString());
                }
                if (row["ArticleId"] != null && row["ArticleId"].ToString() != "")
                {
                    model.ArticleId = long.Parse(row["ArticleId"].ToString());
                }
                if (row["UserId"] != null && row["UserId"].ToString() != "")
                {
                    model.UserId = int.Parse(row["UserId"].ToString());
                }
                if (row["Vote"] != null && row["Vote"].ToString() != "")
                {
                    if ((row["Vote"].ToString() == "1") || (row["Vote"].ToString().ToLower() == "true"))
                    {
                        model.Vote = true;
                    }
                    else
                    {
                        model.Vote = false;
                    }
                }
                if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                }
                if (row["UpdateTime"] != null && row["UpdateTime"].ToString() != "")
                {
                    model.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
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
            strSql.Append("select Id,ArticleId,UserId,Vote,CreateTime,UpdateTime ");
            strSql.Append(" FROM ArticleVote ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" Id,ArticleId,UserId,Vote,CreateTime,UpdateTime ");
            strSql.Append(" FROM ArticleVote ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM ArticleVote ");
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
        public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.Id desc");
            }
            strSql.Append(")AS Row, T.*  from ArticleVote T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperSQL.Query(strSql.ToString());
        }

        

        #endregion  BasicMethod
        #region  获取文章点赞数


        /// <summary>
        /// 获取文章点赞数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetArticleVoteCount(long id,bool isLike)
        {
            const string sqlStr = "SELECT COUNT(1) AS voteCount FROM dbo.ArticleVote WHERE ArticleId =@ArticleId AND Vote=@Vote";
            SqlParameter[] parameters = {
                    new SqlParameter("@ArticleId", SqlDbType.BigInt,8){Value = id},
                    new SqlParameter("@Vote", SqlDbType.Bit,1){Value = isLike}};

            int count = (int)DbHelperSQL.GetSingle(sqlStr.ToString(), parameters);
            return count;
        }


        #endregion  ExtensionMethod
    }
}

