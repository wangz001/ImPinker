using System.Data;
using ImDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImModel;
using ImModel.Enum;

namespace ImBLL
{
    public class WeiBoBll
    {
        private WeiBoDal weiBoDal = new WeiBoDal();

        public bool AddWeiBo(ImModel.WeiBo model)
        {
            var flag=weiBoDal.AddWeiBo(model);
            return flag;
        }
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<WeiBo> GetListByPage(int pageindex, int pagesize)
        {
            var resultList = new List<WeiBo>();
            var ds= weiBoDal.GetListByPage(pageindex, pagesize);
            if (ds!=null&&ds.Tables[0].Rows.Count>0)
            {
                resultList.AddRange(from DataRow dataRow in ds.Tables[0].Rows select DataRowToModel(dataRow));
            }
            return resultList;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public WeiBo DataRowToModel(DataRow row)
        {
            var model = new WeiBo();
            if (row != null)
            {
                if (row.Table.Columns.Contains("Id") && row["Id"] != null && row["Id"].ToString() != "")
                {
                    model.Id = long.Parse(row["Id"].ToString());
                }
                if (row.Table.Columns.Contains("UserId") && row["UserId"] != null && row["UserId"].ToString() != "")
                {
                    model.UserId = Int32.Parse(row["UserId"].ToString());
                }
                if (row.Table.Columns.Contains("Description") && row["Description"] != null)
                {
                    model.Description = row["Description"].ToString();
                }
                if (row.Table.Columns.Contains("ContentValue") && row["ContentValue"] != null)
                {
                    model.ContentValue = row["ContentValue"].ToString();
                }
                if (row.Table.Columns.Contains("ContentType") && row["ContentType"] != null && row["ContentType"].ToString() != "")
                {
                    model.ContentType = (WeiBoContentTypeEnum)int.Parse(row["ContentType"].ToString());
                }
                if (row.Table.Columns.Contains("LocationText") && row["LocationText"] != null)
                {
                    model.LocationText = row["LocationText"].ToString();
                }
                if (row.Table.Columns.Contains("Longitude") && row["Longitude"] != null)
                {
                    model.Longitude = decimal.Parse(row["Longitude"].ToString());
                }
                if (row.Table.Columns.Contains("Latitude") && row["Latitude"] != null)
                {
                    model.Lantitude = decimal.Parse(row["Latitude"].ToString());
                }
                if (row.Table.Columns.Contains("Height") && row["Height"] != null)
                {
                    model.Height = decimal.Parse(row["Height"].ToString());
                }
                if (row.Table.Columns.Contains("State") && row["State"] != null && row["State"].ToString() != "")
                {
                    model.State = (WeiBoStateEnum)int.Parse(row["State"].ToString());
                }
                if (row.Table.Columns.Contains("HardWareType") && row["HardWareType"] != null)
                {
                    model.HardWareType = row["HardWareType"].ToString();
                }
                if (row.Table.Columns.Contains("IsRePost") && row["IsRePost"] != null)
                {
                    model.IsRePost = Boolean.Parse(row["IsRePost"].ToString());
                }
                if (row.Table.Columns.Contains("CreateTime") && row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                {
                    model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                }
                if (row.Table.Columns.Contains("UpdateTime") && row["UpdateTime"] != null && row["UpdateTime"].ToString() != "")
                {
                    model.UpdateTime = DateTime.Parse(row["UpdateTime"].ToString());
                }
            }
            return model;
        }

    }
}
