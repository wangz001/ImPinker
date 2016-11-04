using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using AhModel;

namespace AhDal
{
    public class BasicMasterBrandJoinMakeDal
    {
        public Boolean IsExit(BasicMasterBrandJoinMake model)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from [BasicMasterBrandJoinMake]");
            strSql.Append(" where MasterBrandId=@MasterBrandId  and MakeId = @MakeId and CompanyId = @CompanyId  ");
            SqlParameter[] parameters = {
					new SqlParameter("@MasterBrandId", SqlDbType.Int,4){Value = model.MasterBrandId},
					new SqlParameter("@MakeId", SqlDbType.Int,4){Value = model.MakeId},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId}
                                        };

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean Insert(BasicMasterBrandJoinMake model)
        {
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO [BasicMasterBrandJoinMake]");
            strSql.Append(" (CompanyId,MasterBrandId,MakeId,CreateTime,UpdateTime ) ");
            strSql.Append(" VALUES ");
            strSql.Append(" (@CompanyId,@MasterBrandId,@MakeId,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters = {
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId},
					new SqlParameter("@MasterBrandId", SqlDbType.Int,4){Value = model.MasterBrandId},
                    new SqlParameter("@MakeId",SqlDbType.Int){Value = model.MakeId}, 
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = model.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime} 
                                        };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;
        }

        public bool NeedUpdate(BasicMasterBrandJoinMake model)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT Id ");
            sqlStr.Append(" FROM [BasicMasterBrandJoinMake] ");
            sqlStr.Append(" WHERE MasterBrandId =@MasterBrandId ");
            sqlStr.Append(" AND CompanyId=@CompanyId ");
            sqlStr.Append(" AND MakeId=@MakeId ");
            sqlStr.Append(" AND IsRemoved=0 ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@MasterBrandId",SqlDbType.Int){Value = model.MasterBrandId},
                        new SqlParameter("@CompanyId",SqlDbType.Int){Value = model.CompanyId},
                        new SqlParameter("@MakeId",SqlDbType.Int){Value = model.MakeId} 
                                        };
            bool exists = DbHelperSql.Exists(sqlStr.ToString(), parameters);
            return !exists;
        }

        public Boolean Update(BasicMasterBrandJoinMake model)
        {

            var strSql = new StringBuilder();
            strSql.Append("UPDATE [BasicMasterBrandJoinMake] SET ");
            strSql.Append(" [MakeId]=@MakeId, ");
            strSql.Append(" [UpdateTime]=@UpdateTime, ");
            strSql.Append(" [IsRemoved]=@IsRemoved ");
            strSql.Append(" where MasterBrandId=@MasterBrandId and CompanyId = @CompanyId ");
            SqlParameter[] parameters = {
                    new SqlParameter("@MakeId",SqlDbType.Int){Value = model.MakeId}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime}, 
                    new SqlParameter("@IsRemoved",SqlDbType.Int){Value = model.IsRemoved}, 
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId},
					new SqlParameter("@MasterBrandId", SqlDbType.Int,4){Value = model.MasterBrandId}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        public DataSet GetLists(string strWhere)
        {
            return null;
        }
    }
}
