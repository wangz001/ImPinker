﻿using DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImDal
{
    public class ArticleCommentVoteDal
    {

        public bool AddVote(ImModel.ArticleCommentVote model)
        {
            var sql = @"
INSERT INTO [dbo].[ArticleCommentVote]
           ([ArticleCommentId]
           ,[UserId]
           ,[Vote]
           ,[CreateTime]
           ,[UpdateTime])
     VALUES
           (@ArticleCommentId
           ,@UserId
           ,@Vote
           ,@CreateTime
           ,@UpdateTime)
";
            
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleCommentId", System.Data.SqlDbType.BigInt,8){Value = model.ArticleCommentId},
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
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="articleCommentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsExist(long articleCommentId, int userId)
        {
            var sql = @"
select Id from ArticleCommentVote where ArticleCommentId=@ArticleCommentId and UserId=@UserId
";
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleCommentId", System.Data.SqlDbType.BigInt,8){Value =articleCommentId},
					new SqlParameter("@UserId", SqlDbType.Int){Value = userId}};

            var flag = DbHelperSQL.Exists(sql,parameters);
            return flag;
        }
    }
}
