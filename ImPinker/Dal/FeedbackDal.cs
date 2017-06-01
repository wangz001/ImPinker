
using System;
using System.Data;
using System.Data.SqlClient;
using DBUtility;
using ImModel;

namespace ImDal
{
    public class FeedbackDal
    {
        public bool AddFeedback(Feedback model)
        {
            var sql = @"
INSERT INTO [Feedback]
           ([UserIdentity]
           ,[ContactWay]
           ,[Description]
           ,[ContentStr]
           ,[State]
           ,[CreateTime]
           ,[UpdateTime])
     VALUES
           (@UserIdentity
           ,@ContactWay
           ,@DESCRIPTION
           ,@ContentStr
           ,@STATE
           ,@CreateTime
           ,@UpdateTime)
";
            SqlParameter[] parameters =
			{
				new SqlParameter("@UserIdentity", SqlDbType.VarChar){Value =model.UserIdentity },
				new SqlParameter("@ContactWay", SqlDbType.VarChar){Value =model.ContactWay },
				new SqlParameter("@DESCRIPTION", SqlDbType.VarChar){Value =model.Description },
				new SqlParameter("@ContentStr", SqlDbType.VarChar){Value = model.ContentStr},
				new SqlParameter("@STATE", SqlDbType.Int){Value =model.FeedBackState },
				new SqlParameter("@CreateTime", SqlDbType.DateTime){Value =DateTime.Now },
				new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = DateTime.Now}
			};
            int obj = DbHelperSQL.ExecuteSql(sql, parameters);
            return obj > 0;
        }
    }
}
