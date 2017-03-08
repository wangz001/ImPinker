using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel;
using ImpinkerApi.Models;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using ImModel.Enum;
using Common.AlyOssUtil;

namespace ImpinkerApi.Controllers
{
    public class WeiBoController : BaseApiController
    {
        readonly WeiBoBll _weiBoBll=new WeiBoBll();
        /// <summary>
        /// 创建微博
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Add(NewWeiBoViewModel vm)
        {
            if (vm!=null&&(!string.IsNullOrEmpty(vm.Description)||!string.IsNullOrEmpty(vm.ContentValue)))
            {
                var model = new WeiBo
                {
                    UserId = vm.UserId,
                    Description = vm.Description,
                    ContentValue = vm.ContentValue,
                    ContentType = vm.ContentType,
                    Longitude = vm.Longitude,
                    Lantitude = vm.Lantitude,
                    Height = vm.Height,
                    LocationText = vm.LocationText,
                    HardWareType = vm.HardWareType,
                    IsRePost = false,
                    CreateTime = DateTime.Now
                };
                long weiboId=_weiBoBll.AddWeiBo(model);
                if (weiboId>0)
                {
                    return GetJson(new JsonResultViewModel
                    {
                        Data = weiboId,
                        IsSuccess = 1,
                        Description = "ok"
                    });
                }
            }

            return GetJson(new JsonResultViewModel
            {
                Data = "",
                IsSuccess = 10,
                Description = "error"
            });
        }

        /// <summary>
        /// mui 多图上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> NewWeibo()
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
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/ImageUpload/"+DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(fileSaveLocation))
            {
                Directory.CreateDirectory(fileSaveLocation);
            }
            var provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);

            string buckeyName = "myautos";
            try
            {
                var rootPath = HttpContext.Current.Server.MapPath("~/ImageUpload");
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.  
                await Request.Content.ReadAsMultipartAsync(provider);

                //封装数据
                var weiboVm = InitWeiBoData(provider);
                List<string> files = new List<string>();
                foreach (MultipartFileData file in provider.FileData)
                {
                    string ImgUrlformat = "weiboimage/{0}/{1}_{2}.jpg";
                    var imgUrl = string.Format(ImgUrlformat, DateTime.Now.ToString("yyyyMMdd"), weiboVm.UserId, DateTime.Now.Ticks);
                    //上传到oss
                    var ossSucess = ObjectOperate.UploadImage(buckeyName, file.LocalFileName, imgUrl);
                    if (ossSucess)
                    {
                        files.Add(imgUrl);
                    }
                }
                weiboVm.ContentValue = string.Join(",", files);
                //数据库操作

                // Send OK Response along with saved file names to the client.  
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Description = "ok",
                    Data = weiboVm
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
                //var fileName = "weiboimage/" + DateTime.Now.ToString("yyyyMMdd") + "/" + headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var type =Path.GetExtension(headers.ContentDisposition.FileName.Replace("\"", string.Empty));
                var sb = new StringBuilder(( DateTime.Now.Ticks.ToString()).Replace("\"", "").Trim().Replace(" ", "_")+type);
                Array.ForEach(Path.GetInvalidFileNameChars(), invalidChar => sb.Replace(invalidChar, '-'));
                return sb.ToString();

            }
        }

        private NewWeiBoViewModel InitWeiBoData(CustomMultipartFormDataStreamProvider provider)
        {
            var vm = new NewWeiBoViewModel();
            var formData = provider.FormData;
            if (formData.HasKeys())
            {
                //从header获取
                if (formData.AllKeys.Contains("userid")&&!string.IsNullOrEmpty(formData.Get("userid")))
                {
                    var aa = formData.Get("userid");
                    vm.UserId = Int32.Parse(aa);
                }
                if (formData.AllKeys.Contains("Description") && !string.IsNullOrEmpty(formData.Get("Description")))
                {
                    var aa = formData.Get("Description");
                    vm.Description = aa;
                }
                if (formData.AllKeys.Contains("Longitude") && !string.IsNullOrEmpty(formData.Get("Longitude")))
                {
                    var aa = formData.Get("Longitude");
                    vm.Longitude = decimal.Parse(aa);
                }
                if (formData.AllKeys.Contains("Latitude") && !string.IsNullOrEmpty(formData.Get("Latitude")))
                {
                    var aa = formData.Get("Latitude");
                    vm.Lantitude = decimal.Parse(aa);
                }
                if (formData.AllKeys.Contains("Height") && !string.IsNullOrEmpty(formData.Get("Height")))
                {
                    var aa = formData.Get("Height");
                    vm.Height = decimal.Parse(aa);
                }
                if (formData.AllKeys.Contains("LocationText") && !string.IsNullOrEmpty(formData.Get("LocationText")))
                {
                    var aa = formData.Get("LocationText");
                    vm.LocationText = aa;
                }
                if (formData.AllKeys.Contains("HardWareType") && !string.IsNullOrEmpty(formData.Get("HardWareType")))
                {
                    var aa = formData.Get("HardWareType");
                    vm.HardWareType = aa;
                }
                vm.IsRePost = false;
                vm.ContentType = WeiBoContentTypeEnum.Image;
                vm.State = WeiBoStateEnum.Normal;
            }
            return vm;
        }
    
    }
}
