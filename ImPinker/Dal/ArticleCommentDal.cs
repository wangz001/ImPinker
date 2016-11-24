using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using ImModel;

namespace ImDal
{
    public class ArticleCommentDal
    {
        public bool Add(ArticleComment model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ArticleComment(");
            strSql.Append("ArticleId,UserId,Content,ToUserId,CreateTime)");
            strSql.Append(" values (");
            strSql.Append("@ArticleId,@UserId,@Content,@ToUserId,@CreateTime)");
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleId", SqlDbType.NVarChar){Value = model.ArticleId},
					new SqlParameter("@UserId", SqlDbType.BigInt){Value = model.UserId},
					new SqlParameter("@Content", SqlDbType.NVarChar){Value = model.Content},
					model.ToUserId == 0
                    ? new SqlParameter("@ToUserId", SqlDbType.BigInt) {Value = DBNull.Value}
                    : new SqlParameter("@ToUserId", SqlDbType.BigInt) {Value = model.ToUserId},
					new SqlParameter("@CreateTime", SqlDbType.DateTime){Value = model.CreateTime}};

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
        /// 获取文章的评论详情
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="pageNum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<ArticleComment> GetListsByArticleId(string articleId, int pageNum, int count)
        {
            var list = new List<ArticleComment>();
            var sqlStr = @"
SELECT  *
FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY T.createtime DESC ) AS row ,
                    T.*
          FROM      ArticleComment T
          WHERE     T.articleid = @articleid
        ) TT
WHERE   TT.row BETWEEN @startIndex AND @endIndex;
";
            var startIndex = (pageNum - 1) * count + 1;
            var endIndex = pageNum * count;
            var paras = new SqlParameter[]
		    {
                new SqlParameter("@articleid",SqlDbType.Int){Value = articleId},
                new SqlParameter("@startIndex",SqlDbType.Int){Value = startIndex},
                new SqlParameter("@endIndex",SqlDbType.Int){Value = endIndex},
		    };
            var ds = DbHelperSQL.Query(sqlStr, paras);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                list = DsToList(ds);

            }
            return list;
        }

        private List<ArticleComment> DsToList(DataSet ds)
        {
            var list = new List<ArticleComment>();
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = new ArticleComment();

                    if (row["Id"] != null && row["Id"].ToString() != "")
                    {
                        model.Id = long.Parse(row["Id"].ToString());
                    }
                    if (row["ArticleId"] != null)
                    {
                        model.ArticleId = long.Parse(row["ArticleId"].ToString());
                    }
                    if (row["UserId"] != null)
                    {
                        model.UserId = int.Parse(row["UserId"].ToString());
                    }
                    if (row["Content"] != null)
                    {
                        model.Content = row["Content"].ToString();
                    }
                    if (row["ToUserId"] != null && row["ToUserId"].ToString() != "")
                    {
                        model.ToUserId = int.Parse(row["ToUserId"].ToString());
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
