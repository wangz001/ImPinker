using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AhDal;
using AhModel;

namespace AhBll
{
    public class BasicStyleBll
    {
        static BasicStyleDal _styleDal = new BasicStyleDal();

        public static void AddStyle(BasicStyle model)
        {
            if (_styleDal.IsExit(model))
            {
                if (_styleDal.NeedUpdate(model))
                {
                    _styleDal.Update(model);
                }
            }
            else
            {
                _styleDal.Insert(model);
            }
        }


        public static List<BasicStyle> GetList(int companyId)
        {
            var ds = _styleDal.GetLists(companyId);
            if (ds == null)
            {
                return null;
            }
            return DataTableToList(ds.Tables[0]);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public static List<BasicStyle> DataTableToList(DataTable dt)
        {
            var modelList = new List<BasicStyle>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                BasicStyle model;
                for (int n = 0; n < rowsCount; n++)
                {
                    model = new BasicStyle();
                    if (dt.Rows[n]["ID"] != null && dt.Rows[n]["ID"].ToString() != "")
                    {
                        model.ID = int.Parse(dt.Rows[n]["ID"].ToString());
                    }
                    if (dt.Rows[n]["CompanyId"] != null && dt.Rows[n]["CompanyId"].ToString() != "")
                    {
                        model.CompanyId = int.Parse(dt.Rows[n]["CompanyId"].ToString());
                    }
                    if (dt.Rows[n]["SerialId"] != null && dt.Rows[n]["SerialId"].ToString() != "")
                    {
                        model.SerialId = int.Parse(dt.Rows[n]["SerialId"].ToString());
                    }
                    if (dt.Rows[n]["Name"] != null && dt.Rows[n]["Name"].ToString() != "")
                    {
                        model.Name = dt.Rows[n]["Name"].ToString();
                    }
                    if (dt.Rows[n]["Year"] != null && dt.Rows[n]["Year"].ToString() != "")
                    {
                        model.Year = dt.Rows[n]["Year"].ToString();
                    }
                    if (dt.Rows[n]["Price"] != null && dt.Rows[n]["Price"].ToString() != "")
                    {
                        model.Price = decimal.Parse(dt.Rows[n]["Price"].ToString());
                    }
                    if (dt.Rows[n]["SaleStatus"] != null && dt.Rows[n]["SaleStatus"].ToString() != "")
                    {
                        model.SaleStatus = dt.Rows[n]["SaleStatus"].ToString();
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

        public static List<BasicStyle> GetListsBySerialId(int companyId, int serialId)
        {
            var ds = _styleDal.GetListsBySerialId(companyId, serialId);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return DataTableToList(ds.Tables[0]);
            }
            return null;
        }

        public static bool CheckRegex(string pattern, string value)
        {
            var regex = new Regex(pattern);
            if (regex.IsMatch(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static BasicStyle GetStyleById(int companyId, int bisicStyleId)
        {
            return _styleDal.GetStyleById(companyId, bisicStyleId);
        }

        /// <summary>
        /// 获取竞品车型信息，附带参配值（排量），做车型自动匹配用
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="serialId"></param>
        /// <param name="comparePropertyId"></param>
        /// <returns></returns>
        public static List<StyleVM> GetListsWithProperty(int companyId, int serialId, int comparePropertyId)
        {
            var lists = new List<StyleVM>();
            var ds = _styleDal.GetListsWithProperty(companyId, serialId, comparePropertyId);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var vm = new StyleVM
                    {
                        Id = Int32.Parse(row["Id"].ToString()),
                        CompanyId = Int32.Parse(row["CompanyId"].ToString()),
                        Name = row["Name"].ToString(),
                        Price = decimal.Parse(row["Price"].ToString()),
                        Year = row["Year"].ToString(),
                        StrValue = row["Value"].ToString() //排量，之家、太平洋、搜狐、腾讯需要转换
                    };

                    if (!string.IsNullOrEmpty(vm.StrValue))
                    {
                        vm.StrValue = vm.StrValue.Replace("L", ""); //去掉搜狐的排量L
                        const string pattern = "^[0-9]*$";
                        if (CheckRegex(pattern, vm.StrValue))
                        {
                            var pailiang = Int32.Parse(vm.StrValue);//ml转换成L
                            var aa = Math.Round((1.0 * pailiang / 100), 0);
                            double result = Math.Round((1.0 * aa / 10), 1);
                            vm.StrValue = result.ToString("0.0");
                        }
                    }
                    lists.Add(vm);
                }
            }
            return lists;
        }
    }

    /// <summary>
    /// 车型自动匹配用，根据年款、价格、排量进行匹配
    /// </summary>
    public class StyleVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Year { get; set; }

        public decimal Price { get; set; }

        public string StrValue { get; set; } //动态参配，排量

        public int CompanyId { get; set; }

    }
}
