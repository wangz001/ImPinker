using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ImpinkerApi.Models;
using System.Threading.Tasks;
using System.Collections;
using System.Net;

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

        /// <summary>
        /// mui 多图上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<Hashtable> ImgUpload()
        {
            // 检查是否是 multipart/form-data 
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                throw new System.Web.Http.HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            //文件保存目录路径 
            string SaveTempPath = "~/SayPlaces/" + "/SayPic/SayPicTemp/";
            String dirTempPath = HttpContext.Current.Server.MapPath(SaveTempPath);
            // 设置上传目录 
            var provider = new MultipartFormDataStreamProvider(dirTempPath);
            //var queryp = Request.GetQueryNameValuePairs();//获得查询字符串的键值集合 
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<Hashtable>(o =>
                {
                    Hashtable hash = new Hashtable();
                    hash["error"] = 1;
                    hash["errmsg"] = "上传出错";
                    var file = provider.FileData[0];//provider.FormData 
                    string orfilename = file.Headers.ContentDisposition.FileName.TrimStart('"').TrimEnd('"');
                    FileInfo fileinfo = new FileInfo(file.LocalFileName);
                    //最大文件大小 
                    int maxSize = 10000000;
                    if (fileinfo.Length <= 0)
                    {
                        hash["error"] = 1;
                        hash["errmsg"] = "请选择上传文件。";
                    }
                    else if (fileinfo.Length > maxSize)
                    {
                        hash["error"] = 1;
                        hash["errmsg"] = "上传文件大小超过限制。";
                    }
                    else
                    {
                        string fileExt = orfilename.Substring(orfilename.LastIndexOf('.'));
                        //定义允许上传的文件扩展名 
                        String fileTypes = "gif,jpg,jpeg,png,bmp";
                        if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(fileTypes.Split(','), fileExt.Substring(1).ToLower()) == -1)
                        {
                            hash["error"] = 1;
                            hash["errmsg"] = "上传文件扩展名是不允许的扩展名。";
                        }
                        else
                        {
                            String ymd = DateTime.Now.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                            String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                            fileinfo.CopyTo(Path.Combine(dirTempPath, newFileName + fileExt), true);
                            fileinfo.Delete();
                            hash["error"] = 0;
                            hash["errmsg"] = "上传成功";
                        }
                    }
                    return hash;
                });
            return task;
        }
    }
}
