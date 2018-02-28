using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using ImModel;
using ImModel.ViewModel;

namespace ImDal
{
    public class ArticleCommentDal
    {
        public bool Add(ArticleComment model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into ArticleComment(");
            strSql.Append("ArticleId,UserId,Content,ToCommentId,CreateTime)");
            strSql.Append(" values (");
            strSql.Append("@ArticleId,@UserId,@Content,@ToCommentId,@CreateTime)");
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleId", SqlDbType.NVarChar){Value = model.ArticleId},
					new SqlParameter("@UserId", SqlDbType.BigInt){Value = model.UserId},
					new SqlParameter("@Content", SqlDbType.NVarChar){Value = model.Content},
					model.ToCommentId == 0
                    ? new SqlParameter("@ToCommentId", SqlDbType.BigInt) {Value = DBNull.Value}
                    : new SqlParameter("@ToCommentId", SqlDbType.BigInt) {Value = model.ToCommentId},
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
        /// 获取文章的评论详情,返回两个datatable.第二个为引用的评论
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="pageNum"></param>
        /// <param name="count"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public DataSet GetListsByArticleId(long articleId, int pageNum, int count,out int totalCount)
        {
            
            var sqlStr = @"
SELECT  tt.Id,tt.ArticleId,tt.UserId,tt.ToCommentId,tt.Content,tt.CreateTime,count(acv.articlecommentid) as CommentVoteCount
 FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY T.createtime DESC ) AS row ,
                    T.*
          FROM      ArticleComment T
          WHERE     T.articleid = @articleid
        ) TT left join ArticleCommentVote acv on tt.id=acv.articlecommentid 
 WHERE   TT.row BETWEEN @startIndex AND @endIndex
 group by tt.Id,tt.ArticleId,tt.UserId,tt.ToCommentId,tt.Content,tt.CreateTime
";

            var sqlStr2 = new StringBuilder();
            sqlStr2.Append("SELECT  * FROM    ArticleComment WHERE   id IN ( SELECT  temp.tocommentid  FROM    ( ");
            sqlStr2.Append(sqlStr);
            sqlStr2.Append(" ) temp )");
            var sqlStr3 = "SELECT    COUNT(id)   FROM  ArticleComment T   WHERE     T.articleid = @articleid";
            var sql = sqlStr + sqlStr2+sqlStr3;
            var startIndex = (pageNum - 1) * count + 1;
            var endIndex = pageNum * count;
            var paras = new SqlParameter[]
		    {
                new SqlParameter("@articleid",SqlDbType.Int){Value = articleId},
                new SqlParameter("@startIndex",SqlDbType.Int){Value = startIndex},
                new SqlParameter("@endIndex",SqlDbType.Int){Value = endIndex},
		    };
            //返回两个datatable
            var ds = DbHelperSQL.Query(sql, paras);
            var table3 = ds.Tables[2];
            totalCount = Int32.Parse(table3.Rows[0][0].ToString());
            return ds;
        }

        public List<ArticleComment> DtToList(DataTable dt)
        {
            var list = new List<ArticleComment>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
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
        /// 根据id，获取列表
        /// </summary>
        /// <param name="toCommentIds"></param>
        /// <returns></returns>
        public List<ArticleComment> GetListsByIds(List<int> toCommentIds)
        {
            var list = new List<ArticleComment>();
            //var sqlStr = @"exec('SELECT * FROM ArticleComment WHERE id IN ('+@toCommentIds+')') ";      
            //exec方式，跟直接拼sql类似，无法形成查询计划
            var sql = new StringBuilder();
            var paras = new SqlParameter[toCommentIds.Count];
            sql.Append("SELECT * FROM ArticleComment WHERE id IN (");
            var i = 0;
            foreach (var commentId in toCommentIds)
            {
                sql.Append(i == 0 ? "@commentId" + i : ",@commentId" + i);
                paras[i] = new SqlParameter("@commentId" + i, SqlDbType.Int) { Value = commentId };
                i++;
            }
            sql.Append(")");

            var ds = DbHelperSQL.Query(sql.ToString(), paras);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                list = DtToList(ds.Tables[0]);
            }
            return list;
        }

        
        /// <summary>
        /// 获取文章评论数
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetArticleCommentCount(long id)
        {
            const string sqlStr = " SELECT COUNT(1) AS CommentCount FROM dbo.ArticleComment WHERE ArticleId =@ArticleId";
            SqlParameter[] parameters = {
                    new SqlParameter("@ArticleId", SqlDbType.BigInt,8){Value = id}};

            int count = (int)DbHelperSQL.GetSingle(sqlStr.ToString(), parameters);
            return count;
        }


    }
}
