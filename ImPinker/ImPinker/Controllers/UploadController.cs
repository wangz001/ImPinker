using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImPinker.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase imgFile, int? resizeWidth, int? resizeHeight)
        {
            string msg = string.Empty;
            string error = string.Empty;
            string imgurl;
            if (imgFile != null && imgFile.ContentLength != 0)
            {
                if (!Directory.Exists(Server.MapPath("/imgtemp/")))//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(Server.MapPath("/imgtemp/"));
                }
                imgFile.SaveAs(Server.MapPath("/imgtemp/") + Path.GetFileName(imgFile.FileName));
                msg = " 成功! 文件大小为:" + imgFile.ContentLength;
                imgurl = "/imgtemp/" + imgFile.FileName;
                string res = "{ error:'" + error + "', msg:'" + msg + "',imgurl:'" + imgurl + "'}";
                return Content(res);
            }
            return Content("test");
        }
        /// <summary>
        /// 保存图片，裁剪过后的
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveJscropImg(string path,int x1,int y1,int x2,int y2)
        {
            var sourcepath = Server.MapPath("/") + path;
            var newPath = string.Empty;
            if (System.IO.File.Exists(sourcepath))
            {
                newPath = (Server.MapPath("/") + string.Format("headimg/100/{1}_{0}.jpg", DateTime.Now.ToString("yyyyMMddHHmmss"), "25"));
                //SaveImage(sourcepath, newPath, x2 - x1, y2 - y1, ImageFormat.Jpeg);
                ImgReduceCutOut(x1, y1, x2 - x1, y2 - y1, sourcepath, newPath);
            }
            return Content("{ error:'" + "" + "', msg:'" + "ok" + "',imgurl:'" + newPath.Replace(Server.MapPath("/"),"/") + "'}");
        }

        private void SaveImage(string oldImagePath, string newImagePath, int newW, int newH,
           ImageFormat imageFormat)
        {
            if (System.IO.File.Exists(oldImagePath))
            {
                Bitmap bitmap = null;
                try
                {
                    var image = Image.FromFile(oldImagePath);
                    bitmap = new Bitmap(image, newW, newH);
                    Color pixel;//颜色匹对
                    for (int i = 0; i < newW; i++)
                    {
                        for (int j = 0; j < newH; j++)
                        {
                            pixel = bitmap.GetPixel(i, j);//获取旧图片的颜色值（ARGB存储方式）
                            int r, g, b, a;
                            r = pixel.R;
                            g = pixel.G;
                            b = pixel.B;
                            a = pixel.A;

                            //白色RGB(255，255，255),黑色（0,0,0）


                            if (!Equals(imageFormat, ImageFormat.Png))   //png格式透明底不处理
                            {
                                if (a == 0)
                                {
                                    bitmap.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                                }
                            }
                        }
                    }
                    bitmap.Save(newImagePath, imageFormat);
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                finally
                {
                    bitmap.Dispose();
                }
            }

        }
       
        
        /// <summary>
        /// 图片裁剪方法
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="int_Width"></param>
        /// <param name="int_Height"></param>
        /// <param name="input_ImgUrl"></param>
        /// <param name="out_ImgUrl"></param>
        public void ImgReduceCutOut(int startX, int startY, int int_Width, int int_Height, string input_ImgUrl, string out_ImgUrl)
        {
            // 上传标准图大小
            int int_Standard_Width = 150;
            int int_Standard_Height = 150;

            //其实在图片裁剪过程中还可以传更多的参数,如把原始图片缩小了再进行裁剪.本实例中是裁剪后,再裁剪后的图片缩小成150X80
            //通过连接创建Image对象
            Image oldimage = Image.FromFile(input_ImgUrl);
            oldimage.Save(Server.MapPath("temp.jpg"));//把原图Copy一份出来,然后在temp.jpg上进行裁剪,最后把裁剪后的图片覆盖原图
            oldimage.Dispose();//一定要释放临时图片,要不之后的在此图上的操作会报错,原因冲突
            Bitmap bm = new Bitmap(Server.MapPath("temp.jpg"));
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
            EncoderParameters ep = new EncoderParameters();
            long level = 95L;
            ep.Param[0] = new EncoderParameter(Encoder.Quality, level);

            // 裁剪图片
            Rectangle cloneRect = new Rectangle(startX, startY, int_Width, int_Height);
            PixelFormat format = bm.PixelFormat;
            Bitmap cloneBitmap = bm.Clone(cloneRect, format);
            if (int_Width > int_Standard_Width)
            {
                //缩小图片
                Image cutImg = cloneBitmap.GetThumbnailImage(int_Standard_Width, int_Standard_Height, (ThumbnailCallback), IntPtr.Zero);
                cutImg.Save(out_ImgUrl, ici, ep);
                cutImg.Dispose();
            }
            else
            {
                //保存图片
                cloneBitmap.Save(out_ImgUrl, ici, ep);
            }
            cloneBitmap.Dispose();
            bm.Dispose();
        }

        public bool ThumbnailCallback()
        {
            return false;
        }
    }
}