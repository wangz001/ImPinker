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

        public List<WeiBoComment> GetList(int weiboid)
        {
            var sql = @"
select * from WeiBoComment where WeiBoId=@WeiBoId order by CreateTime desc
";
            SqlParameter[] parameters = {
					new SqlParameter("@WeiBoId", SqlDbType.NVarChar){Value = weiboid},
					};
            var ds = DbHelperSQL.Query(sql, parameters);
            return DtToList(ds.Tables[0]);
        }

        public List<WeiBoComment> DtToList(DataTable dt)
        {
            var list = new List<WeiBoComment>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var model = new WeiBoComment();

                    if (row["Id"] != null && row["Id"].ToString() != "")
                    {
                        model.Id = long.Parse(row["Id"].ToString());
                    }
                    if (row["WeiBoId"] != null)
                    {
                        model.WeiBoId = long.Parse(row["WeiBoId"].ToString());
                    }
                    if (row["UserId"] != null)
                    {
                        model.UserId = int.Parse(row["UserId"].ToString());
                    }
                    if (row["ContentText"] != null)
                    {
                        model.ContentText = row["ContentText"].ToString();
                    }
                    if (row["ToCommentId"] != null && row["ToCommentId"].ToString() != "")
                    {
                        model.ToCommentId = int.Parse(row["ToCommentId"].ToString());
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
    }
}
