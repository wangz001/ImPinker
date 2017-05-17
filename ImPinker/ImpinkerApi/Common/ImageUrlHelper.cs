using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace ImPinkerApi.Common
{
    public class ImageUrlHelper
    {
        readonly static string _imgDomain = ConfigurationManager.AppSettings["ImageDomain54"];
        /// <summary>
        /// 获取头像url地址
        /// </summary>
        /// <param name="url">数据库保存的url</param>
        /// <param name="size">尺寸（180，100，40）0表示原图limit</param>
        /// <returns></returns>
        public static string GetHeadImageUrl(string url,int size)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (!url.StartsWith("http://"))
                {
                    return Path.Combine(_imgDomain, size==0 ? url : url.Replace("headimg/limit", "headimg/"+size));
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
                    return Path.Combine(_imgDomain, url);
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