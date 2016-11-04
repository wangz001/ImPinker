using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AhModel;

namespace AhDal
{
    public class BasicStylePropertyDal
    {
        public Boolean IsExit(BasicStyleProperty model)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from BasicStyleProperty");
            strSql.Append(" where ID=@ID and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID}			};

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 没有ID的参配方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="englishName">（英文名）腾讯汽车专用</param>
        /// <param name="propertyGroupId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public bool IsExit(string name,string englishName, int propertyGroupId, int companyId)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from BasicStyleProperty");
            strSql.Append(" WHERE CompanyId=@CompanyId AND PropertyGroupId =@PropertyGroupId AND Name=@Name ");
            SqlParameter[] parameters = {
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId}, 
					new SqlParameter("@PropertyGroupId", SqlDbType.Int){Value = propertyGroupId},
					new SqlParameter("@Name", SqlDbType.NVarChar){Value = name}	
                                        };

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean Insert(BasicStyleProperty model)
        {
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO [BasicStyleProperty]");
            strSql.Append(" (CompanyId,Id,Name,EnglishName,PropertyGroupId,CreateTime,UpdateTime ) ");
            strSql.Append(" VALUES ");
            strSql.Append(" (@CompanyId,@ID,@Name,@EnglishName,@PropertyGroupId,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters = {
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId},
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
					new SqlParameter("@EnglishName", SqlDbType.NVarChar,50){Value = model.EnglishName},
                    new SqlParameter("@PropertyGroupId",SqlDbType.Int){Value = model.PropertyGroupId}, 
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = model.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime} 
                                        };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;
        }

        public bool NeedUpdate(BasicStyleProperty model)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT COUNT(1) ");
            sqlStr.Append(" FROM [BasicStyleProperty] ");
            sqlStr.Append(" WHERE id =@ID ");
            sqlStr.Append(" AND Name=@Name AND EnglishName=@EnglishName ");
            sqlStr.Append(" AND PropertyGroupId=@PropertyGroupId and CompanyId=@CompanyId  ");

            SqlParameter[] parameters = { 
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId},
                        new SqlParameter("@ID",SqlDbType.Int){Value = model.ID},
                        new SqlParameter("@Name",SqlDbType.VarChar){Value = model.Name},
                        new SqlParameter("@EnglishName",SqlDbType.VarChar){Value = model.EnglishName},
                        new SqlParameter("@PropertyGroupId",SqlDbType.Int){Value = model.PropertyGroupId} 
                                        };
            bool exists = DbHelperSql.Exists(sqlStr.ToString(), parameters);
            return !exists;
        }

        public Boolean Update(BasicStyleProperty model)
        {

            var strSql = new StringBuilder();
            strSql.Append("UPDATE [BasicStyleProperty] SET ");
            strSql.Append(" [Name]=@Name, ");
            strSql.Append(" [EnglishName]=@EnglishName, ");
            strSql.Append(" [PropertyGroupId]=@PropertyGroupId, ");
            strSql.Append(" [UpdateTime]=@UpdateTime ");
            strSql.Append(" where ID=@ID and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
					new SqlParameter("@EnglishName", SqlDbType.NVarChar,50){Value = model.EnglishName},
                    new SqlParameter("@PropertyGroupId",SqlDbType.Int){Value = model.PropertyGroupId}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        public DataSet GetLists(int companyId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT [CompanyId] ,[Id] ,[Name] ,[EnglishName],[PropertyGroupId] ");
            strSql.Append(" ,[CreateTime] ,[UpdateTime] ");
            strSql.Append(" FROM [BasicStyleProperty] ");
            strSql.Append(" WHERE  CompanyId=@CompanyId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId}
            };

            return DbHelperSql.Query(strSql.ToString(), parameters);

        }

        /// <summary>
        /// 获取数据库中最小的参配ID。负数
        /// </summary> 
        /// <returns></returns>
        public int GetMinId()
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT MIN(ID) FROM BasicStyleProperty ");
            
            return (int)DbHelperSql.GetSingle(strSql.ToString());
        }

        public bool IsExitPropertyWithEnglishName(BasicStyleProperty model)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from BasicStyleProperty");
            strSql.Append(" where EnglishName=@EnglishName and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId}, 
					new SqlParameter("@EnglishName", SqlDbType.VarChar){Value = model.EnglishName}			};

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean UpdatePropertyWithEnglishName(BasicStyleProperty model)
        {

            var strSql = new StringBuilder();
            strSql.Append("UPDATE [BasicStyleProperty] SET ");
            strSql.Append(" [Name]=@Name, ");
            strSql.Append(" [PropertyGroupId]=@PropertyGroupId, ");
            strSql.Append(" [UpdateTime]=@UpdateTime ");
            strSql.Append(" where EnglishName=@EnglishName and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
					new SqlParameter("@EnglishName", SqlDbType.NVarChar,50){Value = model.EnglishName},
                    new SqlParameter("@PropertyGroupId",SqlDbType.Int){Value = model.PropertyGroupId}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime}, 
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId}};
            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }
    }
}
