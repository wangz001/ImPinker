using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class UploadController : BaseApiController
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
        public HttpResponseMessage Upload(HttpPostedFileBase imgFile, int? resizeWidth, int? resizeHeight)
        {
            string msg = string.Empty;
            string error = string.Empty;
            string imgurl = string.Empty;
            if (imgFile != null && imgFile.ContentLength != 0)
            {
                if (!Directory.Exists(System.Web.Hosting.HostingEnvironment.MapPath("/imgtemp/")))//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(System.Web.Hosting.HostingEnvironment.MapPath("/imgtemp/"));
                }
                imgurl = string.Format("/imgtemp/{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmmss"),
                    Path.GetFileName(imgFile.FileName));
                imgFile.SaveAs(System.Web.Hosting.HostingEnvironment.MapPath("/") + imgurl);
                msg = " 成功! 文件大小为:" + imgFile.ContentLength;
                string res = "{ error:'" + error + "', msg:'" + msg + "',imgurl:'" + imgurl + "'}";
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = imgurl,
                    Description = msg
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = "",
                Description = "上传失败"
            });
        }
        /// <summary>
        /// 百度webupload 上传图片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage BaiduUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Data = "",
                    Description = "文件为空"
                });
            }
            try
            {
                var fileName = "/Upload/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName;
                using (var flieStream = new FileStream(System.Web.Hosting.HostingEnvironment.MapPath("~/") + fileName, FileMode.Create))
                {
                    file.InputStream.CopyTo(flieStream);
                }
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = new { filename = fileName },
                    Description = "上传出错"
                });
            }
            catch (Exception e)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Data = "",
                    Description = "上传出错"
                });
            }
        }
    }
}
