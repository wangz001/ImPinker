using System;

namespace AhModel
{
    /// <summary>
    /// 易车网和汽车之家、太平洋、搜狐、爱卡、腾讯车型库，对应关系
    /// </summary>
    public class BasicEntityMap 
    {

        public int Id { get; set; }
       
        /// <summary>
        /// 易车实体ID
        /// </summary>
        public int BitEntityId { get; set; } 
        /// <summary>
        /// xx之家实体ID
        /// </summary>
        public int CompareEntityId { get; set; }
        /// <summary>
        /// 实体类型。0：主品牌、1：品牌、2：车系、3：车型
        /// </summary>
        public int EntityType { get; set; }

        /// <summary>
        /// 竞品类型
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// 是否人工设置的对应关系
        /// </summary>
        public int IsPeopleSet { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
        
    }

}
