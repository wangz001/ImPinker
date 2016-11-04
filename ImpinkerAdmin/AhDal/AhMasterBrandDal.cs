using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using AhModel;

namespace AhDal
{
    public class AhMasterBrandDal
    {

        public Boolean IsExit(AhModel.AhMasterBrand masterBrand)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from [AHMasterBrand]");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)			};
            parameters[0].Value = masterBrand.ID;

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean Insert(AhMasterBrand masterBrand)
        {
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO [CarsDataAutoHome].[dbo].[AHMasterBrand]");
            strSql.Append(" ([Id] ,[Initial],[MasterBrandName],CreateTime,UpdateTime ) ");
            strSql.Append(" VALUES ");
            strSql.Append(" (@ID,@Initial,@Name,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = masterBrand.ID},
					new SqlParameter("@Initial", SqlDbType.VarChar,50){Value = masterBrand.Initial},
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = masterBrand.MasterBrandName},
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = masterBrand.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = masterBrand.UpdateTime} 
                                        };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;
        }

        public bool NeedUpdate(AhMasterBrand mb)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT Id ");
            sqlStr.Append(" FROM [CarsDataAutoHome].[dbo].[AHMasterBrand] ");
            sqlStr.Append(" WHERE id =@ID ");
            sqlStr.Append(" AND MasterBrandName=@Name ");
            sqlStr.Append(" AND IsRemoved=0 ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@ID",SqlDbType.Int){Value = mb.ID},
                        new SqlParameter("@Name",SqlDbType.VarChar){Value = mb.MasterBrandName}
                                        };
            bool exists = DbHelperSql.Exists(sqlStr.ToString(), parameters);
            return !exists;
        }

        public Boolean Update(AhMasterBrand masterBrand)
        {

            var strSql = new StringBuilder();
            strSql.Append("UPDATE [CarsDataAutoHome].[dbo].[AHMasterBrand] SET ");
            strSql.Append(" Initial=@Initial, ");
            strSql.Append(" [MasterBrandName]=@Name, ");
            strSql.Append(" [UpdateTime]=@UpdateTime, ");
            strSql.Append(" [IsRemoved]=@IsRemoved ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@Initial", SqlDbType.VarChar,50){Value = masterBrand.Initial},
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = masterBrand.MasterBrandName},
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = masterBrand.UpdateTime}, 
                    new SqlParameter("@IsRemoved",SqlDbType.Int){Value = masterBrand.IsRemoved}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = masterBrand.ID}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        public DataSet GetLists(string strWhere)
        {
            var strSql = new StringBuilder();
            strSql.Append("select Id,Initial,MasterBrandName ");
            strSql.Append(" FROM AHMasterBrand ");
            strSql.Append("  WHERE IsRemoved=0 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" AND " + strWhere);
            }
            strSql.Append(" ORDER BY MasterBrandName ASC ");

            return DbHelperSql.Query(strSql.ToString());
        }

        public List<AhMasterBrand> GetAllMasterBrands()
        {
            var ahMasterBrands = new List<AhMasterBrand>();
            var ds = GetLists("");
            if (ds != null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var masterBrand = new AhMasterBrand
                    {
                        ID = Int32.Parse(row["Id"].ToString()),
                        Initial = row["Initial"] == null ? "" : row["Initial"].ToString(),
                        MasterBrandName = row["MasterBrandName"] == null ? "" : row["MasterBrandName"].ToString()
                    };
                    ahMasterBrands.Add(masterBrand);
                }
            }
            return ahMasterBrands;
        }


        public int GetCount()
        {
            const string strSql = @"
SELECT COUNT(1) 
  FROM [CarsDataAutoHome].[dbo].[AHMasterBrand] 
  WHERE IsRemoved=0
";

            return Convert.ToInt32(DbHelperSql.GetSingle(strSql));
        }

        public bool Delete(int id)
        {
            const string strSql = @"
DELETE FROM [AHMasterBrand]
 WHERE Id=@ID
";
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = id}
                                        };
            int rows = DbHelperSql.ExecuteSql(strSql, parameters);
            return rows > 0;
        }

        public int DeleteList(string IDlist)
        {
            string sqlStr = "UPDATE AHMasterBrand  SET [IsRemoved] = 1 where ID in ("+IDlist+")";
           
            int rows = DbHelperSql.ExecuteSql(sqlStr);
            return rows;
        }
    }
}
