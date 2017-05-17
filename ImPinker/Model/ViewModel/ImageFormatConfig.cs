using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel.ViewModel
{
    public class ArticleImageFormat
    {
        /// <summary>
        /// 封面图360*240
        /// </summary>
        public static string CoverImage_36_24
        {
            get { return "?x-oss-process=style/articlecover_36_24"; }
        }
        /// <summary>
        /// 封面图240*160
        /// </summary>
        public static string CoverImage_24_16
        {
            get { return "?x-oss-process=style/articlecover_24_16"; }
        }
        /// <summary>
        /// 文章图片。最大宽度900
        /// </summary>
        public static string ArticleImage_900
        {
            get { return "?x-oss-process=style/article_900"; }
        }
    }

    public class WeiboImageFormat
    {
        /// <summary>
        /// 240*160
        /// </summary>
        public static string weibo_24_16
        {
            get { return "?x-oss-process=style/weibo_24_16"; }
        }
        /// <summary>
        /// 最大宽度1200
        /// </summary>
        public static string weibo_1200
        {
            get { return "?x-oss-process=style/weibo_1200"; }
        }
        /// <summary>
        /// 最大宽度900
        /// </summary>
        public static string weibo_900
        {
            get { return "?x-oss-process=style/weibo_900"; }
        }
        /// <summary>
        /// 最大宽度600
        /// </summary>
        public static string weibo_600
        {
            get { return "?x-oss-process=style/weibo_600"; }
        }

    }
    /// <summary>
    /// 头像规格
    /// </summary>
    public class HeadImageFormat
    {
        public static string head_180
        {
            get { return "?x-oss-process=style/head_180"; }
        }
        public static string head_100
        {
            get { return "?x-oss-process=style/head_100"; }
        }
        public static string head_40
        {
            get { return "?x-oss-process=style/head_40"; }
        }
    }
}
