using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using Common.AlyOssUtil;
using Common.Utils;
using ImBLL;
using ImModel.ViewModel;

namespace GetCarDataService.ImArticleFirstImage
{
    /// <summary>
    /// 根据articlesnap表的firstimageurl，生成缩略图，上传到oss，并更新到article主表（solr索引自动更新或定时更新）
    /// </summary>
    public class ArticleFirstImageUpload
    {
        static ArticleBll articleBll = new ArticleBll();
        static ArticleSnapsBll articlesnapBll = new ArticleSnapsBll();
        private const string buckeyName = "myautos";
        const string ImgUrlformat = "articlefirstimg/{0}/{1}_{2}.jpg";
        public static void Start()
        {
            Console.WriteLine("开始检查图片：" + DateTime.Now.Ticks);
            var articleList = articleBll.GetArticlesWithoutCoverImage();
            foreach (var article in articleList)
            {
                var articleSnap = articlesnapBll.GetModel(article.Id);
                if (articleSnap != null && !string.IsNullOrEmpty(articleSnap.FirstImageUrl))
                {
                    var firstimageUrl = articleSnap.FirstImageUrl;
                    var imgurl = string.Format(ImgUrlformat, DateTime.Now.ToString("yyyyMMdd"), article.Id, DateTime.Now.Ticks);
                    var flag = UploadToOss(firstimageUrl, imgurl);
                    if (flag)
                    {
                        var updateflag = articleBll.UpdateCoverImage(article.Id, imgurl);
                        if (!updateflag)
                        {
                            //记录错误日志
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成360*240小图，上传到oss
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool UploadToOss(string imgUrl, string key)
        {
            if (string.IsNullOrEmpty(imgUrl))
            {
                return false;
            }
            try
            {
                var tempdir = AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\temp.jpg";
                const int width = 360;
                const int height = 240;
                //下载
                Uri myUri = new Uri(imgUrl);
                WebRequest webRequest = WebRequest.Create(myUri);
                WebResponse webResponse = webRequest.GetResponse();
                Bitmap myImage = new Bitmap(webResponse.GetResponseStream());
                MemoryStream ms = new MemoryStream();
                myImage.Save(ms, ImageFormat.Jpeg);
                myImage.Save(tempdir);
                //剪切(如果高大于1.5倍宽，剪切)
                if (1.5 * (myImage.Width) < myImage.Height)
                {
                    ImageUtils.ImgReduceCutOut(0, 0, myImage.Width, myImage.Width * 2 / 3, tempdir, tempdir);
                }
                //缩放
                ImageUtils.ThumbnailImage(tempdir, tempdir, width, height, ImageFormat.Jpeg);

                //保存
                bool flag = ObjectOperate.UploadImage(buckeyName, tempdir, key);
                return flag;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

}
