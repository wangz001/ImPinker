using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using AhModel;

namespace AhDal
{
    /// <summary>
    /// 数据访问类:AHStyle
    /// </summary>
    public class AhStyleDal
    {

        #region  Method

        
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            var strSql = new StringBuilder();
            strSql.Append("select count(1) from AHStyle");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@ID", SqlDbType.Int, 4)
            };
            parameters[0].Value = id;

            return DbHelperSql.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Insert(AhStyle model)
        {
            var strSql = new StringBuilder();
            strSql.Append("insert into AHStyle(");
            strSql.Append("ID,ModelID,StyleName,SaleStatus,Price,Year,CreateTime,UpdateTime)");
            strSql.Append(" values (");
            strSql.Append("@ID,@ModelID,@StyleName,@SaleStatus,@Price,@Year,@CreateTime,@UpdateTime)");
            SqlParameter[] parameters = {
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID},
					new SqlParameter("@ModelID", SqlDbType.Int,4){Value = model.ModelID},
					new SqlParameter("@StyleName", SqlDbType.NVarChar,50){Value = model.StyleName},
					new SqlParameter("@SaleStatus", SqlDbType.NVarChar,50){Value = model.SaleStatus},
					new SqlParameter("@Price", SqlDbType.Decimal){Value = model.Price},
                    new SqlParameter("@Year",SqlDbType.VarChar,50){Value = model.Year},
                    new SqlParameter("@CreateTime",SqlDbType.DateTime){Value = model.CreateTime}, 
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime} };

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);

            return rows > 0;

        }

        public bool NeedUpdate(AhStyle ahStyle)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT Id ");
            sqlStr.Append(" FROM [CarsDataAutoHome].[dbo].[AHStyle] ");
            sqlStr.Append(" WHERE id =@ID ");
            sqlStr.Append(" AND StyleName=@Name ");
            sqlStr.Append(" AND ModelID=@ModelID ");
            sqlStr.Append(" AND SaleStatus=@SaleStatus ");
            sqlStr.Append(" AND Price=@Price ");
            sqlStr.Append(" AND Year=@Year ");
            sqlStr.Append(" AND IsRemoved=0 ");
            SqlParameter[] parameters = { 
                        new SqlParameter("@ID",SqlDbType.Int){Value = ahStyle.ID},
                        new SqlParameter("@Name",SqlDbType.VarChar){Value = ahStyle.StyleName},
                        new SqlParameter("@ModelID",SqlDbType.VarChar){Value = ahStyle.ModelID},
                        new SqlParameter("@SaleStatus",SqlDbType.VarChar){Value = ahStyle.SaleStatus},
                        new SqlParameter("@Price",SqlDbType.VarChar){Value = ahStyle.Price},
                        new SqlParameter("@Year",SqlDbType.VarChar){Value = ahStyle.Year}
                                        };
            bool exists = DbHelperSql.Exists(sqlStr.ToString(), parameters);
            return !exists;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(AhStyle model)
        {
            var strSql = new StringBuilder();
            strSql.Append("update AHStyle set ");
            strSql.Append("ModelID=@ModelID,");
            strSql.Append("StyleName=@StyleName,");
            strSql.Append("SaleStatus=@SaleStatus,");
            strSql.Append("Price=@Price,");
            strSql.Append("Year=@Year, ");
            strSql.Append("[UpdateTime]=@UpdateTime, ");
            strSql.Append("[IsRemoved]=@IsRemoved ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
					new SqlParameter("@ModelID", SqlDbType.Int,4){Value = model.ModelID},
					new SqlParameter("@StyleName", SqlDbType.NVarChar,50){Value = model.StyleName},
					new SqlParameter("@SaleStatus", SqlDbType.NVarChar,50){Value = model.SaleStatus},
					new SqlParameter("@Price", SqlDbType.Decimal){Value = model.Price},
                    new SqlParameter("@Year",SqlDbType.VarChar,50){Value = model.Year},
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = model.UpdateTime}, 
                    new SqlParameter("@IsRemoved",SqlDbType.Int){Value = model.IsRemoved}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = model.ID}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }

        /// <summary>
        /// 统计每天增加的车型个数
        /// </summary>
        /// <param name="time"></param>
        public int CountAddEveryday(DateTime time)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT  COUNT(1)");
            sqlStr.Append(" FROM    [CarsDataAutoHome].[dbo].[AHStyle] ");
            sqlStr.Append(" WHERE   CreateTime BETWEEN @time AND @time2 ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@time",SqlDbType.DateTime){Value = time.ToShortDateString()} ,
                new SqlParameter("@time2",SqlDbType.DateTime){Value = time.AddDays(1).ToShortDateString()} 
            };
            return (int)DbHelperSql.GetSingle(sqlStr.ToString(), parameters);
        }

        /// <summary>
        /// 获取增加的车型详细信息 邮件附件使用
        /// </summary>
        public DataTable GetDetailAddStyleInfo(DateTime dateTime)
        {
            IDataParameter[] parameters =
            {
                new SqlParameter("@startTime", SqlDbType.DateTime) {Value = dateTime.ToShortDateString()},
                new SqlParameter("@endTime", SqlDbType.DateTime) {Value = dateTime.AddDays(1).ToShortDateString()}
            };
            var dataTable = DbHelperSql.RunProcedure("GetDetailAddStyleInfo", parameters, "ds").Tables[0];
            return dataTable;
        }

        /// <summary>
        /// 获取编辑的车型详细信息 邮件附件使用
        /// </summary>
        public DataTable GetDetailEditStyleInfo(DateTime dateTime)
        {
            IDataParameter[] parameters =
            {
                new SqlParameter("@startTime", SqlDbType.DateTime) {Value = dateTime.ToShortDateString()},
                new SqlParameter("@endTime", SqlDbType.DateTime) {Value = dateTime.AddDays(1).ToShortDateString()}
            };
            var dataTable = DbHelperSql.RunProcedure("GetDetailEditStyleInfo", parameters, "ds").Tables[0];
            return dataTable;
        }

        /// <summary>
        /// 统计每天更新的车型个数
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int CountUpdateEveryday(DateTime time)
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append("SELECT  COUNT(1)");
            sqlStr.Append(" FROM    [CarsDataAutoHome].[dbo].[AHStyle] ");
            sqlStr.Append(" WHERE   UpdateTime BETWEEN @time1 AND @time2  ");
            sqlStr.Append(" AND CreateTime!=UpdateTime ");
            sqlStr.Append(" AND IsRemoved=0 ");
            SqlParameter[] parameters =
            {
                new SqlParameter("@time1",SqlDbType.DateTime){Value = time.ToShortDateString()} ,
                new SqlParameter("@time2",SqlDbType.DateTime){Value = time.AddDays(1).ToShortDateString()} 
            };
            return (int)DbHelperSql.GetSingle(sqlStr.ToString(), parameters);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            var strSql = new StringBuilder();
            strSql.Append("select ID,ModelID,StyleName,SaleStatus,Price,Year ");
            strSql.Append(" FROM AHStyle ");
            strSql.Append(" WHERE IsRemoved=0 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" AND " + strWhere);
            }
            return DbHelperSql.Query(strSql.ToString());
        }

       
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public int DeleteList(string IDlist)
        {
            string sqlStr = "UPDATE AHStyle  SET [IsRemoved] = 1 where ID in ("+IDlist+")";
           
            int rows = DbHelperSql.ExecuteSql(sqlStr);
            return rows ;
        }


        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM AHStyle ");
            strSql.Append(" WHERE IsRemoved=0 ");
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
        
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(int masterBrandId, int manufacturerId, int modelId, int year,
            string styleName, int startIndex, int endIndex)
        {
            var strSql = new StringBuilder();
            strSql.Append("SELECT  * ");
            strSql.Append(" FROM    ( SELECT    ROW_NUMBER() OVER ( ORDER BY T.ID DESC ) AS Row ,");
            strSql.Append(" T.*,MO.ModelName,MA.ManufacturerName ");
            strSql.Append(" FROM      AHStyle T ");
            strSql.Append(" JOIN dbo.AHModel MO ON T.ModelID=MO.ID ");
            strSql.Append(" JOIN dbo.AHManufacturer MA ON MO.ManufacturerID=MA.ID ");
            strSql.Append(" JOIN dbo.AHMasterBrand MB ON mo.MasterBrandID=MB.Id  ");

            strSql.Append(" WHERE    T.IsRemoved=0 ");
            if (masterBrandId > 0)
            {
                strSql.Append(" AND MB.ID= "+masterBrandId);
            }
            if (manufacturerId>0)
            {
                strSql.Append(" AND MO.ManufacturerID= " + manufacturerId);
            }
            if (modelId>0)
            {
                strSql.Append(" AND MO.ID= "+modelId);
            }
            if (year>0)
            {
                strSql.Append(" AND T.Year= "+year);
            }
            if (!string.IsNullOrEmpty(styleName))
            {
                strSql.Append(string.Format(" AND T.StyleName LIKE '%{0}%'",styleName));
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperSql.Query(strSql.ToString());
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(int masterBrandId, int manufacturerId, int modelId, int year, string styleName)
        {
            var strSql = new StringBuilder();
            strSql.Append(" SELECT  ");
            strSql.Append(" COUNT(1) ");
            strSql.Append(" FROM      AHStyle T ");
            strSql.Append(" JOIN dbo.AHModel MO ON T.ModelID=MO.ID ");
            strSql.Append(" JOIN dbo.AHManufacturer MA ON MO.ManufacturerID=MA.ID ");
            strSql.Append(" JOIN dbo.AHMasterBrand MB ON mo.MasterBrandID=MB.Id  ");
            strSql.Append(" WHERE   T.IsRemoved=0 ");
            if (masterBrandId > 0)
            {
                strSql.Append(" AND MB.ID= " + masterBrandId);
            }
            if (manufacturerId > 0)
            {
                strSql.Append(" AND MO.ManufacturerID= " + manufacturerId);
            }
            if (modelId > 0)
            {
                strSql.Append(" AND MO.ID= " + modelId);
            }
            if (year > 0)
            {
                strSql.Append(" AND T.Year= " + year);
            }
            if (!string.IsNullOrEmpty(styleName))
            {
                strSql.Append(string.Format(" AND T.StyleName LIKE '%{0}%'", styleName));
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

        /// <summary>
        /// 获取车系下所含的年款
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        public List<int> GetYearsByModelId(int modelId)
        {
            var years=new List<int>();
            var sqlStr=new StringBuilder();
            sqlStr.Append("SELECT [Year]");
            sqlStr.Append("  FROM [CarsDataAutoHome].[dbo].[AHStyle]");
            sqlStr.Append("  WHERE ModelID="+modelId);
            sqlStr.Append("  GROUP BY Year");
            sqlStr.Append("  ORDER BY Year DESC");
            var ds = DbHelperSql.Query(sqlStr.ToString());
            if (ds!=null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var year = row["Year"];
                    if (year!=null)
                    {
                        years.Add(Int32.Parse(year.ToString()));
                    }
                }
            }
            return years;
        }

        #endregion  Method

        public DataSet GetModelListNotHavePropertyValue()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM dbo.AHStyle WHERE ID NOT IN ( ");
            strSql.Append(" ( SELECT StyleID ");
            strSql.Append(" FROM [CarsDataAutoHome].[dbo].[AHStylePropertyValue] ");
            strSql.Append(" GROUP BY StyleID) ");
            strSql.Append(" ) ");
            return DbHelperSql.Query(strSql.ToString());
        }


        public List<AhStyle> GetAhStyles(int modelId)
        {
            var strWhere=" ModelID= " + modelId;
            var ds =GetList(strWhere);
            return DsToList(ds);
        }

        public List<AhStyle> GetLists(string strWhere)
        {
            var ds = GetList(strWhere);

            return DsToList(ds);
        }

        private List<AhStyle> DsToList(DataSet ds)
        {
            var lists=new List<AhStyle>();
            if (ds != null)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var style = new AhStyle();
                    style.ID = Int32.Parse(row["ID"].ToString());
                    style.ModelID = Int32.Parse(row["ModelID"].ToString());
                    style.StyleName = row["StyleName"].ToString();
                    style.SaleStatus = row["SaleStatus"].ToString();
                    style.Year = row["Year"].ToString();
                    lists.Add(style);
                }
            }
            return lists;
        }
        /// <summary>
        /// 更新修改时间。当车型参配值变化的时候修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="now"></param>
        public bool ReplaceUpdateTime(int id, DateTime now)
        {
            var strSql = new StringBuilder();
            strSql.Append("update AHStyle set ");
            strSql.Append("[UpdateTime]=@UpdateTime ");
            strSql.Append(" where ID=@ID ");
            SqlParameter[] parameters = {
                    new SqlParameter("@UpdateTime",SqlDbType.DateTime){Value = now}, 
					new SqlParameter("@ID", SqlDbType.Int,4){Value = id}};

            int rows = DbHelperSql.ExecuteSql(strSql.ToString(), parameters);
            return rows > 0;
        }
    }
}

