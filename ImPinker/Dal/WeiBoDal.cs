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
        public bool AddWeiBo(ImModel.WeiBo model)
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
					new SqlParameter("@UserId", SqlDbType.Int){Value = model.UserId},
                    new SqlParameter("@DESCRIPTION",SqlDbType.VarChar){Value = model.Description}, 
                    new SqlParameter("@ContentValue",SqlDbType.VarChar){Value = model.ContentValue}, 
                    new SqlParameter("@ContentType",SqlDbType.TinyInt){Value =(int) model.ContentType}, 
                    new SqlParameter("@Longitude",SqlDbType.Decimal){Value = model.Longitude}, 
                    new SqlParameter("@Latitude",SqlDbType.Decimal){Value = model.Lantitude}, 
                    new SqlParameter("@Height",SqlDbType.Decimal){Value = model.Height}, 
                    new SqlParameter("@LocationText",SqlDbType.VarChar){Value = model.LocationText}, 
					new SqlParameter("@IsRePost", SqlDbType.Bit){Value = model.IsRePost},
					new SqlParameter("@State", SqlDbType.TinyInt){Value = (int)model.State},
                    new SqlParameter("@HardWareType",SqlDbType.VarChar){Value = model.HardWareType}, 
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
    }
}
