using System;

namespace AhModel
{
    /// <summary>
    /// 易车网和之家车型库，对应关系
    /// </summary>
    public class AhEntityMap  :EntityBase
    {
       
        /// <summary>
        /// 易车实体ID
        /// </summary>
        public int BitID { get; set; } 
        /// <summary>
        /// xx之家实体ID
        /// </summary>
        public int AhID { get; set; }
        /// <summary>
        /// 实体类型。0：主品牌、1：品牌、2：车系、3：车型
        /// </summary>
        public int ModelType { get; set; }
        
    }
}
