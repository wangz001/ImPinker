using DBUtility;
using ImModel;
using ImModel.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImDal
{
    
    public class ArticleCollectionDal
    {
        public bool IsExist(long articleId, int userid)
        {
            var sql = @"
select Id from ArticleCollection where ArticleId=@ArticleId and UserId=@UserId
";
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleId", System.Data.SqlDbType.BigInt,8){Value =articleId},
					new SqlParameter("@UserId", SqlDbType.Int){Value = userid}};

            var flag = DbHelperSQL.Exists(sql, parameters);
            return flag;
        }

        public bool AddCollect(ArticleCollection model)
        {
            var sql = @"
INSERT INTO [dbo].[ArticleCollection]
           ([ArticleId]
           ,[UserId]
           ,[State]
           ,[CreateTime]
           ,[UpdateTime])
     VALUES
           (@ArticleId
           ,@UserId
           ,@State
           ,@CreateTime
           ,@UpdateTime)
";
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleId", System.Data.SqlDbType.BigInt,8){Value = model.ArticleId},
					new SqlParameter("@UserId", SqlDbType.Int){Value = model.UserId},
					new SqlParameter("@State", SqlDbType.TinyInt,1){Value = (int)model.State},
					new SqlParameter("@CreateTime", SqlDbType.DateTime){Value = model.CreateTime},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime}};

            int rows = DbHelperSQL.ExecuteSql(sql, parameters);
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
        /// 获取记录，根据用户id和文章id
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ArticleCollection GetModel(long articleId, int userid)
        {
            var sql = @"
SELECT [Id]
      ,[ArticleId]
      ,[UserId]
      ,[State]
      ,[CreateTime]
      ,[UpdateTime]
  FROM [dbo].[ArticleCollection] where UserId=@UserId and ArticleId=@ArticleId
";
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleId", System.Data.SqlDbType.BigInt,8){Value = articleId},
					new SqlParameter("@UserId", SqlDbType.Int){Value = userid}};

            var ds = DbHelperSQL.Query(sql, parameters);
            var list = DsToList(ds);
            if (list!=null&&list.Count > 0)
            {
                return list[0];
            }
            return null;
        }
        /// <summary>
        /// ds 转换成List<ArticleCollection>
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public List<ArticleCollection> DsToList(DataSet ds)
        {
            var list = new List<ArticleCollection>();
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = new ArticleCollection();
                    if (row["Id"] != null && row["Id"].ToString() != "")
                    {
                        model.Id = int.Parse(row["Id"].ToString());
                    }
                    if (row["ArticleId"] != null)
                    {
                        model.ArticleId = long.Parse(row["ArticleId"].ToString());
                    }
                    if (row["UserId"] != null)
                    {
                        model.UserId = int.Parse(row["UserId"].ToString());
                    }
                    if (row["State"] != null)
                    {
                        model.State = (ArticleCollectionStateEnum)int.Parse(row["State"].ToString());
                    }
                    if (row["UpdateTime"] != null && row["UpdateTime"].ToString() != "")
                    {
                        model.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
                    }
                    if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                    {
                        model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                    }
                    list.Add(model);
                }
            }

            return list;
        }

        /// <summary>
        /// 修改记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateCollect(ArticleCollection model)
        {
            var sql = @"
UPDATE [dbo].[ArticleCollection]
   SET [ArticleId] = @ArticleId
      ,[UserId] = @UserId
      ,[State] = @State
      ,[UpdateTime] = @UpdateTime
 WHERE Id=@Id
";
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleId", System.Data.SqlDbType.BigInt,8){Value = model.ArticleId},
					new SqlParameter("@UserId", SqlDbType.Int){Value = model.UserId},
					new SqlParameter("@State", SqlDbType.Int){Value = model.State},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime},
					new SqlParameter("@Id", SqlDbType.Int){Value = model.Id},
                                        };
            int num = DbHelperSQL.ExecuteSql(sql, parameters);
            return num > 0;
        }


        public DataTable GetMyCollectsByPage(int userId, int pageNum, int pagecount, out int totalCount)
        {
            totalCount = 0;
            var sql = @"
  select * from (
	select ROW_NUMBER() over(
		order by AC.CreateTime desc) as row,
		A.Id,A.ArticleName,A.Url,A.CoverImage,A.CreateTime   
		from Article A join ArticleCollection AC on A.Id=AC.ArticleId
	where AC.UserId=@UserId and AC.State=1
  ) TT WHERE TT.Row between @startIndex and @endIndex ;

  select COUNT(0) from ArticleCollection where UserId=@UserId and state=1 ;
";

            var startIndex = (pageNum - 1) * pagecount + 1;
            var endIndex = pageNum * pagecount;
            var paras = new SqlParameter[]
		    {
                new SqlParameter("@UserId",SqlDbType.Int){Value = userId},
                new SqlParameter("@startIndex",SqlDbType.Int){Value = startIndex},
                new SqlParameter("@endIndex",SqlDbType.Int){Value = endIndex},
		    };
            var ds = DbHelperSQL.Query(sql.ToString(), paras);
            if (ds != null && ds.Tables.Count > 1)
            {
                int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out totalCount);
                return ds.Tables[0];
            }
            return null;
        }
    }
}
