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

        public DataSet GetListByPage(int pageNum, int pagesize)
        {
            var sql = @"
SELECT  T2.* ,
        COUNT(WV.WeiBoId) AS VoteCount ,
        COUNT(WC.WeiBoId) AS CommentCount
FROM    ( SELECT    [Id] ,
                    [UserId] ,
                    [Description] ,
                    [ContentValue] ,
                    [ContentType] ,
                    [Longitude] ,
                    [Latitude] ,
                    [Height] ,
                    [LocationText] ,
                    [State] ,
                    [HardWareType] ,
                    [IsRePost] ,
                    [CreateTime] ,
                    [UpdateTime]
          FROM      ( SELECT    ROW_NUMBER() OVER ( ORDER BY Id DESC ) AS row ,
                                [WeiBo].*
                      FROM      [MyAutosTest].[dbo].[WeiBo]
                      WHERE     State = 1
                    ) T
          WHERE     T.row BETWEEN @startIndex AND @endIndex
        ) AS T2
        LEFT JOIN dbo.WeiBoVote WV ON T2.Id = WV.WeiBoId
        LEFT JOIN dbo.WeiBoComment WC ON T2.Id = WC.WeiBoId
GROUP BY T2.[Id] ,
        T2.[UserId] ,
        T2.[Description] ,
        T2.[ContentValue] ,
        T2.[ContentType] ,
        T2.[Longitude] ,
        T2.[Latitude] ,
        T2.[Height] ,
        T2.[LocationText] ,
        T2.[State] ,
        T2.[HardWareType] ,
        T2.[IsRePost] ,
        T2.[CreateTime] ,
        T2.[UpdateTime]
ORDER BY T2.Id DESC;
";
            var startIndex = (pageNum - 1) * pagesize + 1;
            var endIndex = pageNum * pagesize;
            SqlParameter[] parameters = {
					new SqlParameter("@startIndex", SqlDbType.Int){Value = startIndex},
                   
					new SqlParameter("@endIndex", SqlDbType.Int){Value = endIndex}};

            var  ds = DbHelperSQL.Query(sql, parameters);
            return ds;
        }

        public DataSet GetListByPage(int userid,int pageNum, int pagesize)
        {
            const string sql = @"
SELECT  *
FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY UpdateTime DESC ) AS row ,
                    [Id] ,
                    [UserId] ,
                    [Description] ,
                    [ContentValue] ,
                    [ContentType] ,
                    [Longitude] ,
                    [Latitude] ,
                    [Height] ,
                    [LocationText] ,
                    [State] ,
                    [HardWareType] ,
                    [IsRePost] ,
                    [CreateTime] ,
                    [UpdateTime]
          FROM      [MyAutosTest].[dbo].[WeiBo]
          WHERE     UserId=@UserId AND State = 1
        ) T
WHERE   T.row BETWEEN @startIndex AND @endIndex 
";
            var startIndex = (pageNum - 1) * pagesize + 1;
            var endIndex = pageNum * pagesize;
            SqlParameter[] parameters = {
					new SqlParameter("@UserId", SqlDbType.Int){Value = userid},
					new SqlParameter("@startIndex", SqlDbType.Int){Value = startIndex},
					new SqlParameter("@endIndex", SqlDbType.Int){Value = endIndex}};
            var ds = DbHelperSQL.Query(sql, parameters);
            return ds;
        }

        public DataSet GetById(long weiboid)
        {
            var sql = @"
SELECT              [Id] ,
                    [UserId] ,
                    [Description] ,
                    [ContentValue] ,
                    [ContentType] ,
                    [Longitude] ,
                    [Latitude] ,
                    [Height] ,
                    [LocationText] ,
                    [State] ,
                    [HardWareType] ,
                    [IsRePost] ,
                    [CreateTime] ,
                    [UpdateTime]
          FROM      [MyAutosTest].[dbo].[WeiBo]
          WHERE     Id = @Id
";
            SqlParameter[] parameters = {
					new SqlParameter("@Id", SqlDbType.Int){Value = weiboid}};

            var ds = DbHelperSQL.Query(sql, parameters);
            return ds;
        }
    }
}
