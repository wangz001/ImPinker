using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using AhModel;

namespace AhDal
{
    /// <summary>
    /// 数据访问类:AHMake
    /// </summary>
    public class AHManufacturerDal
    {
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            const string sqlStr = @"
 SELECT count(1) FROM AHManufacturer WHERE ID = @ID  
";

            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)
                                        };
            parameters[0].Value = id;

            return DbHelperSql.Exists(sqlStr, parameters);
        }


        public bool Insert(AhManufacturer manufacturer)
        {
            const string sqlStr = @"
insert into AHManufacturer(  
 ID,Initial,ManufacturerName,CreateTime,UpdateTime) 
 values ( 
 @ID,@Initial,@ManufacturerName,@CreateTime,@UpdateTime) 
";
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = manufacturer.ID},
					new SqlParameter("@Initial", SqlDbType.VarChar,50){Value = manufacturer.Initial},
					new SqlParameter("@ManufacturerName", SqlDbType.NVarChar,50){Value = manufacturer.ManufacturerName},
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = manufacturer.CreateTime},
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = manufacturer.UpdateTime} };

            int rows = DbHelperSql.ExecuteSql(sqlStr, parameters);
            return rows > 0;
        }

        public bool NeedUpdate(AhManufacturer manufacturer)
        {
            const string sqlStr = @"
SELECT Id 
 FROM AHManufacturer
 WHERE id =@ID 
 AND ManufacturerName=@ManufacturerName 
 AND IsRemoved=0
";

            SqlParameter[] parameters = { 
                        new SqlParameter("@ID",SqlDbType.Int){Value = manufacturer.ID},
                        new SqlParameter("@ManufacturerName",SqlDbType.VarChar){Value = manufacturer.ManufacturerName}
                                        };
            bool exists = DbHelperSql.Exists(sqlStr, parameters);
            return !exists;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(AhManufacturer manufacturer)
        {
            var strSql = new StringBuilder();
            strSql.Append("update AHManufacturer set ");
            strSql.Append("Initial=@Initial,");
            strSql.Append("ManufacturerName=@ManufacturerName,");
            strSql.Append("UpdateTime=@UpdateTime, ");
            strSql.Append("IsRemoved=@IsRemoved  ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@Initial", SqlDbType.VarChar,50){Value = manufacturer.Initial},
					new SqlParameter("@ManufacturerName", SqlDbType.NVarChar,50){Value = manufacturer.ManufacturerName},
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = manufacturer.UpdateTime}, 
                    new SqlParameter("@IsRemoved",SqlDbType.Int){Value = manufacturer.IsRemoved}, 
                    new SqlParameter("@ID",SqlDbType.Int,4){Value = manufacturer.ID} };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;
        }


        /// <summary>
        /// 批量删除数据
        /// </summary>
        public int DeleteList(string IDlist)
        {
            string sqlStr = "UPDATE AHManufacturer  SET IsRemoved = 1 where ID in (" + IDlist + ")";

            int rows = DbHelperSql.ExecuteSql(sqlStr);
            return rows;
        }


        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            var strSql = new StringBuilder();
            strSql.Append("select ID,Initial,ManufacturerName");
            strSql.Append(" FROM AHManufacturer ");
            strSql.Append("  WHERE IsRemoved=0 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" AND " + strWhere);
            }
            return DbHelperSql.Query(strSql.ToString());
        }

        public List<AhManufacturer> GetAllAhManufacturers()
        {
            var ds = GetList("");
            return DsToList(ds);
        }


        private List<AhManufacturer> DsToList(DataSet ds)
        {
            var lists = new List<AhManufacturer>();
            if (ds != null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var manufacturer = new AhManufacturer
                    {
                        ID = Int32.Parse(row["Id"].ToString()),
                        Initial = row["Initial"] == null ? "" : row["Initial"].ToString(),
                        ManufacturerName = row["ManufacturerName"] == null ? "" : row["ManufacturerName"].ToString()
                    };
                    lists.Add(manufacturer);
                }
            }
            return lists;
        }


        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM AHManufacturer ");
            strSql.Append("  WHERE IsRemoved=0  ");
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

        public AhManufacturer GetManufacturer(int id)
        {
            string sqlStr = "id =" + id;
            var ds = GetList(sqlStr);
            if (DsToList(ds) != null)
            {
                return DsToList(ds)[0];
            }
            return null;
        }
    }
}

