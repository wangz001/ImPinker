using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Common.Utils;
using ImpinkerApi.Models;
using System.Threading.Tasks;
using System.Net;
using ImpinkerApi.Filters;
using ImpinkerApi.Common;
using System.Configuration;
using ImBLL;

namespace ImpinkerApi.Controllers
{
    public class UploadController : BaseApiController
    {
        private static readonly UserBll UserBll = new UserBll();
        readonly string _buckeyName = ConfigurationManager.AppSettings["MyautosOssBucket"];
        /// <summary>
        /// 头像上传
        /// </summary>
        /// <returns></returns>
        [TokenCheck]
        [HttpPost]
        public async Task<HttpResponseMessage> UserHeadimgUpload()
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
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/ImageUpload/headimage");
            if (!Directory.Exists(fileSaveLocation))//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(fileSaveLocation);
            }
            var provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            var files = new List<string>();
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
                var userid = userinfo.Id;
                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                    var flag = UserBll.UpdateHeadImage(_buckeyName,file.LocalFileName, userid);
                    if (flag)
                    {
                        break;
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                    {
                        IsSuccess = 0,
                        Description = "上传图片失败",
                        Data = ""
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Description = "ok",
                    Data = files
                });
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(e.ToString());
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Description = "上传游记图片出错",
                    Data = ""
                });
            }
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
                LogHelper.Instance.Error(e.ToString());
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Description = "上传图片出错",
                    Data = ""
                });
            }
        }
        
    }
}
