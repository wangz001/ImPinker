using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Common.Utils
{
    public class ImageUtils
    {

        static ImageUtils()
        {
            string imageDomainStr = ConfigurationManager.AppSettings["ImageDomains"];
        }

        /// <summary>
        /// 图片裁剪方法
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="intWidth"></param>
        /// <param name="intHeight"></param>
        /// <param name="inputImgUrl"></param>
        /// <param name="outImgUrl"></param>
        public static void ImgReduceCutOut(int startX, int startY, int intWidth, int intHeight, string inputImgUrl, string outImgUrl)
        {
            //通过连接创建Image对象
            Image oldimage = Image.FromFile(inputImgUrl);
            var tempjpg = AppDomain.CurrentDomain.BaseDirectory + "Upload\\tempcut.jpg";
            oldimage.Save(tempjpg);//把原图Copy一份出来,然后在temp.jpg上进行裁剪,最后把裁剪后的图片覆盖原图
            oldimage.Dispose();//一定要释放临时图片,要不之后的在此图上的操作会报错,原因冲突
            var bm = new Bitmap(tempjpg);
            //处理JPG质量的函数
            var codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (var codec in codecs)
            {
                if (codec.MimeType == "image/jpeg")
                {
                    ici = codec;
                    break;
                }
            }
            var ep = new EncoderParameters();
            const long level = 95L;//图片质量
            ep.Param[0] = new EncoderParameter(Encoder.Quality, level);

            // 裁剪图片
            var cloneRect = new Rectangle(startX, startY, intWidth, intHeight);
            PixelFormat format = bm.PixelFormat;
            Bitmap cloneBitmap = bm.Clone(cloneRect, format);
            if (!Directory.Exists(Path.GetDirectoryName(outImgUrl)))//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(Path.GetDirectoryName(outImgUrl));
            }
            //保存图片
            cloneBitmap.Save(outImgUrl, ici, ep);
            cloneBitmap.Dispose();
            bm.Dispose();
        }

        public static bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// 缩放图片
        /// </summary>
        /// <param name="oldImagePath"></param>
        /// <param name="newImagePath"></param>
        /// <param name="newW"></param>
        /// <param name="newH"></param>
        /// <param name="imageFormat"></param>
        public static void ThumbnailImage(string oldImagePath, string newImagePath, int newW, int newH, ImageFormat imageFormat)
        {
            if (System.IO.File.Exists(oldImagePath))
            {
                //处理JPG质量的函数
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.MimeType == "image/jpeg")
                    {
                        ici = codec;
                        break;
                    }
                }
                var ep = new EncoderParameters();
                const long level = 95L;
                ep.Param[0] = new EncoderParameter(Encoder.Quality, level);
                //通过连接创建Image对象
                Image oldimage = Image.FromFile(oldImagePath);
                var tempjpg = AppDomain.CurrentDomain.BaseDirectory + "Upload\\tempthumb.jpg";
                if (!Directory.Exists(Path.GetDirectoryName(newImagePath)))//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newImagePath));
                }
                oldimage.Save(tempjpg);//把原图Copy一份出来,然后在temp.jpg上进行裁剪,最后把裁剪后的图片覆盖原图
                oldimage.Dispose();//一定要释放临时图片,要不之后的在此图上的操作会报错,原因冲突
                var bm = new Bitmap(tempjpg);
                var cutImg = bm.GetThumbnailImage(newW, newH, (ThumbnailCallback), IntPtr.Zero);
                if (ici != null) cutImg.Save(newImagePath, ici, ep);
                cutImg.Dispose();
                bm.Dispose();

            }
        }

    }
}
