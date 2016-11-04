using System;
using System.Linq;
using System.Text.RegularExpressions;
using AhBll;
using AhModel;

namespace GetCarDataService.GetXCAutoCarsData
{
    /// <summary>
    /// 爱卡汽车
    /// </summary>
    public class GetBasicData
    {
        private const int CompanyId = (int)CompanyEnum.XCauto;

        public static void Get()
        {
            const string masterUrl = "http://newcar.xcar.com.cn/pub_js/car_arr_newcar_2009_ps.js";
            string strResult = Common.GetResponseContent(masterUrl).Trim();
            int startIndex = (strResult.IndexOf("pb_arr='", StringComparison.Ordinal)) + 8;
            int endIndex = (strResult.IndexOf("ps_arr=", StringComparison.Ordinal));
            var brandResult = strResult.Substring(startIndex, endIndex - startIndex);
            var serialResult = strResult.Substring(endIndex + 21);
            GetMasterBrand(brandResult);
            GetMakeAndSerial(serialResult);
            GetBasicStyle.Get();
        }

        public static void GetMasterBrand(string brandResult)
        {
            brandResult = brandResult.Substring(0, brandResult.LastIndexOf(";", StringComparison.Ordinal) - 1);
            var lists = brandResult.Split(',');
            if (lists.Count() % 2 != 0)
            {
                Common.WriteErrorLog("[" + Common.GetDateTimeStr() + "]" + "获取爱卡汽车主品牌错误:" + brandResult);
                return;
            }
            for (var i = 0; i < lists.Count(); i++)
            {
                var id = lists[i];
                var name = lists[i + 1];
                const string pattern = "^[0-9]*$";
                if (!string.IsNullOrEmpty(id) && CheckRegex(pattern, id))
                {
                    var brand = new BasicMasterBrand
                    {
                        ID = Int32.Parse(id),
                        CompanyId = CompanyId,
                        Name = name,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now
                    };
                    BasicMasterBrandBll.AddBrand(brand);
                    Console.WriteLine(name);
                }
                i++;
            }
        }

        public static void GetMakeAndSerial(string serialResult)
        {
            const string pattern = "^[0-9]*$";
            const string patternName = "^==[^x00-xff]*==$";
            serialResult = serialResult.Substring(0, serialResult.LastIndexOf(";", StringComparison.Ordinal));
            var lists = serialResult.Split(';');
            for (int i = 0; i < lists.Count(); i++)
            {
                var manu = lists[i];
                var startIndex = manu.IndexOf("ps_arr['", StringComparison.Ordinal) + 8;
                var endIndex = manu.IndexOf("']=", StringComparison.Ordinal);
                var masterBrandId = manu.Substring(startIndex, endIndex - startIndex);
                startIndex = manu.IndexOf("']='", StringComparison.Ordinal) + 4;
                endIndex = manu.LastIndexOf("'", StringComparison.Ordinal);
                var serialList = manu.Substring(startIndex, endIndex - startIndex).Split(',');

                var makeId = 0;
                for (int j = 0; j < serialList.Count(); j++)
                {
                    var id = serialList[j];
                    var name = serialList[j + 1];
                    if (!string.IsNullOrEmpty(id) && CheckRegex(pattern, id))
                    {
                        if (CheckRegex(patternName, name)) //厂商
                        {
                            name = name.Replace("=", "");
                            makeId = Int32.Parse(id);
                            AddMake(Int32.Parse(masterBrandId), makeId, name);
                        }
                        else
                        {
                            AddSerial(Int32.Parse(masterBrandId), makeId, Int32.Parse(id), name);
                        }
                    }
                    j++;
                }
            }
        }

        public static void AddMake(int masterBrandId, int makeId, string makeName)
        {
            var make = new BasicMake
            {
                ID = makeId,
                CompanyId = CompanyId,
                Name = makeName,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            BasicMakeBll.AddMake(make);

            var brandJoinMake = new BasicMasterBrandJoinMake
            {
                MasterBrandId = masterBrandId,
                MakeId = makeId,
                CompanyId = CompanyId,
                IsRemoved = 0,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            BasicMakeBll.AddMasterBrandJoinMake(brandJoinMake);
        }

        private static void AddSerial(int masterBrandId, int makeId, int serialId, string serialName)
        {
            var model = new BasicSerial
            {
                ID = serialId,
                CompanyId = CompanyId,
                Name = serialName,
                ManufacturerId = makeId,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            BasicSerialBll.AddSerial(model);
            Console.WriteLine(model.Name);
            var brandJoinSerial = new BasicMasterBrandJoinSerial
            {
                CompanyId = CompanyId,
                MasterBrandId = masterBrandId,
                SerialId = serialId,
                IsRemoved = 0,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            BasicSerialBll.AddBrandJoinSerial(brandJoinSerial);
        }

        public static bool CheckRegex(string pattern, string value)
        {
            var regex = new Regex(pattern);
            if (regex.IsMatch(value))
            {
                return true;
            }
            return false;
        }
    }
}
