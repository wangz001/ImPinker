using System;

namespace AhModel
{
    /// <summary>
    /// AHStyle:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public class AhStyle : EntityBase
    {



        /// <summary>
        /// 
        /// </summary>
        public int ModelID{set;get;}

        /// <summary>
        /// 
        /// </summary>
        public string StyleName{set;get;}

        /// <summary>
        /// 销售状态
        /// </summary>
        public string SaleStatus{set;get;}

        /// <summary>
        /// 
        /// </summary>
        public decimal? Price{set;get;}

        public string Year{ set; get; }

        public int IsRemoved { get; set; }

    }
}

