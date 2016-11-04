using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AhModel;

namespace AhDal
{
    public class AHEntityRecordDal
    {

        public DataSet GetEntities(int EntityType, int[] bitIds)
        {


            return null;
        }

        public bool Add(AHEntityRecord record)
        {
            const string sqlStr = @"
INSERT INTO [CarsDataAutoHome].[dbo].[AHEntityRecord] 
 ([EntityType],[NewAddCount],[UpdateCount],[CreateTime]) 
 values (
@EntityType,@NewAddCount,@UpdateCount,@CreateTime)
";

            SqlParameter[] parameters = {
					new SqlParameter("@EntityType", SqlDbType.Int,4){Value = record.EntityType},
					new SqlParameter("@NewAddCount", SqlDbType.Int,4){Value = record.NewAddCount},
					new SqlParameter("@UpdateCount", SqlDbType.Int){Value = record.UpdateCount},
					new SqlParameter("@CreateTime", SqlDbType.DateTime){Value = record.CreateTime}
                                        };
            int rows = DbHelperSql.ExecuteSql(sqlStr, parameters);

            return rows > 0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public List<AHEntityRecord> GetModels(int entityType, int year, int month)
        {
            var stringBuilder = new StringBuilder("SELECT * FROM [CarsDataAutoHome].[dbo].[AHEntityRecord] ");
            stringBuilder.AppendFormat("WHERE YEAR(CreateTime) = @Year ");
            stringBuilder.AppendFormat("AND MONTH(CreateTime) = @Month ");
            stringBuilder.AppendFormat("AND EntityType = @EntityType ");
            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Year", SqlDbType.Int));
            parameters.Add(new SqlParameter("@Month", SqlDbType.Int));
            parameters.Add(new SqlParameter("@EntityType", SqlDbType.Int));
            parameters[0].Value = year;
            parameters[1].Value = month;
            parameters[2].Value = entityType;
            DataTable dataTable = DbHelperSql.Query(stringBuilder.ToString(), parameters.ToArray()).Tables[0];
            if (dataTable.Rows.Count > 0)
            {
                var ahEntityRecords = new List<AHEntityRecord>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var model = new AHEntityRecord();
                    model.Id = int.Parse(row["ID"].ToString());
                    model.EntityType = int.Parse(row["EntityType"].ToString());
                    model.NewAddCount = int.Parse(row["NewAddCount"].ToString());
                    model.UpdateCount = int.Parse(row["UpdateCount"].ToString());
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                    ahEntityRecords.Add(model);
                }
                return ahEntityRecords;
            }
            return null;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AHEntityRecord GetAhEntityRecord(int entityType, DateTime createTime)
        {
            const string sqlStr = @"
SELECT * FROM [dbo].[AHEntityRecord]  
 WHERE CreateTime BETWEEN @time1 AND @time2 
 AND EntityType = @EntityType
";

            SqlParameter[] parameters = {
					new SqlParameter("@time1", SqlDbType.DateTime){Value = createTime.ToShortDateString()},
					new SqlParameter("@time2", SqlDbType.DateTime){Value = createTime.AddDays(1).ToShortDateString()},
                    new SqlParameter("@EntityType", SqlDbType.Int){Value = entityType}
			};

            DataTable dataTable = DbHelperSql.Query(sqlStr, parameters).Tables[0];
            if (dataTable.Rows.Count > 0)
            {
                var model = new AHEntityRecord();
                model.Id = int.Parse(dataTable.Rows[0]["ID"].ToString());
                model.EntityType = int.Parse(dataTable.Rows[0]["EntityType"].ToString());
                model.NewAddCount = int.Parse(dataTable.Rows[0]["NewAddCount"].ToString());
                model.UpdateCount = int.Parse(dataTable.Rows[0]["UpdateCount"].ToString());
                model.CreateTime = DateTime.Parse(dataTable.Rows[0]["CreateTime"].ToString());
                return model;
            }
            return null;
        }
    }
}
