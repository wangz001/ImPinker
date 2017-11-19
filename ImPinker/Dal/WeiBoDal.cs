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
select t2.*,isnull(t3.VoteCount,0)as VoteCount,isnull(t4.CommentCount,0)as CommentCount from ( SELECT    [Id] ,
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
		left join (select count(*)as VoteCount,WeiBoId from WeiBoVote group by WeiBoId) T3 on t2.Id=t3.WeiBoId
		left join (select count(*)as CommentCount,WeiBoId from WeiBoComment group by WeiBoId) T4 on t2.Id=t4.WeiBoId

";
            var startIndex = (pageNum - 1) * pagesize + 1;
            var endIndex = pageNum * pagesize;
            SqlParameter[] parameters = {
					new SqlParameter("@startIndex", SqlDbType.Int){Value = startIndex},
                   
					new SqlParameter("@endIndex", SqlDbType.Int){Value = endIndex}};

            var  ds = DbHelperSQL.Query(sql, parameters);
            return ds;
        }
        /// <summary>
        /// 根据用户id获取列表
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pageNum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public DataSet GetListByPage(int userid,int pageNum, int pagesize)
        {
            const string sql = @"
select t2.*,isnull(t3.VoteCount,0)as VoteCount,isnull(t4.CommentCount,0)as CommentCount FROM (
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
WHERE   T.row BETWEEN @startIndex AND @endIndex )AS t2
 left join (select count(*)as VoteCount,WeiBoId from WeiBoVote group by WeiBoId) T3 on t2.Id=t3.WeiBoId
 left join (select count(*)as CommentCount,WeiBoId from WeiBoComment group by WeiBoId) T4 on t2.Id=t4.WeiBoId
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

        public bool Update(ImModel.ViewModel.WeiboVm model)
        {
            const string sqlStr = @"
UPDATE [dbo].[WeiBo]
   SET [UserId] = @UserId
      ,[Description] = @Description
      ,[ContentValue] = @ContentValue
      ,[ContentType] = @ContentType
      ,[Longitude] = @Longitude
      ,[Latitude] = @Latitude
      ,[Height] = @Height
      ,[LocationText] = @LocationText
      ,[State] = @State
      ,[HardWareType] = @HardWareType
      ,[IsRePost] = @IsRePost
      ,[CreateTime] = @CreateTime
      ,[UpdateTime] = @UpdateTime
 WHERE Id=@Id
";
            SqlParameter[] parameters =
			{
				new SqlParameter("@UserId", SqlDbType.Int, 100){Value =model.UserId },
				new SqlParameter("@Description", SqlDbType.VarChar, 200){Value =model.Description },
				new SqlParameter("@ContentValue", SqlDbType.VarChar, 100){Value =model.ContentValue },
				new SqlParameter("@ContentType", SqlDbType.Int, 4){Value =model.ContentType },
				new SqlParameter("@Longitude", SqlDbType.Decimal, 100){Value =model.Longitude },
				new SqlParameter("@Latitude", SqlDbType.Decimal, 200){Value =model.Lantitude },
				new SqlParameter("@Height", SqlDbType.Decimal, 1){Value =model.Height },
				new SqlParameter("@LocationText", SqlDbType.VarChar){Value =model.LocationText },
				new SqlParameter("@State", SqlDbType.TinyInt, 1){Value =model.State },
				new SqlParameter("@HardWareType", SqlDbType.VarChar){Value =model.HardWareType },
				new SqlParameter("@IsRePost", SqlDbType.TinyInt, 1){Value =model.IsRePost },
				new SqlParameter("@PublishTime", SqlDbType.DateTime){Value =model.PublishTime },
				new SqlParameter("@CreateTime", SqlDbType.DateTime){Value =model.CreateTime },
				new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value =model.UpdateTime },
				new SqlParameter("@Id", SqlDbType.BigInt, 8){Value =model.Id }
			};

            int rows = DbHelperSQL.ExecuteSql(sqlStr, parameters);
            return rows > 0;
        }
    }
}
