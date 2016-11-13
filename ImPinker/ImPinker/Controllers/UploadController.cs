using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using Microsoft.AspNet.Identity;

namespace ImPinker.Controllers
{
    public class UploadController : Controller
    {
        private static readonly UserBll UserBll = new UserBll();
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
            string imgurl=string.Empty;
            if (imgFile != null && imgFile.ContentLength != 0)
            {
                if (!Directory.Exists(Server.MapPath("/imgtemp/")))//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(Server.MapPath("/imgtemp/"));
                }
                imgurl =string.Format("/imgtemp/{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Path.GetFileName(imgFile.FileName));
                imgFile.SaveAs(Server.MapPath("/") +imgurl);
                msg = " 成功! 文件大小为:" + imgFile.ContentLength;
                string res = "{ error:'" + error + "', msg:'" + msg + "',imgurl:'" + imgurl + "'}";
                return Content(res);
            }
            return Content("{ error:'" + error + "', msg:'" + msg + "',imgurl:'" + imgurl + "'}");
        }
        /// <summary>
        ///  保存裁剪过后的图片
        /// </summary>
        /// <param name="path">图片url</param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveJscropImg(string path,int x1,int y1,int x2,int y2)
        {
            var sourcepath = Server.MapPath("/") + path;
            var newPath = string.Empty;
            if (System.IO.File.Exists(sourcepath))
            {
                var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
                var newimgUrl = string.Format("headimg/180/{1}_{0}.jpg", DateTime.Now.ToString("yyyyMMddHHmmss"), userId);
                newPath = (Server.MapPath("/") + newimgUrl);
                //裁切图片,保存为180X180格式
                ImgReduceCutOut(x1, y1, x2 - x1, y2 - y1, sourcepath, newPath);
                //缩放、保存为3种格式
                var img100 = newPath.Replace("headimg/180", "headimg/100");
                SaveImage(newPath, img100,100, 100, ImageFormat.Jpeg);
                var img40 = newPath.Replace("headimg/180", "headimg/40");
                SaveImage(newPath, img40, 40, 40, ImageFormat.Jpeg);
                //更新数据库
                UserBll.UpdateHeadImg(userId,newimgUrl);
            }
            return Content("{ error:'" + "" + "', msg:'" + "ok" + "',imgurl:'" + newPath.Replace(Server.MapPath("/"),"/") + "'}");
        }

        private void SaveImage(string oldImagePath, string newImagePath, int newW, int newH,
           ImageFormat imageFormat)
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
                EncoderParameters ep = new EncoderParameters();
                long level = 95L;
                ep.Param[0] = new EncoderParameter(Encoder.Quality, level);
                
                var bm = new Bitmap(oldImagePath);
                var cutImg = bm.GetThumbnailImage(newW, newH, (ThumbnailCallback), IntPtr.Zero);
                if (ici != null) cutImg.Save(newImagePath, ici, ep);
                cutImg.Dispose();
                bm.Dispose();
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
            int int_Standard_Width = 180;
            int int_Standard_Height = 180;

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