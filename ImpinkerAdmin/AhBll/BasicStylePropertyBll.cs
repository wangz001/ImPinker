using System;
using System.Collections.Generic;
using System.Data;
using AhDal;
using AhModel;

namespace AhBll
{
    public class BasicStylePropertyBll
    {
        static BasicStylePropertyDal _stylePropertyDal = new BasicStylePropertyDal();


        public static bool AddStyleProperty(BasicStyleProperty model)
        {
            var flag = true;

            var isExit = _stylePropertyDal.IsExit(model);

            if (!isExit)
            {
                flag = _stylePropertyDal.Insert(model);
            }
            else
            {
                if (_stylePropertyDal.NeedUpdate(model))
                {
                    flag = _stylePropertyDal.Update(model);
                }
            }
            return flag;
        }


        public static List<BasicStyleProperty> GetList(int companyId)
        {
            var ds = _stylePropertyDal.GetLists(companyId);
            if (ds==null)
            {
                return null;
            }
            return DataTableToList(ds.Tables[0]);
        }

        public static List<BasicStyleProperty> DataTableToList(DataTable dt)
        {
            var modelList = new List<BasicStyleProperty>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                BasicStyleProperty model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new BasicStyleProperty();
                    if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
                    {
                        model.ID = int.Parse(dt.Rows[n]["ID"].ToString());
                    }
                    if (dt.Rows[n]["CompanyId"] != null && dt.Rows[n]["CompanyId"].ToString() != "")
                    {
                        model.CompanyId = int.Parse(dt.Rows[n]["CompanyId"].ToString());
                    }
                    if (dt.Rows[n]["PropertyGroupId"] != null && dt.Rows[n]["PropertyGroupId"].ToString() != "")
                    {
                        model.PropertyGroupId = int.Parse(dt.Rows[n]["PropertyGroupId"].ToString());
                    }
                    if (dt.Rows[n]["Name"] != null && dt.Rows[n]["Name"].ToString() != "")
                    {
                        model.Name = dt.Rows[n]["Name"].ToString();
                    }
                    if (dt.Rows[n]["EnglishName"] != null && dt.Rows[n]["EnglishName"].ToString() != "")
                    {
                        model.EnglishName = dt.Rows[n]["EnglishName"].ToString();
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

       
        public static int GetMinId()
        {
            return _stylePropertyDal.GetMinId();
        }
        /// <summary>
        /// 腾讯汽车和搜狐汽车用，用英文名当做唯一标识
        /// </summary>
        /// <param name="model"></param>
        public static void AddStylePropertyWithEnglishName(BasicStyleProperty model)
        {
            bool flag = _stylePropertyDal.IsExitPropertyWithEnglishName(model);
            if (flag)
            {
                _stylePropertyDal.UpdatePropertyWithEnglishName(model);
            }
            else
            {
                if (_stylePropertyDal.IsExit(model))// 解决腾讯汽车参配ID相同的情况
                {
                    var propertyId = -100;
                    var minId = _stylePropertyDal.GetMinId();
                    if (minId <= -100)
                    {
                        propertyId = minId - 1;
                    }
                    model.ID = propertyId;
                }
                _stylePropertyDal.Insert(model);
            }
        }
    }
}
