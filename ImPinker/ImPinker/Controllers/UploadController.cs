using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.AlyOssUtil;
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
            string imgurl = string.Empty;
            if (imgFile != null && imgFile.ContentLength != 0)
            {
                if (!Directory.Exists(Server.MapPath("/imgtemp/")))//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(Server.MapPath("/imgtemp/"));
                }
                imgurl = string.Format("/imgtemp/{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Path.GetFileName(imgFile.FileName));
                imgFile.SaveAs(Server.MapPath("/") + imgurl);
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
        public ActionResult SaveJscropImg(string path, int x1, int y1, int x2, int y2)
        {
            var sourcepath = Server.MapPath("/") + path;
            //headimg/limit/{0}/{1}_{2}.jpg
            var headImageLimit = ConfigurationManager.AppSettings["HeadImageLimit"];
            var limitPath = string.Empty;
            if (System.IO.File.Exists(sourcepath))
            {
                try
                {
                    var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
                    var limitimgUrl = string.Format(headImageLimit, DateTime.Now.ToString("yyyyMMdd"), userId, DateTime.Now.ToString("yyyyMMddHHmmss"));
                    limitPath = (Server.MapPath("/") + limitimgUrl);
                    //裁切图片
                    ImageUtils.ImgReduceCutOut(x1, y1, x2 - x1, y2 - y1, sourcepath, limitPath);
                    var flag=UploadToOss(limitPath, limitimgUrl);
                    //缩放、保存为3种格式
                    int[] imgSize = {180,100,40};
                    foreach (var size in imgSize)
                    {
                        var tempimgUrl = limitimgUrl.Replace("headimg/limit", "headimg/" + size);
                        var tempimgPath = (Server.MapPath("/") + tempimgUrl);
                        ImageUtils.ThumbnailImage(limitPath, tempimgPath, size, size, ImageFormat.Jpeg);
                        flag=UploadToOss(tempimgPath, tempimgUrl);
                        if (!flag)
                        {
                            //error
                            break;
                        }
                        //删除本地文件
                        System.IO.File.Delete(tempimgPath);
                    }
                    //更新数据库
                    UserBll.UpdateHeadImg(userId, limitimgUrl);
                }
                catch (Exception e)
                {
                    return Content("{ error:'" + e + "', msg:'" + "error" + "',imgurl:''}");
                }
            }
            return Content("{ error:'" + "" + "', msg:'" + "ok" + "',imgurl:'" + limitPath.Replace(Server.MapPath("/"), "/") + "'}");
        }
        
        /// <summary>
        /// 上传到oss
        /// </summary>
        /// <param name="sourcePath">绝对路径</param>
        /// <param name="newImgUrl">上传到oss的路径：headimg/limit/{0}/{1}_{2}.jpg</param>
        /// <returns></returns>
        private bool UploadToOss(string sourcePath,string newImgUrl)
        {
            string buckeyName = "myautos";
            var flag = ObjectOperate.UploadImage(buckeyName, sourcePath, newImgUrl);
            return flag;
        }

    }
}