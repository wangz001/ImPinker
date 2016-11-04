using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using AhModel;

namespace AhDal
{
    public class BasicStyleDal
    {
        public Boolean IsExit(BasicStyle model)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from BasicStyle");
            strSql.Append(" where ID=@ID and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId}			};

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean Insert(BasicStyle model)
        {
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO [BasicStyle]");
            strSql.Append(" ([Id],CompanyId,[Name],SerialId,Year,Price,SaleStatus,CreateTime,UpdateTime ) ");
            strSql.Append(" VALUES ");
            strSql.Append(" (@ID,@CompanyId,@Name,@SerialId,@Year,@Price,@SaleStatus,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId},
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
                    new SqlParameter("@SerialId",SqlDbType.Int){Value = model.SerialId}, 
                    new SqlParameter("@SaleStatus",SqlDbType.VarChar){Value = model.SaleStatus}, 
                    new SqlParameter("@Year",SqlDbType.VarChar){Value = model.Year}, 
                    new SqlParameter("@Price",SqlDbType.Decimal){Value = model.Price}, 
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = model.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime} 
                                        };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;
        }

        public bool NeedUpdate(BasicStyle model)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT Id ");
            sqlStr.Append(" FROM [BasicStyle] ");
            sqlStr.Append(" WHERE id =@ID ");
            sqlStr.Append(" AND CompanyId=@CompanyId ");
            sqlStr.Append(" AND Name=@Name ");
            sqlStr.Append(" AND SerialId=@SerialId ");
            sqlStr.Append(" AND SaleStatus=@SaleStatus ");
            sqlStr.Append(" AND Year=@Year ");
            sqlStr.Append(" AND Price=@Price ");
            sqlStr.Append(" AND IsRemoved=0 ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@ID",SqlDbType.Int){Value = model.ID},
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId},
                        new SqlParameter("@Name",SqlDbType.VarChar){Value = model.Name},
                        new SqlParameter("@SerialId",SqlDbType.Int){Value = model.SerialId} ,
                        new SqlParameter("@SaleStatus",SqlDbType.VarChar){Value = model.SaleStatus} ,
                        new SqlParameter("@Year",SqlDbType.VarChar){Value = model.Year} ,
                        new SqlParameter("@Price",SqlDbType.Decimal){Value = model.Price} 
                                        };
            bool exists = DbHelperSql.Exists(sqlStr.ToString(), parameters);
            return !exists;
        }

        public Boolean Update(BasicStyle model)
        {

            var strSql = new StringBuilder();
            strSql.Append("UPDATE [BasicStyle] SET ");
            strSql.Append(" [Name]=@Name, ");
            strSql.Append(" [SerialId]=@SerialId, ");
            strSql.Append(" [Price]=@Price, ");
            strSql.Append(" [UpdateTime]=@UpdateTime, ");
            strSql.Append(" [IsRemoved]=@IsRemoved ");
            strSql.Append(" where ID=@ID and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
                    new SqlParameter("@SerialId",SqlDbType.Int){Value = model.SerialId}, 
                    new SqlParameter("@Year",SqlDbType.VarChar){Value = model.Year}, 
                    new SqlParameter("@SaleStatus",SqlDbType.VarChar){Value = model.SaleStatus}, 
                    new SqlParameter("@Price",SqlDbType.Decimal){Value = model.Price}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime}, 
                    new SqlParameter("@IsRemoved",SqlDbType.Int){Value = model.IsRemoved}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        public DataSet GetLists(int companyId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT [CompanyId] ,[Id] ,[Name] ,[SerialId] ,[Year] ,[Price] ");
            strSql.Append(" ,[SaleStatus] ,[CreateTime],[UpdateTime],[IsRemoved] ");
            strSql.Append(" FROM [BasicStyle] ");
            strSql.Append(" WHERE IsRemoved=0 and  CompanyId=@CompanyId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId}
            };

            return DbHelperSql.Query(strSql.ToString(), parameters);
        }

        public DataSet GetListsWithProperty(int companyId, int serialId, int comparePropertyId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT s.[CompanyId] ,s.[Id]  ,s.[Name] ,s.[SerialId] ");
            strSql.Append(" ,s.[Year] ,s.[Price] ,s.[SaleStatus] ,v.Value ");
            strSql.Append(" FROM [CarsDataAutoHome].[dbo].[BasicStyle] s ");
            strSql.Append(" LEFT JOIN dbo.BasicStylePropertyValue v ON s.Id=v.StyleId ");
            strSql.Append(" WHERE s.SerialId=@SerialId AND s.CompanyId=@CompanyId AND v.PropertyId=@PropertyId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@SerialId",SqlDbType.Int){Value = serialId},
                new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId},
                new SqlParameter("@PropertyId",SqlDbType.Int){Value = comparePropertyId}
            };

            return DbHelperSql.Query(strSql.ToString(), parameters);
        }

        public BasicStyle GetStyleById(int companyId, int bisicStyleId)
        {
            BasicStyle basicStyle=null;
            var strSql = new StringBuilder();
            strSql.Append("SELECT [CompanyId] ,[Id] ,[Name] ,[SerialId] ,[Year] ");
            strSql.Append(" ,[Price] ,[SaleStatus] ,[CreateTime] ,[UpdateTime],[IsRemoved] ");
            strSql.Append(" FROM [BasicStyle] ");
            strSql.Append(" WHERE CompanyId=@CompanyId AND ID=@ID ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId},
                new SqlParameter("@ID",SqlDbType.Int){Value = bisicStyleId}
            };
            var ds= DbHelperSql.Query(strSql.ToString(), parameters);
            if (ds!=null&&ds.Tables[0].Rows.Count>0)
            {
                basicStyle = new BasicStyle();
                var row = ds.Tables[0].Rows[0];
                basicStyle.ID = Int32.Parse(row["Id"].ToString());
                basicStyle.CompanyId = Int32.Parse(row["CompanyId"].ToString());
                basicStyle.Name = row["Name"].ToString();
                basicStyle.SerialId = Int32.Parse(row["SerialId"].ToString());
                basicStyle.Year = row["Year"].ToString();
                basicStyle.Price = decimal.Parse(row["Price"].ToString());
                basicStyle.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                basicStyle.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
            }
            return basicStyle;
        }

        public DataSet GetListsBySerialId(int companyId, int serialId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT [CompanyId] ,[Id] ,[Name] ,[SerialId] ,[Year] ,[Price] ");
            strSql.Append(" ,[SaleStatus] ,[CreateTime],[UpdateTime],[IsRemoved] ");
            strSql.Append(" FROM [BasicStyle] ");
            strSql.Append(" WHERE IsRemoved=0 and  CompanyId=@CompanyId and SerialId=@SerialId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId},
                new SqlParameter("@SerialId",SqlDbType.Int){Value = serialId} 
            };

            return DbHelperSql.Query(strSql.ToString(), parameters);
        }
    }
}
