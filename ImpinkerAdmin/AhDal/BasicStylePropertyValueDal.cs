using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AhModel;

namespace AhDal
{
    public class BasicStylePropertyValueDal
    {
        public Boolean IsExit(BasicStyle model)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from BasicStyle");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)			};
            parameters[0].Value = model.ID;

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        public Boolean Insert(BasicStyle model)
        {
            var strSql = new StringBuilder();
            strSql.Append("INSERT INTO [BasicStyle]");
            strSql.Append(" ([Id],[Name],SerialId,Year,Price,SaleStatus,CreateTime,UpdateTime ) ");
            strSql.Append(" VALUES ");
            strSql.Append(" (@ID,@Name,@SerialId,@Year,@Price,@SaleStatus,@CreateTime,@UpdateTime) ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
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
            sqlStr.Append(" AND Name=@Name ");
            sqlStr.Append(" AND SerialId=@SerialId ");
            sqlStr.Append(" AND SaleStatus=@SaleStatus ");
            sqlStr.Append(" AND Year=@Year ");
            sqlStr.Append(" AND Price=@Price ");
            sqlStr.Append(" AND IsRemoved=0 ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@ID",SqlDbType.Int){Value = model.ID},
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
            strSql.Append(" [UpdateTime]=@UpdateTime, ");
            strSql.Append(" [IsRemoved]=@IsRemoved ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@Name", SqlDbType.NVarChar,50){Value = model.Name},
                    new SqlParameter("@SerialId",SqlDbType.Int){Value = model.SerialId}, 
                    new SqlParameter("@Year",SqlDbType.VarChar){Value = model.Year}, 
                    new SqlParameter("@SaleStatus",SqlDbType.VarChar){Value = model.SaleStatus}, 
                    new SqlParameter("@Price",SqlDbType.Decimal){Value = model.Price}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime}, 
                    new SqlParameter("@IsRemoved",SqlDbType.Int){Value = model.IsRemoved}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }


        /// <summary>
        /// 用标志参数方式插入数据
        /// </summary>
        /// <param name="dt"></param>
        public void InitWithTvp(DataTable dt)
        {
            IDataParameter[] sqlParameter = { new SqlParameter("@TVP", dt) };
            int count;
            DbHelperSql.RunProcedure("InitBasicStylePropertyValue", sqlParameter, out count);
        }

        /// <summary>
        /// 获取和易车匹配的车型参配值
        /// </summary>
        /// <param name="basicStyleId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public DataSet GetPropertyValues(int basicStyleId, int companyId)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT  PM.BitPropertyId , BSV.[Id] , BSV.[CompanyId] , BSV.[PropertyId] , ");
            sqlStr.Append(" BSV.[StyleId] , BSV.[Value] , BSV.[CreateTime] , BSV.[UpdateTime],sp.Name ");
            sqlStr.Append(" FROM    [BasicStylePropertyValue] BSV ");
            sqlStr.Append(" JOIN dbo.BasicPropertyMap PM ON PM.ComParePropertyId = BSV.PropertyId AND BSV.CompanyId = PM.CompanyId ");
            sqlStr.Append(" JOIN dbo.BasicStyleProperty SP ON SP.Id=PM.ComParePropertyId AND SP.CompanyId=PM.CompanyId ");
            sqlStr.Append(" WHERE BSV.StyleId=@StyleId AND BSV.CompanyId=@CompanyId ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@StyleId",SqlDbType.Int){Value =basicStyleId },
                 new SqlParameter("@CompanyId",SqlDbType.Int){Value = companyId} 
            };
            var ds = DbHelperSql.Query(sqlStr.ToString(), parameters);

            return ds;
        }
    }
}
