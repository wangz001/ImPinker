using DBUtility;
using ImModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImModel.ViewModel;

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

        public List<WeiBoComment> GetList(int weiboid, int page, int pageSize)
        {
            var sql = @"
SELECT  *
FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY T.CreateTime DESC ) AS row ,
                    T.*
          FROM      dbo.WeiBoComment T
          WHERE     T.WeiBoId = @WeiBoId
        ) TT
WHERE   TT.row BETWEEN @start AND @end;
";
            var start = (page - 1) * pageSize + 1;
            var end = (page * pageSize);
            SqlParameter[] parameters = {
					new SqlParameter("@WeiBoId", SqlDbType.NVarChar){Value = weiboid},
					new SqlParameter("@start", SqlDbType.NVarChar){Value = start},
					new SqlParameter("@end", SqlDbType.NVarChar){Value = end}
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
        /// <summary>
        /// 根据id的集合获取评论列表
        /// </summary>
        /// <param name="commentIds"></param>
        /// <returns></returns>
        public List<WeiBoComment> GetList(List<long> commentIds)
        {
            var sql = @"
SELECT  T.*
FROM    dbo.WeiBoComment T
WHERE   T.Id IN ( {0} );
";
            if (commentIds.Any())
            {
                var str = string.Join(",", commentIds);
                sql = string.Format(sql, str);
                var ds = DbHelperSQL.Query(sql);
                return DtToList(ds.Tables[0]);
            }
            return null;
        }
    }
}
