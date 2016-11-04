using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GetCarDataService.CarsDataCountTask
{
    public class FrontStyleProperty
    {
        private  static List<FrontStylePropertyMap> StylePropertyMaps; 
        /// <summary>
        /// 构造方法，加载数据
        /// </summary>
        static FrontStyleProperty()
        {
            StylePropertyMaps = ReadFromTxt();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public static List<FrontStylePropertyMap> GetStylePropertyMaps()
        {
            return StylePropertyMaps;
        }

        /// <summary>
        /// 从配置文件读取json数据，并序列化
        /// </summary>
        /// <returns></returns>
        private static List<FrontStylePropertyMap> ReadFromTxt()
        {
            string result;
            string appPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ResourceFile\\" + "stylePropertyJson.txt");
            using (var fileStream = File.Open(appPath, FileMode.OpenOrCreate, FileAccess.Read))
            using (var streamReader = new StreamReader(fileStream))
            {
                result=streamReader.ReadToEnd();
                streamReader.Close();
            }
            var ser = new DataContractJsonSerializer(typeof(List<FrontStylePropertyMap>));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (List<FrontStylePropertyMap>)ser.ReadObject(ms);
            return data;
        }
    }


    /// <summary>
    /// 模型类
    /// </summary>
    public class FrontStylePropertyMap
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}