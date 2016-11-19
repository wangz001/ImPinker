using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using AhModel;
using AhModel.ViewModel;

namespace AhDal
{
    public class BasicSerialDal
    {
        public Boolean IsExit(BasicSerial model)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from [BasicSerial]");
            strSql.Append(" where ID=@ID and CompanyId = @CompanyId  ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId}
                                        };

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean Insert(BasicSerial model)
        {
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO [BasicSerial]");
            strSql.Append(" ([Id],CompanyId,[Name],ManufacturerId,CreateTime,UpdateTime ) ");
            strSql.Append(" VALUES ");
            strSql.Append(" (@ID,@CompanyId,@Name,@ManufacturerId,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId},
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
                    new SqlParameter("@ManufacturerId",SqlDbType.Int){Value = model.ManufacturerId}, 
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = model.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime} 
                                        };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;
        }

        public bool NeedUpdate(BasicSerial model)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT Id ");
            sqlStr.Append(" FROM [BasicSerial] ");
            sqlStr.Append(" WHERE id =@ID ");
            sqlStr.Append(" AND CompanyId=@CompanyId ");
            sqlStr.Append(" AND Name=@Name ");
            sqlStr.Append(" AND ManufacturerId=@ManufacturerId ");
            sqlStr.Append(" AND IsRemoved=0 ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@ID",SqlDbType.Int){Value = model.ID},
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId},
                        new SqlParameter("@Name",SqlDbType.VarChar){Value = model.Name},
                        new SqlParameter("@ManufacturerId",SqlDbType.Int){Value = model.ManufacturerId} 
                                        };
            bool exists = DbHelperSql.Exists(sqlStr.ToString(), parameters);
            return !exists;
        }

        public Boolean Update(BasicSerial model)
        {

            var strSql = new StringBuilder();
            strSql.Append("UPDATE [BasicSerial] SET ");
            strSql.Append(" [Name]=@Name, ");
            strSql.Append(" [ManufacturerId]=@ManufacturerId, ");
            strSql.Append(" [UpdateTime]=@UpdateTime, ");
            strSql.Append(" [IsRemoved]=@IsRemoved ");
            strSql.Append(" where ID=@ID and CompanyId = @CompanyId ");
            SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
                    new SqlParameter("@ManufacturerId",SqlDbType.Int){Value = model.ManufacturerId}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime}, 
                    new SqlParameter("@IsRemoved",SqlDbType.Int){Value = model.IsRemoved}, 
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId},
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        public DataSet GetLists(string strWhere)
        {
            return null;
        }

        public List<BasicSerial> GetSerialsByMasterBrandId(int companyId, int id)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT  [CompanyId] ,[Id] ,[Name] ,[ManufacturerId] ,[CreateTime] ,[UpdateTime] ,[IsRemoved] ");
            strSql.Append(" FROM    [CarsDataAutoHome].[dbo].[BasicSerial] ");
            strSql.Append(" WHERE   IsRemoved = 0 ");
            strSql.Append(" AND CompanyId = @CompanyId ");
            strSql.Append(" AND ID IN ( SELECT  serialId ");
            strSql.Append(" FROM    dbo.BasicMasterBrandJoinSerial ");
            strSql.Append(" WHERE   CompanyId = @CompanyId ");
            strSql.Append(" AND MasterBrandId = @MasterBrandId ) ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value =companyId},
                        new SqlParameter("@MasterBrandId",SqlDbType.Int){Value =id}
                                        };
            var ds = DbHelperSql.Query(strSql.ToString(), parameters);
            return DsToList(ds);
        }

        public List<BasicSerial> GetSerialsByMakeId(int companyId, int makeid)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT [CompanyId] ,[Id] ,[Name],[ManufacturerId],[CreateTime],[UpdateTime],[IsRemoved] ");
            strSql.Append("   FROM [BasicSerial]  ");
            strSql.Append("  WHERE IsRemoved=0 AND CompanyId=@CompanyId AND ManufacturerId=@ManufacturerId ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value =companyId},
                        new SqlParameter("@ManufacturerId",SqlDbType.Int){Value =makeid}
                                        };
            var ds = DbHelperSql.Query(strSql.ToString(), parameters);
            return DsToList(ds);
        }

        private List<BasicSerial> DsToList(DataSet ds)
        {
            var lists = new List<BasicSerial>();
            if (ds != null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var make = new BasicSerial
                    {
                        ID = Int32.Parse(row["Id"].ToString()),
                        Name = row["Name"] == null ? "" : row["Name"].ToString(),
                        CompanyId = Int32.Parse(row["CompanyId"].ToString()),
                        ManufacturerId = Int32.Parse(row["ManufacturerId"].ToString()),
                        CreateTime = DateTime.Parse(row["CreateTime"].ToString()),
                        UpdateTime = DateTime.Parse(row["UpdateTime"].ToString()),
                        IsRemoved = ((bool)row["IsRemoved"]) ? 1 : 0
                    };
                    lists.Add(make);
                }
            }
            return lists;
        }


        public BasicSerial GetSerialById(int companyId, int serialId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT [CompanyId] ,[Id] ,[Name],[ManufacturerId],[CreateTime],[UpdateTime],[IsRemoved] ");
            strSql.Append("   FROM [BasicSerial]  ");
            strSql.Append("  WHERE IsRemoved=0 AND CompanyId=@CompanyId AND Id=@Id ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value =companyId},
                        new SqlParameter("@Id",SqlDbType.Int){Value =serialId}
                                        };
            var ds = DbHelperSql.Query(strSql.ToString(), parameters);
            var lists = DsToList(ds);
            if (lists != null && lists.Count > 0)
            {
                return lists[0];
            }
            return null;
        }

        public List<BasicSerial> GetSerial(int companyId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT [CompanyId] ,[Id] ,[Name],[ManufacturerId],[CreateTime],[UpdateTime],[IsRemoved] ");
            strSql.Append("   FROM [BasicSerial]  ");
            strSql.Append("  WHERE IsRemoved=0 AND CompanyId=@CompanyId ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value =companyId}
                                        };
            var ds = DbHelperSql.Query(strSql.ToString(), parameters);
            var lists = DsToList(ds);
            return lists;
        }

        public BasicMasterBrand GetMasterBrand(int basicSerialId, int companyId)
        {
            BasicMasterBrand mb = null;
            var strSql = new StringBuilder();
            strSql.Append("SELECT mb.CompanyId,mb.Id,mb.Name,mb.CreateTime,mb.UpdateTime,mb.IsRemoved ");
            strSql.Append("  FROM [BasicMasterBrand] mb  ");
            strSql.Append("  JOIN dbo.BasicMasterBrandJoinSerial bsj ON mb.Id=bsj.MasterBrandId AND mb.CompanyId=bsj.CompanyId ");
            strSql.Append("  WHERE bsj.SerialId=@SerialId AND mb.CompanyId=@CompanyId ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value =companyId},
                        new SqlParameter("@SerialId",SqlDbType.Int){Value = basicSerialId} 
                                        };
            var ds = DbHelperSql.Query(strSql.ToString(), parameters);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                var row = ds.Tables[0].Rows[0];
                mb = new BasicMasterBrand
                {
                    ID = Int32.Parse(row["Id"].ToString()),
                    Name = row["Name"] == null ? "" : row["Name"].ToString(),
                    CompanyId = Int32.Parse(row["CompanyId"].ToString()),
                    CreateTime = DateTime.Parse(row["CreateTime"].ToString()),
                    UpdateTime = DateTime.Parse(row["UpdateTime"].ToString()),
                    IsRemoved = ((bool)row["IsRemoved"]) ? 1 : 0
                };
            }
            return mb;
        }
        /// <summary>
        /// 获取所有车系信息，包括品牌名和主品牌名
        /// </summary>
        /// <returns></returns>
        public List<BasicSerialVm> GetAllSerials(int companyId)
        {
            var sqlStr = @"
SELECT  S.CompanyId ,
        S.Id ,
        S.Name AS SerialName ,
        S.ManufacturerId AS MakeId ,
        S.CreateTime ,
        S.UpdateTime ,
        S.IsRemoved ,
        M.Name AS MakeName ,
        MBJS.MasterBrandId ,
        MB.Name AS MasterBrandName
FROM    [CarsDataAutoHome].[dbo].[BasicSerial] S
        JOIN dbo.BasicMake M ON S.ManufacturerId = M.Id
        JOIN dbo.BasicMasterBrandJoinSerial MBJS ON S.Id = MBJS.SerialId
        JOIN dbo.BasicMasterBrand MB ON MB.Id = MBJS.MasterBrandId
WHERE   S.CompanyId = @CompanyId
        AND S.IsRemoved = 0;
";
            SqlParameter[] parameters = { 
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value =companyId}
                                        };
            var ds = DbHelperSql.Query(sqlStr, parameters);
            var list = new List<BasicSerialVm>();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var vm = new BasicSerialVm
                    {
                        ID = Int32.Parse(row["Id"].ToString()),
                        Name = row["SerialName"] == null ? "" : row["SerialName"].ToString(),
                        CompanyId = Int32.Parse(row["CompanyId"].ToString()),
                        CreateTime = DateTime.Parse(row["CreateTime"].ToString()),
                        UpdateTime = DateTime.Parse(row["UpdateTime"].ToString()),
                        IsRemoved = ((bool)row["IsRemoved"]) ? 1 : 0,
                        MakeName = row["MakeName"] == null ? "" : row["MakeName"].ToString(),
                        MasterBrandId = Int32.Parse(row["MasterBrandId"].ToString()),
                        MasterBrandName = row["MasterBrandName"] == null ? "" : row["MasterBrandName"].ToString(),
                    };
                    list.Add(vm);
                }
            }
            return list;
        }
    }
}
