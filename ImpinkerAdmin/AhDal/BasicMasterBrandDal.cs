using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AhModel;

namespace AhDal
{
    public class BasicMasterBrandDal
    {
        public Boolean IsExit(BasicMasterBrand model)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from [BasicMasterBrand]");
            strSql.Append(" where ID=@ID ");
            strSql.Append(" and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value =model.ID },
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId}
                                        };

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean Insert(BasicMasterBrand brand)
        {
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO [BasicMasterBrand]");
            strSql.Append(" (CompanyId,[Id],[Name],CreateTime,UpdateTime ) ");
            strSql.Append(" VALUES ");
            strSql.Append(" (@CompanyId,@ID,@Name,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters = {
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = brand.CompanyId}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = brand.ID},
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = brand.Name},
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = brand.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = brand.UpdateTime} 
                                        };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;
        }

        public bool NeedUpdate(BasicMasterBrand brand)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT Id ");
            sqlStr.Append(" FROM BasicMasterBrand ");
            sqlStr.Append(" WHERE id =@ID ");
            sqlStr.Append(" AND Name=@Name ");
            sqlStr.Append(" AND IsRemoved=0 ");
            sqlStr.Append(" AND CompanyId=@CompanyId ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@ID",SqlDbType.Int){Value = brand.ID},
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value = brand.CompanyId},
                        new SqlParameter("@Name",SqlDbType.VarChar){Value = brand.Name}
                                        };
            bool exists = DbHelperSql.Exists(sqlStr.ToString(), parameters);
            return !exists;
        }

        public Boolean Update(BasicMasterBrand brand)
        {

            var strSql = new StringBuilder();
            strSql.Append("UPDATE BasicMasterBrand SET ");
            strSql.Append(" [Name]=@Name, ");
            strSql.Append(" [UpdateTime]=@UpdateTime, ");
            strSql.Append(" [IsRemoved]=@IsRemoved ");
            strSql.Append(" where ID=@ID and CompanyId=@CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = brand.Name},
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = brand.UpdateTime}, 
                    new SqlParameter("@IsRemoved",SqlDbType.Int){Value = brand.IsRemoved}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = brand.ID},
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = brand.CompanyId} };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        public DataSet GetLists(string strWhere)
        {
            var strSql = new StringBuilder();
            strSql.Append("select Id,Name,CompanyId,CreateTime,UpdateTime,IsRemoved ");
            strSql.Append(" FROM BasicMasterBrand ");
            strSql.Append("  WHERE IsRemoved=0 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" AND " + strWhere);
            }
            strSql.Append(" ORDER BY Name ASC ");

            return DbHelperSql.Query(strSql.ToString());
        }

        public List<BasicMasterBrand> GetAllMasterBrands(int companyId = 0)
        {
            var ahMasterBrands = new List<BasicMasterBrand>();
            var ds = new DataSet();
            ds = companyId!=0 ? GetLists(" CompanyId=" + companyId) : GetLists("");
            if (ds != null)
            {
                ahMasterBrands = DsToList(ds);
            }
            return ahMasterBrands;
        }

        public BasicMasterBrand GetModel(int brandId, int companyId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT [CompanyId],[Id],[Name],[CreateTime],[UpdateTime]");
            strSql.Append(" FROM BasicMasterBrand ");
            strSql.Append(" WHERE CompanyId=@CompanyId AND ID = @ID  ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = brandId},
                    new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId}
                                        };
            var ds = DbHelperSql.Query(strSql.ToString(), parameters);
            if (ds == null)
            {
                return null;
            }
            var dt = ds.Tables[0];
            var model = new BasicMasterBrand
            {
                ID = Int32.Parse(dt.Rows[0]["Id"].ToString()),
                CompanyId = Int32.Parse(dt.Rows[0]["CompanyId"].ToString()),
                Name = dt.Rows[0]["Name"].ToString()
            };
            return model;
        }

        public List<BasicMasterBrand> DsToList(DataSet ds)
        {
            var ahMasterBrands = new List<BasicMasterBrand>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var brand = new BasicMasterBrand
                {
                    ID = Int32.Parse(row["Id"].ToString()),
                    Name = row["Name"] == null ? "" : row["Name"].ToString(),
                    CompanyId = Int32.Parse(row["CompanyId"].ToString()),
                    CreateTime = DateTime.Parse(row["CreateTime"].ToString()),
                    UpdateTime = DateTime.Parse(row["UpdateTime"].ToString()),
                    IsRemoved = ((bool)row["IsRemoved"]) ? 1 : 0
                };
                ahMasterBrands.Add(brand);
            }
            return ahMasterBrands;

        }
    }
}
