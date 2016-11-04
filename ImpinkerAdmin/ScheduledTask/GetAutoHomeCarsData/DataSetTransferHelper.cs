using System.Collections.Generic;
using System.Data;
using AhModel;

namespace GetCarDataService.GetAutoHomeCarsData
{
    class DataSetTransferHelper
    {
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public static List<AhStyle> DataTableToList(DataTable dt)
        {
            var modelList = new List<AhStyle>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                AhStyle model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new AhStyle();
                    if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
                    {
                        model.ID = int.Parse(dt.Rows[n]["ID"].ToString());
                    }
                    if (dt.Rows[n]["ModelID"] != null && dt.Rows[n]["ModelID"].ToString() != "")
                    {
                        model.ModelID = int.Parse(dt.Rows[n]["ModelID"].ToString());
                    }
                    if (dt.Rows[n]["StyleName"] != null && dt.Rows[n]["StyleName"].ToString() != "")
                    {
                        model.StyleName = dt.Rows[n]["StyleName"].ToString();
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public static List<AhStyleProperty> DataTableToListStyleProperty(DataTable dt)
        {
            List<AhStyleProperty> modelList = new List<AhStyleProperty>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                AhStyleProperty model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new AhStyleProperty();
                    if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
                    {
                        model.ID = int.Parse(dt.Rows[n]["ID"].ToString());
                    }
                    if (dt.Rows[n]["PropertyGroupID"] != null && dt.Rows[n]["PropertyGroupID"].ToString() != "")
                    {
                        model.PropertyGroupID = int.Parse(dt.Rows[n]["PropertyGroupID"].ToString());
                    }
                    if (dt.Rows[n]["Name"] != null && dt.Rows[n]["Name"].ToString() != "")
                    {
                        model.Name = dt.Rows[n]["Name"].ToString();
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }


        public static List<AhStylePropertyGroup> DataTableToListsGroups(DataTable dt)
        {
            List<AhStylePropertyGroup> modelList = new List<AhStylePropertyGroup>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                AhStylePropertyGroup model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new AhStylePropertyGroup();
                    if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
                    {
                        model.ID = int.Parse(dt.Rows[n]["ID"].ToString());
                    }
                    if (dt.Rows[n]["Name"] != null && dt.Rows[n]["Name"].ToString() != "")
                    {
                        model.Name = dt.Rows[n]["Name"].ToString();
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }
    }
}
