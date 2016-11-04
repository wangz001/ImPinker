using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AhModel;

namespace AhDal
{
    public class AHEntityMapDal
    {
        public List<AhEntityMap> GetEntityMaps(string strWhere)
        {
            var lists = new List<AhEntityMap>();
            var sqlStr = new StringBuilder();
            sqlStr.Append(" SELECT  [ID],[BitID],[AhID],[ModelType],[CreateTime],[UpdateTime] ");
            sqlStr.Append(" FROM [CarsDataAutoHome].[dbo].[AHEntityMap] ");
            if (strWhere.Trim() != "")
            {
                sqlStr.Append(" where " + strWhere);
            }
            var ds = DbHelperSql.Query(sqlStr.ToString());
            if (ds != null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var map = new AhEntityMap();
                    map.ID = Int32.Parse(row["ID"].ToString());
                    map.BitID = Int32.Parse(row["BitID"].ToString());
                    map.AhID = Int32.Parse(row["AhID"].ToString());
                    map.ModelType = Int32.Parse(row["ModelType"].ToString());
                    map.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                    map.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
                    lists.Add(map);
                }
            }
            return lists;
        }

        public bool IsExist(int masterBrandId, int modelType)
        {
            const string sqlStr = @"
SELECT COUNT(1) FROM [CarsDataAutoHome].[dbo].[AHEntityMap] 
 WHERE BitID=@BitID AND ModelType=@ModelType 
";
            SqlParameter[] parameters = {
					new SqlParameter("@BitID", SqlDbType.Int,4){Value = masterBrandId},
                    new SqlParameter("@ModelType",SqlDbType.Int){Value = modelType}
                                        };
            return DbHelperSql.Exists(sqlStr, parameters);
        }

        public Boolean Insert(int masterBrandId, int ahMasterBrandId, int modelType)
        {
            const string sqlStr = @"
INSERT INTO [CarsDataAutoHome].[dbo].[AHEntityMap]  
 ([BitID],[AhID],[ModelType],[CreateTime],[UpdateTime]) 
 VALUES 
 (@BitID,@AhID,@ModelType,@CreateTime,@UpdateTime) 
";
            SqlParameter[] parameters = {
					new SqlParameter("@BitID", SqlDbType.Int,4){Value = masterBrandId},
					new SqlParameter("@AhID", SqlDbType.Int){Value = ahMasterBrandId},
					new SqlParameter("@ModelType", SqlDbType.Int){Value = modelType},
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value =DateTime.Now}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value =DateTime.Now} 
                                        };

            int rows = DbHelperSql.ExecuteSql(sqlStr, parameters);

            return rows > 0;
        }

        public Boolean Update(int masterBrandId, int ahMasterBrandId, int modelType)
        {
            const string sqlStr = @"
UPDATE [CarsDataAutoHome].[dbo].[AHEntityMap] 
  SET [AhID] = @AhID  
 ,[UpdateTime] = @UpdateTime 
 WHERE BitID=@BitID AND ModelType=@ModelType 
";
            SqlParameter[] parameters = {
					new SqlParameter("@AhID", SqlDbType.Int){Value =ahMasterBrandId},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value =DateTime.Now},
                    new SqlParameter("@BitID",SqlDbType.Int){Value = masterBrandId}, 
					new SqlParameter("@ModelType", SqlDbType.Int){Value = modelType}};
            int rows = DbHelperSql.ExecuteSql(sqlStr, parameters);
            return rows > 0;
        }
        /// <summary>
        /// 删除多个品牌的对应关系
        /// </summary>
        /// <param name="type">实体枚举类型</param>
        /// <param name="bitIds">实体ID集合</param>
        public bool Delete(int type, int[] bitIds)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("DELETE FROM [CarsDataAutoHome].[dbo].[AHEntityMap] ");
            sqlStr.Append(" WHERE  ModelType= " + type);
            if (bitIds.Length > 0)
            {
                var ids = string.Join(",", bitIds);
                sqlStr.Append(" AND BitID IN (" + ids + ") ");
            }
            int rows = DbHelperSql.ExecuteSql(sqlStr.ToString());
            return rows > 0;
        }

    }
}
