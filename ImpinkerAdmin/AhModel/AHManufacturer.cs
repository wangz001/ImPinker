using System;

namespace AhModel
{
    /// <summary>
    /// AHMake:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class AhManufacturer:EntityBase
    {

        public string Initial { set; get; }

        public string ManufacturerName { set; get; }

        public int IsRemoved { get; set; }

    }
}

