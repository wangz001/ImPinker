using System;
using ImModel.Enum;

namespace ImModel
{
    /// <summary>
    /// ArticleTag:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public class ArticleTag
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string TagName
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDelete
        {
            get;
            set;
        }
        /// <summary>
        /// tag前台展示状态
        /// </summary>
        public FrountShowStateEnum FrountShowState
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime
        {
            get;
            set;
        }
    }
}

