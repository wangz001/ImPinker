using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AhModel;

namespace AhDal
{
    public class BasicMakeDal
    {
        public Boolean IsExit(BasicMake model)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from [BasicMake]");
            strSql.Append(" where ID=@ID and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId}
                                        };

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean IsExit(string name,int companyId,int brandId)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from [BasicMake]");
            strSql.Append(" where Name=@Name and CompanyId=@CompanyId and BrandId=@BrandId ");
            SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar){Value = name},
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId},
                    new SqlParameter("@BrandId",SqlDbType.Int){Value = brandId}
                                        };
           

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean Insert(BasicMake model)
        {
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO [BasicMake]");
            strSql.Append(" ([Id],CompanyId,[Name],CreateTime,UpdateTime ) ");
            strSql.Append(" VALUES ");
            strSql.Append(" (@ID,@CompanyId,@Name,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId},
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = model.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime} 
                                        };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;
        }

        public bool NeedUpdate(BasicMake model)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT Id ");
            sqlStr.Append(" FROM [BasicMake] ");
            sqlStr.Append(" WHERE id =@ID ");
            sqlStr.Append(" AND CompanyId=@CompanyId ");
            sqlStr.Append(" AND Name=@Name ");
            sqlStr.Append(" AND IsRemoved=0 ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@ID",SqlDbType.Int){Value = model.ID},
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId},
                        new SqlParameter("@Name",SqlDbType.VarChar){Value = model.Name}
                                        };
            bool exists = DbHelperSql.Exists(sqlStr.ToString(), parameters);
            return !exists;
        }

        public Boolean Update(BasicMake model)
        {

            var strSql = new StringBuilder();
            strSql.Append("UPDATE [BasicMake] SET ");
            strSql.Append(" [Name]=@Name, ");
            strSql.Append(" [UpdateTime]=@UpdateTime, ");
            strSql.Append(" [IsRemoved]=@IsRemoved ");
            strSql.Append(" where ID=@ID and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime}, 
                    new SqlParameter("@IsRemoved",SqlDbType.Int){Value = model.IsRemoved}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        

        /// <summary>
        /// 获取最小ID
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public object GetMinId()
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT MIN(id) FROM BasicMake ");
             return DbHelperSql.GetSingle(sqlStr.ToString());
            
        }

        public int GetMakeId(string manufacturerName, int companyId)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT Id FROM BasicMake WHERE CompanyId=@CompanyId AND Name = @Name ");

            SqlParameter[] parameters = { 
                        
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value =companyId},
                        new SqlParameter("@Name",SqlDbType.NVarChar){Value =manufacturerName}
                                        };
            var obj= DbHelperSql.GetSingle(sqlStr.ToString(), parameters);
            return obj==null ? 0 : Int32.Parse(obj.ToString());
        }

        public List<BasicMake> GetListsByMasterBrandId(int companyId, int id)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT  CompanyId , Id , Name , CreateTime , UpdateTime ,IsRemoved ");
            strSql.Append(" FROM    dbo.BasicMake ");
            strSql.Append(" WHERE   IsRemoved = 0 ");
            strSql.Append(" AND dbo.BasicMake.CompanyId = @CompanyId ");
            strSql.Append(" AND ID IN ( SELECT  MakeId ");
            strSql.Append(" FROM    dbo.BasicMasterBrandJoinMake ");
            strSql.Append(" WHERE   CompanyId = @CompanyId ");
            strSql.Append(" AND MasterBrandId = @MasterBrandId ) ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value =companyId},
                        new SqlParameter("@MasterBrandId",SqlDbType.Int){Value =id}
                                        };
            var ds= DbHelperSql.Query(strSql.ToString(),parameters);
            return DsToList(ds);
        }

        private List<BasicMake> DsToList(DataSet ds)
        {
            var lists = new List<BasicMake>();
            if (ds!=null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var make = new BasicMake
                    {
                        ID = Int32.Parse(row["Id"].ToString()),
                        Name = row["Name"] == null ? "" : row["Name"].ToString(),
                        CompanyId = Int32.Parse(row["CompanyId"].ToString()),
                        CreateTime = DateTime.Parse(row["CreateTime"].ToString()),
                        UpdateTime = DateTime.Parse(row["UpdateTime"].ToString()),
                        IsRemoved = ((bool)row["IsRemoved"]) ? 1 : 0
                    };
                    lists.Add(make);
                }
            }
            return lists;
        }

        public BasicMake GetMakeById(int companyId, int makeId)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT CompanyId , Id , Name , CreateTime , UpdateTime ,IsRemoved ");
            sqlStr.Append(" FROM [BasicMake] ");
            sqlStr.Append(" WHERE Id =@Id ");
            sqlStr.Append(" AND CompanyId=@CompanyId ");
            sqlStr.Append(" AND IsRemoved=0 ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@Id",SqlDbType.Int){Value = makeId},
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId},
                                        };
            var ds = DbHelperSql.Query(sqlStr.ToString(), parameters);
            var lists = DsToList(ds);
            if (lists!=null&&lists.Count>0)
            {
                return lists[0];
            }
            return null;
        }
    }
}
