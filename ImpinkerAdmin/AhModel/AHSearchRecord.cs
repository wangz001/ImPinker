using System;

namespace AhModel
{
    /// <summary>
    /// 用户对比记录
    /// </summary>
    internal class AhSearchRecord
    {
        /// <summary>
        /// 自增iD
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 易车车型库，车型id
        /// </summary>
        public int BitStyleID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


    }
}
