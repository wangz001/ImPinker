using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImpinkerApi.Models
{
    public class ArticleViewModel
    {
        public long Id
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string ArticleName
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Url
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public string CoverImage
        {
            set;
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        public int UserId
        {
            set;
            get;
        }

        public string KeyWords
        {
            set;
            get;
        }

        /// <summary>
        /// 所属网站。易车、之家、e族等
        /// </summary>
        public string Company
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

        public string Content { get; set; }

        /// <summary>
        ///  状态:      0:删除    1:正常可显示   2: 待审核   3:审核不通过  4:草稿，写游记用到
        /// </summary>
        public int State
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
        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime
        {
            set;
            get;
        }
        public DateTime PublishTime
        {
            set;
            get;
        }
    }
}