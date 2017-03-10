using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using ImpinkerApi.Models;
using System.Threading.Tasks;
using System.Net;

namespace ImpinkerApi.Controllers
{
    public class UploadController : BaseApiController
    {
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
                    Data = e,
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
            {
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Description = "数据格式错误",
                    Data = HttpStatusCode.UnsupportedMediaType
                });
            }
            // Prepare CustomMultipartFormDataStreamProvider in which our multipart form  
            // data will be loaded.  
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/ImageUpload");
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
