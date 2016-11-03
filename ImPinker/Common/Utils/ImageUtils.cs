using System;
using System.Configuration;

namespace Common.Utils
{
    public class ImageUtils
    {

        /// <summary>
        /// 可用的域名
        /// </summary>
        private static string[] imageDomains;

        static ImageUtils()
        {
            string imageDomainStr = ConfigurationManager.AppSettings["ImageDomains"];
            imageDomains = imageDomainStr.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 获得图片所在的域名
        /// </summary>
        /// <param name="imageID"></param>
        /// <returns></returns>
        public static string GetImageDomain(int imageID)
        {
            int domainCount = imageDomains.Length;
            int index = imageID % domainCount;
            return imageDomains[index];
        }

        /// <summary>
        /// 获得图片地址(不含域名和规格)
        /// </summary>
        /// <param name="imageID">图片ID</param>
        /// <param name="imagePartialUrl">图片路径</param>
        /// <returns></returns>
        public static string GetImageUrlWithoutDomainAndSpecification(int imageID, string imagePartialUrl)
        {
            if (imageID <= 0)
            {
                throw new ArgumentException("imageID不能小于等于0");
            }
            if (string.IsNullOrEmpty(imagePartialUrl))
            {
                throw new ArgumentException("imagePath不能为空");
            }

            int extensionIndex = imagePartialUrl.LastIndexOf('.');
            string imageUrl = string.Format("/{0}_{1}_{{0}}{2}",
                imagePartialUrl.Substring(0, extensionIndex),
                imageID, imagePartialUrl.Substring(extensionIndex)
            );
            return imageUrl;
        }

        /// <summary>
        /// 获取图片地址，不包含图片编号
        /// </summary>
        /// <param name="imagePartialUrl"></param>
        /// <returns></returns>
        public static string GetImageUrlWithoutDomainAndSpecification(string imagePartialUrl)
        {
            if (string.IsNullOrEmpty(imagePartialUrl))
            {
                throw new ArgumentException("imagePath不能为空");
            }

            int extensionIndex = imagePartialUrl.LastIndexOf('.');
            string imageUrl = string.Format("/{0}_{{0}}{1}",
                imagePartialUrl.Substring(0, extensionIndex),
                 imagePartialUrl.Substring(extensionIndex)
            );
            return imageUrl;
        }


        /// <summary>
        /// 获取图片路径，路径中不包含图片编号
        /// </summary>
        /// <param name="imageId"></param>
        /// <param name="imagePartialUrl"></param>
        /// <param name="specification"></param>
        /// <returns></returns>
        public static string GetImageUrlWithoutImageId(int imageId, string imagePartialUrl, int specification)
        {
            return string.Format("http://{0}{1}",
                GetImageDomain(imageId),
                string.Format(GetImageUrlWithoutDomainAndSpecification(imagePartialUrl),
                                specification)
                );
        }

        /// <summary>
        /// 获得图片地址(不含规格)
        /// </summary>
        /// <param name="imageID">图片ID</param>
        /// <param name="imagePartialUrl">图片路径</param>
        /// <returns></returns>
        public static string GetImageUrlWithoutSpecification(int imageID, string imagePartialUrl)
        {
            string imageUrl = string.Format("http://{0}{1}",
                GetImageDomain(imageID),
                GetImageUrlWithoutDomainAndSpecification(imageID, imagePartialUrl)
            );
            return imageUrl;
        }



        /// <summary>
        /// 获得图片地址
        /// </summary>
        /// <param name="imageID">图片大小</param>
        /// <param name="imagePartialUrl">图片路径</param>
        /// <param name="specific">图片规格</param>
        /// <returns></returns>
        public static string GetImageUrl(int imageID, string imagePartialUrl, int specification)
        {
            return string.Format(GetImageUrlWithoutSpecification(imageID, imagePartialUrl), specification);
        }
    }
}
