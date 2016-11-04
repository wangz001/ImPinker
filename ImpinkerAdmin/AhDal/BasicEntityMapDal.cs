using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AhModel;

namespace AhDal
{
    public class BasicEntityMapDal
    {
        public List<BasicEntityMap> GetEntityMaps(int bitStyleId, int entityType)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(" SELECT [Id],[EntityType],[CompanyId],[BitEntityId],[CompareEntityId] ");
            sqlStr.Append(" ,[IsPeopleSet],[CreateTime] ,[UpdateTime] ");
            sqlStr.Append(" FROM [BasicEntityMap] ");
            sqlStr.Append(" WHERE BitEntityId=@BitEntityId AND EntityType=@EntityType ");

            SqlParameter[] parameters =
            {
                new SqlParameter("@BitEntityId",SqlDbType.Int){Value = bitStyleId},
                new SqlParameter("@EntityType",SqlDbType.Int){Value = entityType} 
            };
            var ds = DbHelperSql.Query(sqlStr.ToString(), parameters);
            
            return DsToList(ds);
        }

        public List<BasicEntityMap> GetEntityMaps(int entityType, string bitEntityIds)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(" SELECT [Id],[EntityType],[CompanyId],[BitEntityId],[CompareEntityId] ");
            sqlStr.Append(" ,[IsPeopleSet],[CreateTime] ,[UpdateTime] ");
            sqlStr.Append(" FROM [BasicEntityMap] ");
            sqlStr.Append(string.Format(" WHERE BitEntityId IN ({0}) ",bitEntityIds));
            sqlStr.Append(string.Format(" AND EntityType={0} ",entityType));

            var ds = DbHelperSql.Query(sqlStr.ToString());

            return DsToList(ds);
        }

        public List<BasicEntityMap> GetEntityMapsPeopleSet(int entityType, int companyId)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(" SELECT [Id],[EntityType],[CompanyId],[BitEntityId],[CompareEntityId] ");
            sqlStr.Append(" ,[IsPeopleSet],[CreateTime] ,[UpdateTime] ");
            sqlStr.Append(" FROM [BasicEntityMap] ");
            sqlStr.Append(" WHERE IsPeopleSet=1 AND CompanyId=@CompanyId AND EntityType=@EntityType ");

            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId},
                new SqlParameter("@EntityType",SqlDbType.Int){Value = entityType} 
            };
            var ds = DbHelperSql.Query(sqlStr.ToString(), parameters);

            return DsToList(ds);
        }

        private List<BasicEntityMap> DsToList(DataSet ds)
        {
            var lists = new List<BasicEntityMap>();
            if (ds != null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var map = new BasicEntityMap
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        CompanyId = Int32.Parse(row["CompanyId"].ToString()),
                        IsPeopleSet = ((bool)row["IsPeopleSet"]) ? 1 : 0,
                        BitEntityId = Int32.Parse(row["BitEntityId"].ToString()),
                        CompareEntityId = Int32.Parse(row["CompareEntityId"].ToString()),
                        EntityType = Int32.Parse(row["EntityType"].ToString()),
                        CreateTime = DateTime.Parse(row["CreateTime"].ToString()),
                        UpdateTime = DateTime.Parse(row["UpdateTime"].ToString())
                    };
                    lists.Add(map);
                }
            }
            return lists;
        }

        public BasicEntityMap GetEntityMap(int bitId
            , int companyId, int entityType)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT [Id],[EntityType],[CompanyId],[BitEntityId],[CompareEntityId] ");
            sqlStr.Append(",[IsPeopleSet],[CreateTime],[UpdateTime] ");
            sqlStr.Append(" FROM [BasicEntityMap] ");
            sqlStr.Append("WHERE BitEntityId=@BitEntityId AND ");
            sqlStr.Append(" CompanyId=@CompanyId AND EntityType=@EntityType ");
            SqlParameter[] paras =
            {
                new SqlParameter("@BitEntityId",SqlDbType.Int){Value = bitId}, 
                new SqlParameter("@CompanyId",SqlDbType.Int){Value =companyId }, 
                new SqlParameter("@EntityType",SqlDbType.Int){Value =entityType }, 
            };
            var ds = DbHelperSql.Query(sqlStr.ToString(), paras);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                var row = ds.Tables[0].Rows[0];
                var map = new BasicEntityMap
                {
                    Id = Int32.Parse(row["Id"].ToString()),
                    EntityType = entityType,
                    CompanyId = companyId,
                    BitEntityId = bitId,
                    CompareEntityId = Int32.Parse(row["CompareEntityId"].ToString()),
                    IsPeopleSet = ((bool)row["IsPeopleSet"]) ? 1 : 0,
                    CreateTime = DateTime.Parse(row["CreateTime"].ToString()),
                    UpdateTime = DateTime.Parse(row["UpdateTime"].ToString())
                };
                return map;
            }
            return null;
        }


        public Boolean Insert(BasicEntityMap map)
        {
            const string sqlStr = @"
INSERT INTO [BasicEntityMap]
 ([EntityType],[CompanyId] ,[BitEntityId],[CompareEntityId]
 ,[IsPeopleSet],[CreateTime] ,[UpdateTime]) 
 VALUES 
 (@EntityType,@CompanyId,@BitEntityId,@CompareEntityId,@IsPeopleSet,@CreateTime,@UpdateTime) 
";
            SqlParameter[] parameters = {
					new SqlParameter("@EntityType", SqlDbType.Int,4){Value = map.EntityType},
					new SqlParameter("@CompanyId", SqlDbType.Int){Value = map.CompanyId},
					new SqlParameter("@BitEntityId", SqlDbType.Int){Value = map.BitEntityId},
					new SqlParameter("@CompareEntityId", SqlDbType.Int){Value = map.CompareEntityId},
					new SqlParameter("@IsPeopleSet", SqlDbType.Int){Value = map.IsPeopleSet},
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value =DateTime.Now}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value =DateTime.Now} 
                                        };

            int rows = DbHelperSql.ExecuteSql(sqlStr, parameters);

            return rows > 0;
        }

        public Boolean Update(BasicEntityMap map)
        {
            const string sqlStr = @"
UPDATE BasicEntityMap
  SET CompareEntityId = @CompareEntityId  
 ,IsPeopleSet=@IsPeopleSet
 ,UpdateTime = @UpdateTime 
 WHERE Id=@Id  
";
            SqlParameter[] parameters = {
					new SqlParameter("@CompareEntityId", SqlDbType.Int){Value =map.CompareEntityId},
					new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value =DateTime.Now},
                    new SqlParameter("@IsPeopleSet",SqlDbType.Int){Value = map.IsPeopleSet},
                    new SqlParameter("@Id",SqlDbType.Int){Value = map.Id}
                                        };
            int rows = DbHelperSql.ExecuteSql(sqlStr, parameters);
            return rows > 0;
        }

        public bool Delete(int bitId, int companyId, int entityType)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("DELETE FROM dbo.BasicEntityMap ");
            sqlStr.Append(" WHERE BitEntityId=@BitEntityId ");
            sqlStr.Append(" AND CompanyId=@CompanyId ");
            sqlStr.Append(" AND EntityType=@EntityType ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@BitEntityId",SqlDbType.Int){Value = bitId},
                new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId},
                new SqlParameter("@EntityType",SqlDbType.Int){Value = entityType} 
            };
            return DbHelperSql.ExecuteSql(sqlStr.ToString(), parameters) > 0;
        }
    }
}
