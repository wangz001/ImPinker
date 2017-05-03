using System;

namespace ImModel
{
    /// <summary>
    /// 文章快照，主要是用来重建索引
    /// </summary>
    [Serializable]
    public class ArticleSnaps
    {
        #region Model

        /// <summary>
        /// 文章id'
        /// </summary>
        public long ArticleId
        {
            set;
            get;
        }
        /// <summary>
        /// 抓到的第一张图片url
        /// </summary>
        public string FirstImageUrl
        {
            set;
            get;
        }
        /// <summary>
        /// keywords
        /// </summary>
        public string KeyWords
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            set;
            get;
        }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime
        {
            set;
            get;
        }

        public DateTime UpdateTime
        {
            set;
            get;
        }
        #endregion Model

    }
}

