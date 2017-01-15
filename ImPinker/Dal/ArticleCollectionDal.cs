using DBUtility;
using ImModel;
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
select Id from ArticleCollection where ArticleId=@ArticleId and UserId=1
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
    }
}
