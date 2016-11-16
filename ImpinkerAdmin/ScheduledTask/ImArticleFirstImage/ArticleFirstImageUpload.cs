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
    public class ArticleFirstImageUpload
    {
        static ArticleBll articleBll = new ArticleBll();
        private const string buckeyName = "myautos";

        public static void Start()
        {
            GetAllArticles();
        }
        public static void GetAllArticles()
        {
            const string key = "articlefirstimg/img/{0}_{1}.jpg";
            List<ArticleViewModel> list = articleBll.GetIndexListByPage(1, 20);
            foreach (var articleViewModel in list)
            {
                if (!string.IsNullOrEmpty(articleViewModel.CoverImage) && articleViewModel.CoverImage.StartsWith("http://"))
                {
                    var imgoldUrl = articleViewModel.CoverImage;
                    var newUrl = string.Format(key, articleViewModel.Id, DateTime.Now.Ticks);
                    var flag=UploadToOss(imgoldUrl, newUrl);
                    if (flag)
                    {
                        articleBll.UpdateCoverImage(long.Parse(articleViewModel.Id),newUrl);
                    }
                }
            }
        }

        public static bool UploadToOss( string imgUrl,string key)
        {
            if (string.IsNullOrEmpty(imgUrl))
            {
                return false;
            }
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
            //剪切(如果高大于2倍宽，剪切)
            if ( 2*(myImage.Width)<myImage.Height)
            {
                ImageUtils.ImgReduceCutOut(0,0,myImage.Width,myImage.Width*2/3,tempdir,tempdir);
            }
            //缩放
            ImageUtils.ThumbnailImage(tempdir,tempdir,width,height,ImageFormat.Jpeg);

            //保存
            bool flag = ObjectOperate.UploadImage(buckeyName, tempdir, key);
            return flag;
        }



    }

}
