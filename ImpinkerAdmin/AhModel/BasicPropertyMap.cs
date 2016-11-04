using System;

namespace AhModel
{
    /// <summary>
    /// 易车网和汽车之家、太平洋、搜狐、爱卡、腾讯车型库，对应关系
    /// </summary>
    public class BasicPropertyMap:BasicEntityBase 
    {

        
        /// <summary>
        /// 易车实体ID
        /// </summary>
        public int BitPropertyId { get; set; } 
        /// <summary>
        /// xx之家实体ID
        /// </summary>
        public int ComparePropertyId { get; set; }
       
        /// <summary>
        /// 竞品类型
        /// </summary>
        //public int CompanyId { get; set; }

       
        
    }
}
