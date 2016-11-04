using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AhModel;

namespace AhDal
{
    public class BasicPropertyMapDal
    {
        public List<BasicPropertyMap> GetPropertyMap()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT [Id] ,[CompanyId] ,[BitPropertyId] ,[ComParePropertyId] ,[CreateTime] ,[UpdateTime] ,[IsRemoved] ");
            stringBuilder.Append(" FROM [BasicPropertyMap] ");
            var ds = DbHelperSql.Query(stringBuilder.ToString());
            return DsToList(ds);
        }

        private List<BasicPropertyMap> DsToList(DataSet ds)
        {
            var lists = new List<BasicPropertyMap>();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var result = new BasicPropertyMap();
                    result.ID = Int32.Parse(row["Id"].ToString());
                    result.BitPropertyId = Int32.Parse(row["BitPropertyId"].ToString());
                    result.CompanyId = Int32.Parse(row["CompanyId"].ToString());
                    result.ComparePropertyId = Int32.Parse(row["ComParePropertyId"].ToString());
                    result.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                    result.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
                    result.IsRemoved = ((bool)row["IsRemoved"]) ? 1 : 0;
                    lists.Add(result);
                }
            }
            return lists;
        }

        public BasicPropertyMap GetPropertyMap(int companyId, int bitPropertyId)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("SELECT [Id] ,[CompanyId] ,[BitPropertyId] ,[ComParePropertyId] ,[CreateTime] ,[UpdateTime] ,[IsRemoved] ");
            stringBuilder.Append(" FROM [BasicPropertyMap] ");
            stringBuilder.Append(" WHERE CompanyId=@CompanyId AND BitPropertyId=@BitPropertyId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyId", SqlDbType.Int) {Value = companyId},
                new SqlParameter("@BitPropertyId",SqlDbType.Int){Value = bitPropertyId},
            };
            var ds = DbHelperSql.Query(stringBuilder.ToString(), parameters);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return DsToList(ds).First();
            }
            return null;
        }

        public bool Update(BasicPropertyMap map)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("UPDATE [BasicPropertyMap] ");
            stringBuilder.Append(" SET ComParePropertyId=@ComParePropertyId, ");
            stringBuilder.Append(" UpdateTime=@UpdateTime ");
            stringBuilder.Append(" WHERE CompanyId=@CompanyId AND BitPropertyId=@BitPropertyId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyId", SqlDbType.Int) {Value = map.CompanyId},
                new SqlParameter("@BitPropertyId",SqlDbType.Int){Value = map.BitPropertyId},
                new SqlParameter("@ComParePropertyId",SqlDbType.Int){Value = map.ComparePropertyId},
                new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = map.UpdateTime},
            };
            var flag = DbHelperSql.ExecuteSql(stringBuilder.ToString(), parameters);
            return flag > 0;

        }

        public bool Insert(BasicPropertyMap map)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("INSERT INTO [BasicPropertyMap] ");
            stringBuilder.Append(" ([CompanyId] ,[BitPropertyId] ,[ComParePropertyId] ");
            stringBuilder.Append(" ,[CreateTime],[UpdateTime]) ");
            stringBuilder.Append(" VALUES ");
            stringBuilder.Append(" (@CompanyId,@BitPropertyId,@ComParePropertyId ");
            stringBuilder.Append(" ,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyId", SqlDbType.Int) {Value = map.CompanyId},
                new SqlParameter("@BitPropertyId",SqlDbType.Int){Value = map.BitPropertyId},
                new SqlParameter("@ComParePropertyId",SqlDbType.Int){Value = map.ComparePropertyId},
                new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = map.CreateTime},
                new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = map.UpdateTime},
            };
            var flag = DbHelperSql.ExecuteSql(stringBuilder.ToString(), parameters);
            return flag > 0;
        }
    }
}
