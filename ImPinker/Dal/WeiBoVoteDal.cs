using DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImDal
{
    public class WeiBoVoteDal
    {
        public bool Add(ImModel.WeiBoVote model)
        {
            var sql = @"
INSERT INTO [WeiBoVote]
           ([WeiBoId]
           ,[UserId]
           ,[Vote]
           ,[CreateTime]
           ,[UpdateTime])
     VALUES
           (@WeiBoId
           ,@UserId
           ,@Vote
           ,@CreateTime
           ,@UpdateTime)
";

            SqlParameter[] parameters = {
					new SqlParameter("@WeiBoId", System.Data.SqlDbType.BigInt,8){Value = model.WeiBoId},
					new SqlParameter("@UserId", SqlDbType.Int){Value = model.UserId},
					new SqlParameter("@Vote", SqlDbType.Bit,1){Value = model.Vote},
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

        public bool UpdtateStete(ImModel.WeiBoVote model)
        {
            var sql = @"
UPDATE [dbo].[WeiBoVote]
   SET [Vote] = @Vote
      ,[UpdateTime] = @UpdateTime
 WHERE WeiBoId=@WeiBoId and UserId=@UserId
";
            SqlParameter[] parameters = {
					new SqlParameter("@WeiBoId", System.Data.SqlDbType.BigInt,8){Value = model.WeiBoId},
					new SqlParameter("@UserId", SqlDbType.Int){Value = model.UserId},
					new SqlParameter("@Vote", SqlDbType.Int){Value = model.Vote},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime},
                                        };
            int num = DbHelperSQL.ExecuteSql(sql, parameters);
            return num > 0;
        }

        public bool IsExists(long weiboId, int userId)
        {
            var sql = @"
select Id from WeiBoVote where WeiBoId=@WeiBoId and UserId=@UserId
";
            SqlParameter[] parameters = {
					new SqlParameter("@WeiBoId", System.Data.SqlDbType.BigInt,8){Value =weiboId},
					new SqlParameter("@UserId", SqlDbType.Int){Value = userId}};

            var flag = DbHelperSQL.Exists(sql, parameters);
            return flag;
        }
    }
}
