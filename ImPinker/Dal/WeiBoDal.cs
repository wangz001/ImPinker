using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;

namespace ImDal
{
    public class WeiBoDal
    {
        /// <summary>
        /// 新建微博
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public long AddWeiBo(ImModel.WeiBo model)
        {
            var sql = @"
INSERT INTO [dbo].[WeiBo]
           ([UserId]
           ,[Description]
           ,[ContentValue]
           ,[ContentType]
           ,[Longitude]
           ,[Latitude]
           ,[Height]
           ,[LocationText]
           ,[State]
           ,[HardWareType]
           ,[IsRePost]
           ,[CreateTime]
           ,[UpdateTime])
     VALUES
           (@UserId
           ,@DESCRIPTION
           ,@ContentValue
           ,@ContentType
           ,@Longitude
           ,@Latitude
           ,@Height
           ,@LocationText
           ,@STATE
           ,@HardWareType
           ,@IsRePost
           ,@CreateTime
           ,@UpdateTime)
";
            SqlParameter[] parameters = {
					new SqlParameter("@ArticleId", System.Data.SqlDbType.BigInt,8){Value = model.UserId},
					new SqlParameter("@UserId", SqlDbType.Int){Value = model.UserId},
					new SqlParameter("@State", SqlDbType.TinyInt,1){Value = (int)model.State},
					new SqlParameter("@CreateTime", SqlDbType.DateTime){Value = model.CreateTime},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime}};

            int rows = DbHelperSQL.ExecuteSql(sql, parameters);
            if (rows > 0)
            {
                return 111;
            }
            else
            {
                return 0;
            }
        }
    }
}
