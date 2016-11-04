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
    public class AHMfJoinMbDal
    {
        #region  Method

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int masterBrandId,int manufacturerId)
        {
            const string sqlStr = @"
 SELECT ID FROM AHMfJoinMb WHERE ManufacturerID = @ManufacturerID AND MasterBrandID = @MasterBrandID 
";
            SqlParameter[] parameters = {
					new SqlParameter("@ManufacturerID", SqlDbType.Int){Value =manufacturerId},
                    new SqlParameter("@MasterBrandID",SqlDbType.Int){Value = masterBrandId} 
                                        };
            return DbHelperSql.Exists(sqlStr, parameters);
        }


        public bool Insert(AHMfJoinMb mfJoinMb)
        {
            const string sqlStr = @"
insert into AHMfJoinMb(  
 ManufacturerID,MasterBrandID,CreateTime,UpdateTime) 
 values ( 
 @ManufacturerID,@MasterBrandID,@CreateTime,@UpdateTime) 
";
            SqlParameter[] parameters = {
					new SqlParameter("@ManufacturerID", SqlDbType.Int,4){Value = mfJoinMb.ManufacturerID},
                    new SqlParameter("@MasterBrandID",SqlDbType.Int,4){Value = mfJoinMb.MasterBrandID},
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = mfJoinMb.CreateTime},
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = mfJoinMb.UpdateTime} };

            int rows = DbHelperSql.ExecuteSql(sqlStr, parameters);
            return rows > 0;
        }

       
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            var strSql = new StringBuilder();
            strSql.Append("select ID,ManufacturerID,MasterBrandID ");
            strSql.Append(" FROM AHMfJoinMb ");
            strSql.Append("  WHERE 1=1 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" AND " + strWhere);
            }
            return DbHelperSql.Query(strSql.ToString());
        }

        public List<AHMfJoinMb> GetAHMfJoinMbs(int masterBrandId)
        {
            var strWhere = "";
            if (masterBrandId > 0)
            {
                strWhere = "MasterBrandID= " + masterBrandId;
            }
            var ds = GetList(strWhere);

            return DsToList(ds);
        }

        private List<AHMfJoinMb> DsToList(DataSet ds)
        {
            var lists = new List<AHMfJoinMb>();
            if (ds != null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var mrow = new AHMfJoinMb
                    {
                        ID = Int32.Parse(row["Id"].ToString()),
                        ManufacturerID = Int32.Parse(row["ManufacturerID"].ToString()),
                        MasterBrandID = Int32.Parse(row["MasterBrandID"].ToString())
                    };
                    lists.Add(mrow);
                }
            }
            return lists;
        }

        #endregion  Method

    }
}

