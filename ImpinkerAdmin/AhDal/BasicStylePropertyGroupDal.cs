using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AhModel;

namespace AhDal
{
    public class BasicStylePropertyGroupDal
    {
        public Boolean IsExit(BasicStylePropertyGroup model)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from BasicStylePropertyGroup");
            strSql.Append(" where ID=@ID and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int){Value = model.ID},
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId}
                                        };
            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean Insert(BasicStylePropertyGroup model)
        {
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO [BasicStylePropertyGroup]");
            strSql.Append(" (CompanyId,ID,Name,CreateTime,UpdateTime ) ");
            strSql.Append(" VALUES ");
            strSql.Append(" (@CompanyId,@ID,@Name,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters = {
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId},
                    new SqlParameter("@ID",SqlDbType.Int){Value = model.ID}, 
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = model.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime} 
                                        };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;
        }

        public bool NeedUpdate(BasicStylePropertyGroup model)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT COUNT(1) ");
            sqlStr.Append(" FROM [BasicStylePropertyGroup] ");
            sqlStr.Append(" WHERE id =@ID ");
            sqlStr.Append(" AND Name=@Name and CompanyId=@CompanyId  ");

            SqlParameter[] parameters = { 
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId}, 
                        new SqlParameter("@ID",SqlDbType.Int){Value = model.ID},
                        new SqlParameter("@Name",SqlDbType.VarChar){Value = model.Name}
                                        };
            bool exists = DbHelperSql.Exists(sqlStr.ToString(), parameters);
            return !exists;
        }

        public Boolean Update(BasicStylePropertyGroup model)
        {

            var strSql = new StringBuilder();
            strSql.Append("UPDATE [BasicStylePropertyGroup] SET ");
            strSql.Append(" [Name]=@Name, ");
            strSql.Append(" [UpdateTime]=@UpdateTime ");
            strSql.Append(" where ID=@ID and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime}, 
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId},
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        public BasicStylePropertyGroup GetModel(string groupName)
        {
            var strSql = new StringBuilder();
            strSql.Append("select  top 1 ID,Name from BasicStylePropertyGroup ");
            strSql.Append(" where Name = @Name");
            SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50)
			};
            parameters[0].Value = groupName;

            var model = new BasicStylePropertyGroup();
            DataSet ds = DbHelperSql.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.ID = int.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
                model.Name = ds.Tables[0].Rows[0]["Name"].ToString();
                return model;
            }
            else
            {
                return null;
            }
        }

        public DataSet GetLists(int companyId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT [CompanyId],[Id] ,[Name],[CreateTime],[UpdateTime] ");
            strSql.Append(" FROM [BasicStylePropertyGroup] ");
            strSql.Append(" WHERE  CompanyId=@CompanyId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId}
            };
            return DbHelperSql.Query(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 获取某竞品的最大参配分组id
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public object GetMaxGroupId(int companyId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT MAX(Id)  FROM [BasicStylePropertyGroup] ");
            strSql.Append(" WHERE CompanyId =@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@CompanyId", SqlDbType.Int){Value = companyId}
                                        };
            return DbHelperSql.GetSingle(strSql.ToString(), parameters);

        }
    }
}
