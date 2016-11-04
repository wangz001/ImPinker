using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using AhModel;

namespace AhDal
{
    /// <summary>
    /// 数据访问类:AHModel
    /// </summary>
    public class AhModelDal
    {
        #region  Method

        
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from AHModel");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)			};
            parameters[0].Value = id;

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Insert(AHModel model)
        {
            var strSql = new StringBuilder();
            strSql.Append("insert into AHModel(");
            strSql.Append("ID,MasterBrandID,ManufacturerID,Initial,ModelName,CreateTime,UpdateTime)");
            strSql.Append(" values (");
            strSql.Append("@ID,@MasterBrandID,@ManufacturerID,@Initial,@ModelName,@CreateTime,@UpdateTime)");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@MasterBrandID", SqlDbType.Int,4){Value = model.MasterBrandID},
					new SqlParameter("@ManufacturerID", SqlDbType.Int,4){Value = model.ManufacturerID},
					new SqlParameter("@Initial", SqlDbType.VarChar,50){Value = model.Initial},
					new SqlParameter("@ModelName", SqlDbType.NVarChar,50){Value = model.ModelName},
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = model.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime} 
                                        };
            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        public bool NeedUpdate(AHModel model)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT Id ");
            sqlStr.Append(" FROM [CarsDataAutoHome].[dbo].[AHModel] ");
            sqlStr.Append(" WHERE id =@ID ");
            sqlStr.Append(" AND ModelName=@Name ");
            sqlStr.Append(" AND IsRemoved=0 ");
            sqlStr.Append(" AND MasterBrandID=@MasterBrandID ");
            sqlStr.Append(" AND ManufacturerID=@ManufacturerID ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@ID",SqlDbType.Int){Value = model.ID},
                        new SqlParameter("@Name",SqlDbType.VarChar){Value = model.ModelName},
                        new SqlParameter("@MasterBrandID",SqlDbType.VarChar){Value = model.MasterBrandID},
                        new SqlParameter("@ManufacturerID",SqlDbType.VarChar){Value = model.ManufacturerID}
                                        };
            bool exists = DbHelperSql.Exists(sqlStr.ToString(), parameters);
            return !exists;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(AHModel model)
        {
            var strSql = new StringBuilder();
            strSql.Append("update AHModel set ");
            strSql.Append("MasterBrandID=@MasterBrandID,");
            strSql.Append("ManufacturerID=@ManufacturerID,");
            strSql.Append("Initial=@Initial,");
            strSql.Append("ModelName=@ModelName, ");
            strSql.Append("[UpdateTime]=@UpdateTime, ");
            strSql.Append("[IsRemoved]=@IsRemoved ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@MasterBrandID", SqlDbType.Int,4){Value = model.MasterBrandID},
					new SqlParameter("@ManufacturerID", SqlDbType.Int,4){Value = model.ManufacturerID},
					new SqlParameter("@Initial", SqlDbType.VarChar,50){Value = model.Initial},
					new SqlParameter("@ModelName", SqlDbType.NVarChar,50){Value = model.ModelName},
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime}, 
                    new SqlParameter("@IsRemoved",SqlDbType.Int){Value = model.IsRemoved}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        /// <summary>
        /// 统计每天增加的车系个数
        /// </summary>
        /// <param name="time"></param>
        public int CountAddEveryday(DateTime time)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT  COUNT(1)");
            sqlStr.Append(" FROM    [CarsDataAutoHome].[dbo].[AHModel]  ");
            sqlStr.Append(" WHERE   CreateTime BETWEEN @time AND @time2 ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@time",SqlDbType.DateTime){Value = time.ToShortDateString()} ,
                new SqlParameter("@time2",SqlDbType.DateTime){Value = time.AddDays(1).ToLongTimeString()}
            };
            return (int)DbHelperSql.GetSingle(sqlStr.ToString(), parameters);
        }

        /// <summary>
        /// 统计每天更新的车系个数
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int CountUpdateEveryday(DateTime time)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT  COUNT(1)");
            sqlStr.Append(" FROM    [CarsDataAutoHome].[dbo].[AHModel] ");
            sqlStr.Append(" WHERE   UpdateTime BETWEEN @time1 AND @time2 ");
            sqlStr.Append(" AND IsRemoved=0 ");
            sqlStr.Append(" AND CreateTime!=UpdateTime ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@time1",SqlDbType.DateTime){Value = time.ToShortDateString()} ,
                new SqlParameter("@time2",SqlDbType.DateTime){Value = time.AddDays(1).ToShortDateString()} 
            };
            return (int)DbHelperSql.GetSingle(sqlStr.ToString(), parameters);
        }


        /// <summary>
        /// 批量删除数据
        /// </summary>
        public int DeleteList(string IDlist)
        {
            string sqlStr = "UPDATE AHModel  SET [IsRemoved] = 1 where ID in ("+IDlist+")";
            
            int rows = DbHelperSql.ExecuteSql(sqlStr);
            return rows;
        }

        /// <summary>
        /// 获取增加的车型详细信息 邮件附件使用
        /// </summary>
        public DataTable GetDetailAddModelInfo(DateTime dateTime)
        {
            IDataParameter[] parameters =
            {
                new SqlParameter("@startTime", SqlDbType.DateTime) {Value = dateTime.ToShortDateString()},
                new SqlParameter("@endTime", SqlDbType.DateTime) {Value = dateTime.AddDays(1).ToShortDateString()}
            };
            var dataTable = DbHelperSql.RunProcedure("GetDetailAddModelInfo", parameters, "ds").Tables[0];
            return dataTable;
        }

        /// <summary>
        /// 获取编辑的车型详细信息 邮件附件使用
        /// </summary>
        public DataTable GetDetailEditModelInfo(DateTime dateTime)
        {
            IDataParameter[] parameters =
            {
                new SqlParameter("@startTime", SqlDbType.DateTime) {Value = dateTime.ToShortDateString()},
                new SqlParameter("@endTime", SqlDbType.DateTime) {Value = dateTime.AddDays(1).ToShortDateString()}
            };
            var dataTable = DbHelperSql.RunProcedure("GetDetailEditModelInfo", parameters, "ds").Tables[0];
            return dataTable;
        }

        
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,MasterBrandID,ManufacturerID,Initial,ModelName ");
            strSql.Append(" FROM AHModel ");
            strSql.Append("  WHERE IsRemoved=0  ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" AND " + strWhere);
            }
            return DbHelperSql.Query(strSql.ToString());
        }

        public List<AHModel> GetAllAhModels()
        {
            var ds = GetList("");
            return DsToList(ds);
        }

        public List<AHModel> GetAhModels(int manufacturerId)
        {
            var strWhere = "";
            if (manufacturerId>0)
            {
                strWhere = "ManufacturerID=" + manufacturerId;
            }
            var ds = GetList(strWhere);
            
            return DsToList(ds);
        }

        private List<AHModel> DsToList(DataSet ds)
        {
            var ahModels = new List<AHModel>();
            if (ds != null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = new AHModel
                    {
                        ID = Int32.Parse(row["Id"].ToString()),
                        MasterBrandID = Int32.Parse(row["MasterBrandID"].ToString()),
                        ManufacturerID = Int32.Parse(row["ManufacturerID"].ToString()),
                        Initial = row["Initial"] == null ? "" : row["Initial"].ToString(),
                        ModelName = row["ModelName"] == null ? "" : row["ModelName"].ToString()
                    };
                    ahModels.Add(model);
                }
            }
            return ahModels;
        }

       
        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM AHModel ");
            strSql.Append(" WHERE IsRemoved=0  ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" AND " + strWhere);
            }
            object obj = DbHelperSql.GetSingle(strSql.ToString());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }
        
        #endregion  Method
    }
}

