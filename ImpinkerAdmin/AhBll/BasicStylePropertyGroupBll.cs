using System;
using System.Collections.Generic;
using System.Data;
using AhDal;
using AhModel;

namespace AhBll
{
    public class BasicStylePropertyGroupBll
    {
        static BasicStylePropertyGroupDal _stylePropertyGroupDal = new BasicStylePropertyGroupDal();


        public static bool AddStylePropertyGroup(int groupId, string groupName, int companyId)
        {
            var flag = true;
            var model = new BasicStylePropertyGroup
            {
                CompanyId = companyId,
                ID = groupId,
                Name = groupName,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            var isExit = _stylePropertyGroupDal.IsExit(model);

            if (!isExit)
            {
                flag = _stylePropertyGroupDal.Insert(model);
            }
            else
            {
                if (_stylePropertyGroupDal.NeedUpdate(model))
                {
                    flag = _stylePropertyGroupDal.Update(model);
                }
            }
            Console.WriteLine("参配分组：" + groupName);
            return flag;
        }


        public static List<BasicStylePropertyGroup> GetList(int companyId)
        {
            var ds = _stylePropertyGroupDal.GetLists(companyId);
            if (ds == null)
            {
                return null;
            }
            return DataTableToList(ds.Tables[0]);

        }

        public static List<BasicStylePropertyGroup> DataTableToList(DataTable dt)
        {
            var modelList = new List<BasicStylePropertyGroup>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                BasicStylePropertyGroup model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new BasicStylePropertyGroup();
                    if (dt.Rows[n]["CompanyId"] != null && dt.Rows[n]["CompanyId"].ToString() != "")
                    {
                        model.CompanyId = int.Parse(dt.Rows[n]["CompanyId"].ToString());
                    }
                    if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
                    {
                        model.ID = int.Parse(dt.Rows[n]["ID"].ToString());
                    }
                    if (dt.Rows[n]["Name"] != null && dt.Rows[n]["Name"].ToString() != "")
                    {
                        model.Name = dt.Rows[n]["Name"].ToString();
                    }
                    if (dt.Rows[n]["CreateTime"] != null && dt.Rows[n]["CreateTime"].ToString() != "")
                    {
                        model.CreateTime = DateTime.Parse(dt.Rows[n]["CreateTime"].ToString());
                    }
                    if (dt.Rows[n]["UpdateTime"] != null && dt.Rows[n]["UpdateTime"].ToString() != "")
                    {
                        model.UpdateTime = DateTime.Parse(dt.Rows[n]["UpdateTime"].ToString());
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }

        public static int GetMaxGroupId(int companyId)
        {
            var maxId = _stylePropertyGroupDal.GetMaxGroupId(companyId);
            if (maxId==null)
            {
                return 0;
            }
            return (int)maxId;
        }
    }
}
