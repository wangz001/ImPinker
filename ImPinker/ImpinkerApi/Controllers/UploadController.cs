using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public async Task<HttpResponseMessage> ImgUpload()
        {
            // 检查是否是 multipart/form-data 
            if (!Request.Content.IsMimeMultipartContent("form-data"))
                throw new System.Web.Http.HttpResponseException(HttpStatusCode.UnsupportedMediaType);
           
            // Prepare CustomMultipartFormDataStreamProvider in which our multipart form  
            // data will be loaded.  
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            List<string> files = new List<string>();

            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.  
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                }

                // Send OK Response along with saved file names to the client.  
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Description = "ok",
                    Data = files
                });
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }  
        }
        /// <summary>
        /// 创建一个 Provider 用于重命名接收到的文件 
        /// </summary>
        public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }  
    }
}
