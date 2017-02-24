using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImpinkerApi.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Index()
        {
            return null;
        }
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
        /// 百度webupload 上传图片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BaiduUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return Json(new { success = false });
            }
            try
            {
                var fileName = "/Upload/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;
                using (var flieStream = new FileStream(Server.MapPath("~/") + fileName, FileMode.Create))
                {
                    file.InputStream.CopyTo(flieStream);
                }
                return Json(new { success = true, fileName = fileName });
            }
            catch (Exception e)
            {
                return Json(new { success = false, errormes = e });
            }
        }
       
    }
}
