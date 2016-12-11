using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace ImPinker.Common
{
    public class ImageUrlHelper
    {
        static string imgDomain = ConfigurationManager.AppSettings["ArtilceCoverImageDomain"];
        /// <summary>
        /// 获取头像url地址
        /// </summary>
        /// <param name="url">数据库保存的url</param>
        /// <param name="size">尺寸（180，100，40）</param>
        /// <returns></returns>
        public static string GetHeadImageUrl(string url,int size)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.StartsWith("http://"))
                {
                    return Path.Combine(imgDomain, url.Replace("headimg/limit", "headimg/"+size));
                }
                else
                {
                    return url;
                }
            }
            else
            {
                return ""; //默认的图片
            }
        }
        /// <summary>
        /// 获取文章的封面图
        /// </summary>
        /// <param name="url"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetArticleCoverImage(string url, int size)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.StartsWith("http://"))
                {
                    return Path.Combine(imgDomain, url);
                }
                else
                {
                    return url;
                }
            }
            else
            {
                return ""; //默认的图片
            }
        }
    }
}