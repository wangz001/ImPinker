using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using AhBll;
using AhModel;

namespace GetCarDataService.Tools
{
    public class ImportPropertyMap
    {
        private static readonly string ExcelPath = AppDomain.CurrentDomain.BaseDirectory + "CompanyStylePropertyMap.txt";

        public static void Import()
        {
            List<BasicPropertyMap> lists = ReadFromTxt(ExcelPath);
            foreach (var row in lists)
            {
                var bitPropertyId = row.BitPropertyId;
                var comparePropertyId = row.ComparePropertyId;
                var companyId = row.CompanyId;
                if (bitPropertyId>0)
                {
                    AddPropertyMap(bitPropertyId, comparePropertyId, companyId);
                }
            }
            Console.WriteLine("初始化车型参配对应关系完毕");
            Console.ReadLine();
        }

        private static void AddPropertyMap(int bitId, int comparePropertyId, int companyId)
        {
            var map = new BasicPropertyMap
            {
                BitPropertyId = bitId,
                CompanyId = companyId,
                ComparePropertyId = comparePropertyId,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            BasicPropertyMapBll.Add(map);
        }

        public static List<BasicPropertyMap> ReadFromTxt(string excelPath)
        {
            string result;
            using (var fileStream = File.Open(excelPath, FileMode.OpenOrCreate, FileAccess.Read))
            using (var streamReader = new StreamReader(fileStream))
            {
                result = streamReader.ReadToEnd();
                streamReader.Close();
            }
            var ser = new DataContractJsonSerializer(typeof(List<BasicPropertyMap>));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (List<BasicPropertyMap>)ser.ReadObject(ms);
            return data;
        }
    }
}
