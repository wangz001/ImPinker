using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Utils;
using ImBLL;
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
            var headImageLimit = ConfigurationManager.AppSettings["HeadImageLimit"];
            var newPath = string.Empty;
            if (System.IO.File.Exists(sourcepath))
            {
                var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
                var newimgUrl = string.Format(headImageLimit, userId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                newPath = (Server.MapPath("/") + newimgUrl);
                //裁切图片,保存为180X180格式
                ImageUtils.ImgReduceCutOut(x1, y1, x2 - x1, y2 - y1, sourcepath, newPath);
                //缩放、保存为3种格式
                var img180 = newPath.Replace("headimg/limit", "headimg/180");
                ImageUtils.ThumbnailImage(newPath, img180, 180, 180, ImageFormat.Jpeg);
                var img100 = newPath.Replace("headimg/limit", "headimg/100");
                ImageUtils.ThumbnailImage(newPath, img100, 100, 100, ImageFormat.Jpeg);
                var img40 = newPath.Replace("headimg/limit", "headimg/40");
                ImageUtils.ThumbnailImage(newPath, img40, 40, 40, ImageFormat.Jpeg);
                //更新数据库
                UserBll.UpdateHeadImg(userId,newimgUrl);
            }
            return Content("{ error:'" + "" + "', msg:'" + "ok" + "',imgurl:'" + newPath.Replace(Server.MapPath("/"),"/") + "'}");
        }
        
    }
}