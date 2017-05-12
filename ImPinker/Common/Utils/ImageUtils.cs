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
        /// 按模版比例最大范围的裁剪图片（从中心采取）并缩放至模版尺寸
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <param name="inputImgUrl"></param>
        /// <param name="outImgUrl"></param>
        /// <param name="quality">质量（范围0-100）</param>
        public static void GetReduceImgFromCenter(int newWidth, int newHeight, string inputImgUrl, string outImgUrl, long quality)
        {
            //通过连接创建Image对象
            Image oldimage = Image.FromFile(inputImgUrl);
            var tempjpg = AppDomain.CurrentDomain.BaseDirectory + "ImageUpload\\tempcutnew.jpg";
            oldimage.Save(tempjpg);//把原图Copy一份出来,然后在temp.jpg上进行裁剪,最后把裁剪后的图片覆盖原图
            oldimage.Dispose();//一定要释放临时图片,要不之后的在此图上的操作会报错,原因冲突
            var initImage = new Bitmap(tempjpg);
            //原图宽高均小于模版，不作处理，直接保存
            if (initImage.Width <= newWidth && initImage.Height <= newHeight)
            {
                SaveImage(initImage, quality, outImgUrl);
                initImage.Dispose();
            }
            else
            {
                //模版的宽高比例
                double templateRate = (double)newWidth / newHeight;
                //原图片的宽高比例
                double initRate = (double)initImage.Width / initImage.Height;
                //原图与模版比例相等，直接缩放
                if (templateRate == initRate)
                {
                    //按模版大小生成最终图片
                    Image templateImage = new Bitmap(newWidth, newHeight);
                    Graphics templateG = Graphics.FromImage(templateImage);
                    templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    templateG.Clear(Color.White);
                    templateG.DrawImage(initImage, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, initImage.Width, initImage.Height), GraphicsUnit.Pixel);
                    SaveImage(templateImage, quality, outImgUrl);
                    templateG.Dispose();
                    templateImage.Dispose();
                }
                //原图与模版比例不等，裁剪后缩放
                else
                {
                    //裁剪对象
                    Image pickedImage = null;
                    Graphics pickedG = null;

                    //定位
                    Rectangle fromR = new Rectangle(0, 0, 0, 0);//原图裁剪定位
                    Rectangle toR = new Rectangle(0, 0, 0, 0);//目标定位

                    //宽为标准进行裁剪
                    if (templateRate > initRate)
                    {
                        //裁剪对象实例化
                        pickedImage = new Bitmap(initImage.Width, (int)System.Math.Floor(initImage.Width / templateRate));
                        pickedG = Graphics.FromImage(pickedImage);

                        //裁剪源定位
                        fromR.X = 0;
                        fromR.Y = (int)Math.Floor((initImage.Height - initImage.Width / templateRate) / 2);
                        fromR.Width = initImage.Width;
                        fromR.Height = (int)Math.Floor(initImage.Width / templateRate);

                        //裁剪目标定位
                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = initImage.Width;
                        toR.Height = (int)Math.Floor(initImage.Width / templateRate);
                    }
                    //高为标准进行裁剪
                    else
                    {
                        pickedImage = new Bitmap((int)Math.Floor(initImage.Height * templateRate), initImage.Height);
                        pickedG = Graphics.FromImage(pickedImage);

                        fromR.X = (int)Math.Floor((initImage.Width - initImage.Height * templateRate) / 2);
                        fromR.Y = 0;
                        fromR.Width = (int)Math.Floor(initImage.Height * templateRate);
                        fromR.Height = initImage.Height;

                        toR.X = 0;
                        toR.Y = 0;
                        toR.Width = (int)Math.Floor(initImage.Height * templateRate);
                        toR.Height = initImage.Height;
                    }

                    //设置质量
                    pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    //裁剪
                    pickedG.DrawImage(initImage, toR, fromR, GraphicsUnit.Pixel);

                    //按模版大小生成最终图片
                    Image templateImage = new Bitmap(newWidth, newHeight);
                    Graphics templateG = Graphics.FromImage(templateImage);
                    //设置 System.Drawing.Graphics对象的SmoothingMode属性为HighQuality
                    templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                    //下面这个也设成高质量
                    templateG.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    //下面这个设成High
                    templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    templateG.Clear(Color.White);
                    templateG.DrawImage(pickedImage, new Rectangle(0, 0, newWidth, newHeight),
                        new Rectangle(0, 0, pickedImage.Width, pickedImage.Height), GraphicsUnit.Pixel);
                    //参考  http://blog.csdn.net/jixiaomeng821/article/details/46756093  
                    SaveImage(templateImage, quality, outImgUrl);
                    //释放资源
                    templateG.Dispose();
                    templateImage.Dispose();
                    pickedG.Dispose();
                    pickedImage.Dispose();
                }
            }
            initImage.Dispose();
        }

        private static void SaveImage(Image myBitmap, long quality, string savePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(savePath)))//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            }
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            Encoder myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);
            var myEncoderParameter = new EncoderParameter(myEncoder, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            myBitmap.Save(savePath, myImageCodecInfo, myEncoderParameters);
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
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
            var tempjpg = AppDomain.CurrentDomain.BaseDirectory + "ImageUpload\\tempcut.jpg";
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

                //通过连接创建Image对象
                Image oldimage = Image.FromFile(oldImagePath);
                var tempjpg = AppDomain.CurrentDomain.BaseDirectory + "ImageUpload\\tempthumb.jpg";
                if (!Directory.Exists(Path.GetDirectoryName(newImagePath)))//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newImagePath));
                }
                oldimage.Save(tempjpg);//把原图Copy一份出来,然后在temp.jpg上进行裁剪,最后把裁剪后的图片覆盖原图
                oldimage.Dispose();//一定要释放临时图片,要不之后的在此图上的操作会报错,原因冲突

                System.Drawing.Image image = System.Drawing.Image.FromFile(tempjpg); //利用Image对象装载源图像
                //接着创建一个System.Drawing.Bitmap对象，并设置你希望的缩略图的宽度和高度。
                int srcWidth = image.Width;
                int srcHeight = image.Height;
                int thumbWidth = newW;
                int thumbHeight = newH;
                Bitmap bmp = new Bitmap(thumbWidth, thumbHeight);
                //从Bitmap创建一个System.Drawing.Graphics对象，用来绘制高质量的缩小图。
                System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
                //设置 System.Drawing.Graphics对象的SmoothingMode属性为HighQuality
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                //下面这个也设成高质量
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                //下面这个设成High
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //把原始图像绘制成上面所设置宽高的缩小图
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
                gr.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);
                //保存图像，大功告成！
                bmp.Save(newImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                //释放资源
                bmp.Dispose();
                image.Dispose();
            }
        }

    }
}
