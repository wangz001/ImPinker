using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using AhModel;

namespace AhDal
{
    /// <summary>
    /// 数据访问类:AHStylePropertyValue
    /// </summary>
    public partial class AhStylePropertyValueDal
    {
        public AhStylePropertyValueDal()
        { }
        #region  Method

        /// <summary>
        /// 得到最大ID
        /// </summary>
        public int GetMaxId()
        {
            return DbHelperSql.GetMaxId("ID", "AHStylePropertyValue");
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from AHStylePropertyValue");
            strSql.Append(" where ID=@ID");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4)
			};
            parameters[0].Value = ID;

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(AHStylePropertyValue model)
        {
            var strSql = new StringBuilder();
            strSql.Append("insert into AHStylePropertyValue(");
            strSql.Append("PropertyID,StyleID,Value)");
            strSql.Append(" values (");
            strSql.Append("@PropertyID,@StyleID,@Value)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@PropertyID", SqlDbType.Int,4),
					new SqlParameter("@StyleID", SqlDbType.Int,4),
					new SqlParameter("@Value", SqlDbType.NVarChar,100)};
            parameters[0].Value = model.PropertyID;
            parameters[1].Value = model.StyleID;
            parameters[2].Value = model.Value;

            object obj = DbHelperSql.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }





        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,PropertyID,StyleID,Value,[CreateTime],[UpdateTime] ");
            strSql.Append(" FROM AHStylePropertyValue ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSql.Query(strSql.ToString());
        }

        public List<AHStylePropertyValue> GetPropertyValueLists(string strWhere)
        {
            var ds = GetList(strWhere);
            return DsToList(ds);
        }
        /// <summary>
        /// 根据参配值判断车型是否更新
        /// </summary>
        /// <param name="styleId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool CheckStyleUpdate(int styleId, DateTime time)
        {
            const string sqlStr = @"
 select ID
 FROM AHStylePropertyValue 
 where StyleID = @StyleID  AND UpdateTime > @UpdateTime
";
            SqlParameter[] parameters =
            {
                new SqlParameter("@StyleID",SqlDbType.Int){Value = styleId},
                new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = time.ToShortDateString()} 
            };
            return DbHelperSql.Exists(sqlStr, parameters);
        }

        private List<AHStylePropertyValue> DsToList(DataSet ds)
        {
            var lists = new List<AHStylePropertyValue>();

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = new AHStylePropertyValue();

                    model.ID = int.Parse(row["ID"].ToString());
                    model.PropertyID = int.Parse(row["PropertyID"].ToString());
                    model.StyleID = int.Parse(row["StyleID"].ToString());
                    model.Value = row["Value"].ToString();
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                    model.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
                    lists.Add(model);
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
            strSql.Append("select count(1) FROM AHStylePropertyValue ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
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
        /// <summary>
        /// 用标志参数方式插入数据
        /// </summary>
        /// <param name="dt"></param>
        public void InitWithTvp(DataTable dt)
        {
            IDataParameter[] sqlParameter = { new SqlParameter("@TVP", dt) };
            int count;
            DbHelperSql.RunProcedure("InitStylePropertyValueWithTVP", sqlParameter, out count);
        }

        public List<AHStylePropertyValue> GetByStyleId(int ahStyleId)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT [ID],[PropertyID],[StyleID],[Value],[CreateTime],[UpdateTime] ");
            strSql.Append(" FROM [CarsDataAutoHome].[dbo].[AHStylePropertyValue]");
            strSql.Append(" WHERE StyleID=@StyleID ");
            SqlParameter[] parameters = {
					new SqlParameter("@StyleID", SqlDbType.Int){Value = ahStyleId}
			};
            DataSet ds = DbHelperSql.Query(strSql.ToString(), parameters);

            return DsToList(ds);
        }
    }
}

