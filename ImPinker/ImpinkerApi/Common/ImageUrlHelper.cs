using ImModel.ViewModel;
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
        readonly static string _imgDomain = ConfigurationManager.AppSettings["ImageDomain"];
        /// <summary>
        /// 获取头像url地址
        /// </summary>
        /// <param name="url">数据库保存的url</param>
        /// <param name="size">尺寸（180，100，40）0表示原图limit</param>
        /// <returns></returns>
        public static string GetHeadImageUrl(string url, int size)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var sizeStyle = "";
                switch (size)
                {
                    case 180: sizeStyle = HeadImageFormat.head_180; break;
                    case 100: sizeStyle = HeadImageFormat.head_100; break;
                    case 40: sizeStyle = HeadImageFormat.head_40; break;
                }
                url=url+sizeStyle;
                if (!url.StartsWith("http://"))
                {
                    return _imgDomain+ url;
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
        /// 获取文章的封面图(900;360*240;240*160)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetArticleImage(string url, int size)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var sizeStyle = "";
                switch (size)
                {
                    case 900: sizeStyle = ArticleImageFormat.ArticleImage_900; break;
                    case 360: sizeStyle = ArticleImageFormat.CoverImage_36_24; break;
                    case 240: sizeStyle = ArticleImageFormat.CoverImage_24_16; break;
                }
                url = url + sizeStyle;
                if (!url.StartsWith("http://"))
                {
                    return _imgDomain + url;
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
        /// 获取微博图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetWeiboFullImageUrl(string imagesStr,int size)
        {
            var contentValue = new List<string>();
            var imagearr = imagesStr.Split(',');
            foreach (var image in imagearr)
            {
                var sizeStyle = "";
                switch (size)
                {
                    case 1200: sizeStyle = WeiboImageFormat.weibo_1200; break;
                    case 900: sizeStyle = WeiboImageFormat.weibo_900; break;
                    case 600: sizeStyle = WeiboImageFormat.weibo_600; break;
                    case 240: sizeStyle = WeiboImageFormat.weibo_24_16; break;
                }
                var imagepath = image + sizeStyle;
                if (!imagepath.StartsWith("http://"))
                {
                    imagepath = _imgDomain + imagepath;
                }
                contentValue.Add(imagepath);
            }
            return string.Join(",", contentValue);
        }
    }
}