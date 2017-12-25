using System;
using System.Data;
using System.Data.SqlClient;
using DBUtility;
using ImModel;

namespace ImDal
{
    public class UserTokenDal
    {
        public bool Add(UserToken model)
        {
            const string sql = @"
INSERT INTO [UserToken]
           ([UserId]
           ,[Token]
           ,[CreateTime]
           ,[UpdateTime])
     VALUES
           (@UserId
           ,@Token
           ,@CreateTime
           ,@UpdateTime)
";
            SqlParameter[] parameters = {
					new SqlParameter("@UserId", SqlDbType.BigInt,8){Value = model.UserId},
					new SqlParameter("@Token", SqlDbType.VarChar){Value = model.Token},
					new SqlParameter("@CreateTime", SqlDbType.DateTime){Value = model.CreateTime},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime}};

            int rows = DbHelperSQL.ExecuteSql(sql, parameters);
            return rows > 0;
        }

        public UserToken GetByUserId(int userId)
        {
            const string sqlStr = @"
SELECT [Id]
      ,[UserId]
      ,[Token]
      ,[CreateTime]
      ,[UpdateTime]
  FROM [UserToken] where UserId=@UserId
";
            SqlParameter[] parameters = {
					new SqlParameter("@UserId", SqlDbType.BigInt,8){Value = userId}};
            var ds = DbHelperSQL.Query(sqlStr, parameters);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                var model = new UserToken
                {
                    Id = Int32.Parse(row["Id"].ToString()),
                    UserId = Int32.Parse(row["UserId"].ToString()),
                    Token = row["Token"].ToString(),
                    CreateTime = DateTime.Parse(row["CreateTime"].ToString()),
                    UpdateTime = DateTime.Parse(row["UpdateTime"].ToString())
                };
                return model;
            }
            return null;
        }

        public bool Update(UserToken userToken)
        {
            const string sqlStr = @"
UPDATE [UserToken]
   SET [Token] = @Token
      ,[UpdateTime] = @UpdateTime
 WHERE UserId=@UserId
";
            SqlParameter[] parameters = {
					new SqlParameter("@UserId", SqlDbType.BigInt,8){Value = userToken.UserId},
					new SqlParameter("@Token", SqlDbType.VarChar){Value = userToken.Token},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = DateTime.Now}
                                        };
            var flag = DbHelperSQL.ExecuteSql(sqlStr, parameters);
            return flag > 0;
        }
    }
}
