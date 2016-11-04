using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using AhModel;

namespace AhDal
{
    public class BasicMasterBrandJoinSerialDal
    {
        public Boolean IsExit(BasicMasterBrandJoinSerial model)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from [BasicMasterBrandJoinSerial]");
            strSql.Append(" where MasterBrandId=@MasterBrandId and SerialId = @SerialId and CompanyId = @CompanyId  ");
            SqlParameter[] parameters = {
					new SqlParameter("@MasterBrandId", SqlDbType.Int,4){Value = model.MasterBrandId},
					new SqlParameter("@SerialId", SqlDbType.Int,4){Value = model.SerialId},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId}
                                        };

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean Insert(BasicMasterBrandJoinSerial model)
        {
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO [BasicMasterBrandJoinSerial]");
            strSql.Append(" (CompanyId,MasterBrandId,SerialId,CreateTime,UpdateTime ) ");
            strSql.Append(" VALUES ");
            strSql.Append(" (@CompanyId,@MasterBrandId,@SerialId,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters = {
					new SqlParameter("@MasterBrandId", SqlDbType.Int,4){Value = model.MasterBrandId},
					new SqlParameter("@CompanyId", SqlDbType.Int,4){Value = model.CompanyId},
                    new SqlParameter("@SerialId",SqlDbType.Int){Value = model.SerialId}, 
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = model.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime} 
                                        };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;
        }

       
        public Boolean Update(BasicMasterBrandJoinSerial model)
        {

            var strSql = new StringBuilder();
            strSql.Append("UPDATE [BasicMasterBrandJoinSerial] SET ");
            strSql.Append(" [SerialId]=@SerialId, ");
            strSql.Append(" [UpdateTime]=@UpdateTime, ");
            strSql.Append(" [IsRemoved]=@IsRemoved ");
            strSql.Append(" where MasterBrandId=@MasterBrandId and CompanyId = @CompanyId ");
            SqlParameter[] parameters = {
                    new SqlParameter("@SerialId",SqlDbType.Int){Value = model.SerialId}, 
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
