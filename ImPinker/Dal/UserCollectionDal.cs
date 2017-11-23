using DBUtility;
using ImModel;
using ImModel.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ImDal
{

    public class UserCollectionDal
    {
        public bool AddCollect(UserCollection model)
        {
            var sql = @"
INSERT INTO [dbo].[UserCollection]
           ([UserId]
            ,[EntityId]
            ,[EntityType]
           ,[State]
           ,[CreateTime]
           ,[UpdateTime])
     VALUES
           (@UserId
           ,@EntityId
           ,@EntityType
           ,@State
           ,@CreateTime
           ,@UpdateTime)
";
            SqlParameter[] parameters = {
					new SqlParameter("@UserId", SqlDbType.Int){Value = model.UserId},
					new SqlParameter("@EntityId", SqlDbType.Int){Value = model.EntityId},
					new SqlParameter("@EntityType", SqlDbType.Int){Value = (int)model.EntityType},
					new SqlParameter("@State", SqlDbType.TinyInt){Value = (int)model.State},
					new SqlParameter("@CreateTime", SqlDbType.DateTime){Value = model.CreateTime},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime}};
            var rows = DbHelperSQL.ExecuteSql(sql, parameters);
            return rows > 0;

        }

        /// <summary>
        /// 获取记录，根据用户id和文章id
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="userid"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public UserCollection GetModel(long entityId, int userid, EntityTypeEnum entityType)
        {
            var sql = @"
SELECT  [Id]
      ,[EntityId]
      ,[UserId]
      ,[State]
      ,[CreateTime]
      ,[UpdateTime]
      ,[EntityType]
  FROM [UserCollection] where UserId=@UserId and EntityId=@EntityId and EntityType=@EntityType
";
            SqlParameter[] parameters = {
					new SqlParameter("@UserId", SqlDbType.Int){Value = userid},
					new SqlParameter("@EntityId", SqlDbType.BigInt,8){Value = entityId},
					new SqlParameter("@EntityType", SqlDbType.Int){Value = (int)entityType}};

            var ds = DbHelperSQL.Query(sql, parameters);
            var list = DsToList(ds);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }
        /// <summary>
        /// ds 转换成List
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public List<UserCollection> DsToList(DataSet ds)
        {
            var list = new List<UserCollection>();
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = new UserCollection();
                    if (row["Id"] != null && row["Id"].ToString() != "")
                    {
                        model.Id = int.Parse(row["Id"].ToString());
                    }
                    if (row["EntityId"] != null)
                    {
                        model.EntityId = long.Parse(row["EntityId"].ToString());
                    }
                    if (row["EntityType"] != null)
                    {
                        model.EntityId = long.Parse(row["EntityType"].ToString());
                    }
                    if (row["UserId"] != null)
                    {
                        model.UserId = int.Parse(row["UserId"].ToString());
                    }
                    if (row["State"] != null)
                    {
                        model.State = (UserCollectionStateEnum)int.Parse(row["State"].ToString());
                    }
                    if (row["UpdateTime"] != null && row["UpdateTime"].ToString() != "")
                    {
                        model.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
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
        /// 修改记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateCollect(UserCollection model)
        {
            var sql = @"
UPDATE [dbo].[UserCollection]
   SET [EntityId] = @EntityId
      ,[EntityType] = @EntityType
      ,[State] = @State
      ,[UpdateTime] = @UpdateTime
 WHERE Id=@Id
";
            SqlParameter[] parameters = {
					new SqlParameter("@EntityId", SqlDbType.BigInt,8){Value = model.EntityId},
					new SqlParameter("@EntityType", SqlDbType.Int){Value = model.EntityType},
					new SqlParameter("@State", SqlDbType.Int){Value = model.State},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime},
					new SqlParameter("@Id", SqlDbType.Int){Value = model.Id}
                                        };
            int num = DbHelperSQL.ExecuteSql(sql, parameters);
            return num > 0;
        }

        /// <summary>
        /// 获取用户收藏的文章和微博
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageNum"></param>
        /// <param name="pagecount"></param>
        /// <returns></returns>
        public DataSet GetMyCollectsByPage(int userId, int pageNum, int pagecount)
        {
            const string sql = @"
SELECT  T2.* ,
        Art.Id AS AId ,
        Art.ArticleName ,
        Art.Url AS AUrl ,
        Art.CoverImage AS ACoverImage ,
        Art.UserId AS AUserId ,
        Art.KeyWords AS AKeyWords ,
        Art.Description AS ADescription ,
        Art.State AS AState ,
        Art.PublishTime AS APublishTime ,
        Art.CreateTime AS ACreateTime ,
        Art.UpdateTime AS AUpdateTime ,
        Art.ComPany AS ACompany ,
        Wei.Id AS WId ,
        Wei.UserId AS WUserid ,
        Wei.Description AS WDescription ,
        Wei.ContentValue AS WContentValue ,
        Wei.ContentType AS WContentType ,
        Wei.Longitude AS WLongitude ,
        Wei.Latitude AS WLatitude ,
        Wei.Height AS WHeight ,
        Wei.LocationText AS WLocationText ,
        Wei.State AS WState ,
        Wei.HardWareType AS WHardWareType ,
        Wei.IsRePost AS WIsRepost ,
        Wei.CreateTime AS WCreateTime ,
        Wei.UpdateTime AS WUpdateTime
FROM    ( SELECT    T.rownum ,
                    T.Id ,
                    T.EntityId ,
                    T.EntityType ,
                    T.UserId ,
                    T.CreateTime
          FROM      ( SELECT    ROW_NUMBER() OVER ( ORDER BY Id DESC ) AS rownum ,
                                *
                      FROM      dbo.UserCollection
                      WHERE     State = 1
                                AND UserId = @UserId
                    ) T
          WHERE     rownum BETWEEN @startIndex AND @endIndex
        ) T2
        LEFT JOIN dbo.Article Art ON T2.EntityId = Art.Id
                                     AND T2.EntityType = 1
        LEFT JOIN dbo.WeiBo Wei ON T2.EntityId = Wei.Id
                                   AND T2.EntityType = 2;
";

            var startIndex = (pageNum - 1) * pagecount + 1;
            var endIndex = pageNum * pagecount;
            var paras = new []
		    {
                new SqlParameter("@UserId",SqlDbType.Int){Value = userId},
                new SqlParameter("@startIndex",SqlDbType.Int){Value = startIndex},
                new SqlParameter("@endIndex",SqlDbType.Int){Value = endIndex}
		    };
            var ds = DbHelperSQL.Query(sql, paras);

            return ds;
        }
    }
}
