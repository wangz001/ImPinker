using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using DBUtility;
using ImModel;
using ImModel.ViewModel;

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
        /// 增加一条数据,返回自增id
        /// </summary>
        public int Add(Article model)
        {
            var strSql = new StringBuilder();
            strSql.Append("insert into Article(");
            strSql.Append("ArticleName,Url,CoverImage,UserId,KeyWords,Description,State,CreateTime,UpdateTime,PublishTime,Company)");
            strSql.Append(" values (");
            strSql.Append("@ArticleName,@Url,@CoverImage,@UserId,@KeyWords,@Description,@State,@CreateTime,@UpdateTime,@PublishTime,@Company)");
            strSql.Append(" select IDENT_CURRENT('Article')");
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
            object  obj = DbHelperSQL.ExecuteScalar(strSql.ToString(), parameters);
            return Convert.ToInt32(obj);
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
            strSql.Append("PublishTime=@PublishTime,");
            strSql.Append("UpdateTime=@UpdateTime");
            strSql.Append(" where Id=@Id ");
            SqlParameter[] parameters =
			{
				new SqlParameter("@ArticleName", SqlDbType.NVarChar, 100){Value =model.ArticleName },
				new SqlParameter("@Url", SqlDbType.VarChar, 200){Value =model.Url },
				new SqlParameter("@CoverImage", SqlDbType.VarChar, 100){Value =model.CoverImage },
				new SqlParameter("@UserId", SqlDbType.Int, 4){Value =model.UserId },
				new SqlParameter("@KeyWords", SqlDbType.NVarChar, 100){Value =model.KeyWords },
				new SqlParameter("@Description", SqlDbType.NVarChar, 200){Value =model.Description },
				new SqlParameter("@State", SqlDbType.TinyInt, 1){Value =model.State },
				new SqlParameter("@PublishTime", SqlDbType.DateTime){Value =model.PublishTime },
				new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value =model.UpdateTime },
				new SqlParameter("@Id", SqlDbType.BigInt, 8){Value =model.Id }
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
        /// 删除一条数据
        /// </summary>
        public bool DeleteThread(int userid,long Id)
        {
            var strSql = new StringBuilder();
            strSql.Append("update Article ");
            strSql.Append(" set State=0 where Id=@Id and UserId=@UserId ");
            SqlParameter[] parameters =
			{
				new SqlParameter("@UserId", SqlDbType.Int){Value = userid},
				new SqlParameter("@Id", SqlDbType.BigInt){Value = Id}
			};

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Article GetModel(long Id)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append(
                "select top 1 Id,ArticleName,Url,CoverImage,UserId,KeyWords,Description,Company,State,CreateTime,UpdateTime,PublishTime from Article ");
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
                if (row.Table.Columns.Contains("Id") && row["Id"] != null && row["Id"].ToString() != "")
                {
                    model.Id = long.Parse(row["Id"].ToString());
                }
                if (row.Table.Columns.Contains("ArticleName") && row["ArticleName"] != null)
                {
                    model.ArticleName = row["ArticleName"].ToString();
                }
                if (row.Table.Columns.Contains("Url") && row["Url"] != null)
                {
                    model.Url = row["Url"].ToString();
                }
                if (row.Table.Columns.Contains("CoverImage") && row["CoverImage"] != null)
                {
                    model.CoverImage = row["CoverImage"].ToString();
                }
                if (row.Table.Columns.Contains("UserId") && row["UserId"] != null && row["UserId"].ToString() != "")
                {
                    model.UserId = int.Parse(row["UserId"].ToString());
                }
                if (row.Table.Columns.Contains("KeyWords") && row["KeyWords"] != null)
                {
                    model.KeyWords = row["KeyWords"].ToString();
                }
                if (row.Table.Columns.Contains("Description") && row["Description"] != null)
                {
                    model.Description = row["Description"].ToString();
                }
                if (row.Table.Columns.Contains("State") && row["State"] != null && row["State"].ToString() != "")
                {
                    model.State = int.Parse(row["State"].ToString());
                }
                if (row.Table.Columns.Contains("CreateTime") && row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                }
                if (row.Table.Columns.Contains("UpdateTime") && row["UpdateTime"] != null && row["UpdateTime"].ToString() != "")
                {
                    model.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
                }
                if (row.Table.Columns.Contains("PublishTime") && row["PublishTime"] != null && row["PublishTime"].ToString() != "")
                {
                    model.PublishTime = DateTime.Parse(row["PublishTime"].ToString());
                }
                if (row.Table.Columns.Contains("Company") && row["Company"] != null && row["Company"].ToString() != "")
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
            strSql.Append("select Id,ArticleName,Url,CoverImage,UserId,KeyWords,Description,State,Company,CreateTime,UpdateTime,PublishTime ");
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
            strSql.Append(" WHERE UserId=@UserId and state<>0");
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between @startIndex and @endIndex ;");
            strSql.Append(" select count(1) from Article T WHERE UserId=@UserId and state<>0");
            var startIndex = (pageNum - 1) * count + 1;
            var endIndex = pageNum * count;
            var paras = new SqlParameter[]
		    {
                new SqlParameter("@UserId",SqlDbType.Int){Value = userid},
                new SqlParameter("@startIndex",SqlDbType.Int){Value = startIndex},
                new SqlParameter("@endIndex",SqlDbType.Int){Value = endIndex},
		    };
            return DbHelperSQL.Query(strSql.ToString(), paras);
        }
        /// <summary>
        /// 根据状态获取文章列表
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pageNum"></param>
        /// <param name="count"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public DataSet GetMyListByState(int userid, int pageNum, int count,ArticleStateEnum state)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            strSql.Append("order by T.CreateTime desc");
            strSql.Append(")AS Row, T.*  from Article T ");
            strSql.Append(" WHERE UserId=@UserId and state=@state");
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between @startIndex and @endIndex ;");
            strSql.Append(" select count(1) from Article T WHERE UserId=@UserId and state=@state");
            var startIndex = (pageNum - 1) * count + 1;
            var endIndex = pageNum * count;
            var paras = new[]
		    {
                new SqlParameter("@UserId",SqlDbType.Int){Value = userid},
                new SqlParameter("@startIndex",SqlDbType.Int){Value = startIndex},
                new SqlParameter("@endIndex",SqlDbType.Int){Value = endIndex},
                new SqlParameter("@state",SqlDbType.Int){Value = (int)state},
		    };
            return DbHelperSQL.Query(strSql.ToString(), paras);
        }

        /// <summary>
        /// 获取首页的文章
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public DataSet GetIndexListByPage(int pageNum, int count)
        {
            const string strSql = @"
SELECT  TT.Id,TT.ArticleName,TT.Url,TT.CoverImage,TT.UserId,TT.ComPany,TT.KeyWords,TT.CreateTime,COUNT(av.ArticleId) as voteCount 
 FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY T.CreateTime DESC ) AS Row ,
                    T.*
          FROM      Article T WHERE T.STATE=1 AND  T.CoverImage IS NOT NULL AND DATALENGTH(T.CoverImage)>0
        ) TT left join ArticleVote AV on TT.Id=AV.ArticleId 
WHERE   TT.Row BETWEEN @startIndex AND @endIndex 
 group by TT.Id,TT.ArticleName,TT.Url,TT.CoverImage,TT.UserId,TT.ComPany,TT.KeyWords,TT.CreateTime;
";
            var startIndex = (pageNum - 1) * count + 1;
            var endIndex = pageNum * count;
            var paras = new SqlParameter[]
		    {
                new SqlParameter("@startIndex",SqlDbType.Int){Value = startIndex},
                new SqlParameter("@endIndex",SqlDbType.Int){Value = endIndex},
		    };
            return DbHelperSQL.Query(strSql, paras);
        }

        /// <summary>
        /// 发布新帖子。操作article和articlesnap表，使用事务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddThread(CreateThreadVm model)
        {
            var sql1 = new StringBuilder();
            sql1.Append("insert into Article(");
            sql1.Append("ArticleName,Url,CoverImage,UserId,KeyWords,Description,State,PublishTime,CreateTime,UpdateTime)");
            sql1.Append(" values (");
            sql1.Append("@ArticleName,@Url,@CoverImage,@UserId,@KeyWords,@Description,@State,@PublishTime,@CreateTime,@UpdateTime)");
            SqlParameter[] parameters1 =
			{
				new SqlParameter("@ArticleName", SqlDbType.NVarChar, 100){Value =model.ArticleName },
				new SqlParameter("@CoverImage", SqlDbType.VarChar, 100){Value =model.Coverimage },
				new SqlParameter("@Url", SqlDbType.VarChar){Value ="" },
				new SqlParameter("@UserId", SqlDbType.Int, 4){Value = model.Userid},
				new SqlParameter("@KeyWords", SqlDbType.NVarChar, 100){Value =model.Keywords },
				new SqlParameter("@Description", SqlDbType.NVarChar, 200){Value =model.Description },
				new SqlParameter("@ConTent", SqlDbType.NVarChar){Value =model.Content },
				new SqlParameter("@State", SqlDbType.TinyInt, 1){Value =model.State },
				new SqlParameter("@PublishTime", SqlDbType.DateTime){Value =model.Createtime },
				new SqlParameter("@CreateTime", SqlDbType.DateTime){Value =model.Createtime },
				new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.Updatetime},
			};

            const string sql2 = @"
INSERT INTO [dbo].[ArticleSnaps]
           ([ArticleId]
           ,[ConTent]
           ,[CreateTime])
     VALUES
           ((SELECT IDENT_CURRENT('Article')),@ConTent,@CreateTime)
";
            SqlParameter[] parameters2 =
			{
				new SqlParameter("@ConTent", SqlDbType.NVarChar){Value =model.Content },
				new SqlParameter("@CreateTime", SqlDbType.DateTime){Value =model.Createtime },
			};

            var strlist = new List<CommandInfo>
            {
                new CommandInfo()
                {
                    CommandText = sql1.ToString(),
                    Parameters = parameters1
                },
                new CommandInfo()
                {
                    CommandText = sql2,
                    Parameters = parameters2
                }
            };
            int rows = DbHelperSQL.ExecuteSqlTran(strlist);
            return rows > 0;
        }
        /// <summary>
        /// 修改帖子
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        public bool UpdateThread(CreateThreadVm vm)
        {
            var sql1 = new StringBuilder();
            sql1.Append("UPDATE Article set ");
            sql1.Append("ArticleName=@ArticleName,CoverImage=@CoverImage,KeyWords=@KeyWords,Description=@Description,UpdateTime=@UpdateTime ");
            sql1.Append(" WHERE ");
            sql1.Append("UserId=@UserId and Id=@Id ");
            SqlParameter[] parameters1 =
			{
                new SqlParameter("@Id", SqlDbType.BigInt){Value =vm.ArticleId },
				new SqlParameter("@ArticleName", SqlDbType.NVarChar, 100){Value =vm.ArticleName },
				new SqlParameter("@CoverImage", SqlDbType.VarChar, 100){Value =vm.Coverimage },
				new SqlParameter("@UserId", SqlDbType.Int, 4){Value = vm.Userid},
				new SqlParameter("@KeyWords", SqlDbType.NVarChar, 100){Value =vm.Keywords },
				new SqlParameter("@Description", SqlDbType.NVarChar, 200){Value =vm.Description },
				new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = vm.Updatetime},
			};

            const string sql2 = @"
UPDATE [dbo].[ArticleSnaps]
           SET [ConTent]=@ConTent
     where ArticleId=@ArticleId
";
            SqlParameter[] parameters2 =
			{
				new SqlParameter("@ArticleId", SqlDbType.BigInt){Value =vm.ArticleId },
				new SqlParameter("@ConTent", SqlDbType.Text){Value =vm.Content },
			};

            var strlist = new List<CommandInfo>
            {
                new CommandInfo()
                {
                    CommandText = sql1.ToString(),
                    Parameters = parameters1
                },
                new CommandInfo()
                {
                    CommandText = sql2,
                    Parameters = parameters2
                }
            };
            int rows = DbHelperSQL.ExecuteSqlTran(strlist);
            return rows > 0;
        }
    }
}

