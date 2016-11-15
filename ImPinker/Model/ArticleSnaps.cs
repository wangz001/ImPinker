using System;
namespace Model
{
    /// <summary>
    /// 文章快照
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
        #endregion Model

    }
}

