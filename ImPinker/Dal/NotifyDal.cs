﻿using DBUtility;
using ImModel;
using ImModel.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImDal
{
    /// <summary>
    /// 通知操作
    /// </summary>
    public class NotifyDal
    {
        public long Add(Notify model)
        {
            var strSql = new StringBuilder();
            strSql.Append("insert into Notify(");
            strSql.Append("ContentStr,NotifyType,Target,TargetType,Action,Sender,Receiver,IsRead,CreateTime,UpdateTime)");
            strSql.Append(" values (");
            strSql.Append("@ContentStr,@NotifyType,@Target,@TargetType,@Action,@Sender,@Receiver,@IsRead,@CreateTime,@UpdateTime)");
            strSql.Append(" select IDENT_CURRENT('Notify')");
            SqlParameter[] parameters =
			{
				new SqlParameter("@ContentStr", SqlDbType.NVarChar, 100){Value =model.ContentStr },
				new SqlParameter("@NotifyType", SqlDbType.Int){Value =(int)model.NotifyType },
				new SqlParameter("@Target", SqlDbType.Int){Value =model.Target },
				new SqlParameter("@TargetType", SqlDbType.Int){Value = (int)model.TargetType},
				new SqlParameter("@Action", SqlDbType.Int){Value =(int)model.Action },
				new SqlParameter("@Sender", SqlDbType.Int){Value =model.Sender },
				new SqlParameter("@Receiver", SqlDbType.Int){Value =model.Receiver },
                new SqlParameter("@IsRead",SqlDbType.Bit,1){Value = model.IsRead},
				new SqlParameter("@CreateTime", SqlDbType.DateTime){Value =model.CreateTime },
				new SqlParameter("@UpdateTime", SqlDbType.DateTime){Value = model.UpdateTime}
			};
            object obj = DbHelperSQL.ExecuteScalar(strSql.ToString(), parameters);
            return Convert.ToInt32(obj);
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="isRead"></param>
        /// <returns></returns>
        public int GetNotifyCount(int userid, bool isRead)
        {
            var sql = @"
select Count(1) from  Notify where NotifyType in (1,2) and IsRead=@IsRead and Receiver=@Receiver
";
            SqlParameter[] parameters =
			{
				new SqlParameter("@Receiver", SqlDbType.Int){Value =userid },
				new SqlParameter("@IsRead", SqlDbType.Bit,1){Value =isRead }
			};
            var count = (int)DbHelperSQL.GetSingle(sql, parameters);
            return count;
        }
        /// <summary>
        /// 获取通知列表
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="notifyType"></param>
        /// <param name="isRead"></param>
        /// <returns></returns>
        public List<Notify> GetNotifyList(int userid, ImModel.Enum.NotifyTypeEnum notifyType, bool isRead)
        {
            var sql = @"
SELECT Id
      ,ContentStr
      ,NotifyType
      ,Target
      ,TargetType
      ,Action
      ,Sender
      ,Receiver
      ,IsRead
      ,CreateTime
      ,UpdateTime
  FROM Notify where IsRead=@IsRead and NotifyType=@NotifyType and Receiver=@Receiver
";
            SqlParameter[] parameters =
			{
				new SqlParameter("@Receiver", SqlDbType.Int){Value =userid },
				new SqlParameter("@IsRead", SqlDbType.Bit,1){Value =isRead },
				new SqlParameter("@NotifyType", SqlDbType.Int){Value =(int)notifyType }
			};
            var ds = DbHelperSQL.Query(sql, parameters);
            var list = DsToList(ds);
            return list;
        }

        private List<Notify> DsToList(DataSet ds)
        {
            var list = new List<Notify>();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = new Notify();
                    if (row.Table.Columns.Contains("Id") && row["Id"] != null && row["Id"].ToString() != "")
                    {
                        model.Id = long.Parse(row["Id"].ToString());
                    }
                    if (row.Table.Columns.Contains("ContentStr") && row["ContentStr"] != null)
                    {
                        model.ContentStr = row["ContentStr"].ToString();
                    }
                    if (row.Table.Columns.Contains("NotifyType") && row["NotifyType"] != null)
                    {
                        model.NotifyType = (NotifyTypeEnum)int.Parse(row["NotifyType"].ToString());
                    }
                    if (row.Table.Columns.Contains("TargetType") && row["TargetType"] != null)
                    {
                        model.TargetType = (TargetTypeEnum)int.Parse(row["TargetType"].ToString());
                    }
                    if (row.Table.Columns.Contains("Target") && row["Target"] != null && row["Target"].ToString() != "")
                    {
                        model.Target = int.Parse(row["Target"].ToString());
                    }
                    if (row.Table.Columns.Contains("Action") && row["Action"] != null)
                    {
                        model.Action = (ActionEnum)int.Parse(row["Action"].ToString());
                    }
                    if (row.Table.Columns.Contains("Sender") && row["Sender"] != null && row["Sender"].ToString() != "")
                    {
                        model.Sender = int.Parse(row["Sender"].ToString());
                    }
                    if (row.Table.Columns.Contains("Receiver") && row["Receiver"] != null && row["Receiver"].ToString() != "")
                    {
                        model.Receiver = int.Parse(row["Receiver"].ToString());
                    }
                    if (row.Table.Columns.Contains("IsRead") && row["IsRead"] != null && row["IsRead"].ToString() != "")
                    {
                        model.IsRead = bool.Parse(row["IsRead"].ToString());
                    }
                    if (row.Table.Columns.Contains("CreateTime") && row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                    {
                        model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                    }
                    if (row.Table.Columns.Contains("UpdateTime") && row["UpdateTime"] != null && row["UpdateTime"].ToString() != "")
                    {
                        model.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
                    }
                    list.Add(model);
                }
            }
            return list;
        }
    }
}
