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
    }
}
