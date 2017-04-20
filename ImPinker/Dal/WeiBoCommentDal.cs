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
    public class WeiBoCommentDal
    {
        public bool AddComment(ImModel.WeiBoComment model)
        {
            var sqlStr = @"
INSERT INTO [dbo].[WeiBoComment]
           ([WeiBoId]
           ,[UserId]
           ,[ContentText]
           ,[ToCommentId]
           ,[State]
           ,[CreateTime])
VALUES
           (@WeiBoId
           ,@UserId
           ,@ContentText
           ,@ToCommentId
           ,@State
           ,@CreateTime)";
            SqlParameter[] parameters = {
					new SqlParameter("@WeiBoId", SqlDbType.NVarChar){Value = model.WeiBoId},
					new SqlParameter("@UserId", SqlDbType.BigInt){Value = model.UserId},
					new SqlParameter("@ContentText", SqlDbType.NVarChar){Value = model.ContentText},
					new SqlParameter("@State", SqlDbType.NVarChar){Value = model.State},
					model.ToCommentId == 0
                    ? new SqlParameter("@ToCommentId", SqlDbType.BigInt) {Value = DBNull.Value}
                    : new SqlParameter("@ToCommentId", SqlDbType.BigInt) {Value = model.ToCommentId},
					new SqlParameter("@CreateTime", SqlDbType.DateTime){Value = model.CreateTime}};

            int rows = DbHelperSQL.ExecuteSql(sqlStr, parameters);
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
