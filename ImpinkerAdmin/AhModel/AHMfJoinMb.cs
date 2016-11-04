using System;

namespace AhModel
{
    /// <summary>
    /// AHMake:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class AHMfJoinMb:EntityBase
    {

        public int MasterBrandID { get; set; }

        public int ManufacturerID { get; set; }
    }
}

