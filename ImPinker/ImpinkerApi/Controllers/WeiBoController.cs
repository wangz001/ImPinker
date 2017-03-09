using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel;
using ImpinkerApi.Common;
using ImpinkerApi.Filters;
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
        const string BuckeyName = "myautos";

        #region 新建微博(图文)
        /// <summary>
        /// mui 多图上传.创建微博
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TokenCheck]
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

            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.  
                await Request.Content.ReadAsMultipartAsync(provider);

                //封装数据
                var weiboModel = InitWeiBoData(provider);
                var files = new List<string>();
                foreach (MultipartFileData file in provider.FileData)
                {
                    const string imgUrlformat = "weiboimage/{0}/{1}_{2}.jpg";
                    var imgUrl = string.Format(imgUrlformat, DateTime.Now.ToString("yyyyMMdd"), weiboModel.UserId, DateTime.Now.Ticks);
                    //上传到oss
                    var ossSucess = ObjectOperate.UploadImage(BuckeyName, file.LocalFileName, imgUrl);
                    if (ossSucess)
                    {
                        files.Add(imgUrl);
                    }
                }
                if (files.Count>0)
                {
                    weiboModel.ContentValue = string.Join(",", files);
                }
                
                //数据库操作
                var flag = _weiBoBll.AddWeiBo(weiboModel);
                if (flag)
                {
                    // Send OK Response along with saved file names to the client.  
                    return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                    {
                        IsSuccess = 1,
                        Description = "ok",
                        Data = weiboModel
                    });
                }
                //记录日志
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
            return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
            {
                IsSuccess = 0,
                Description = "上传失败",
                Data = ""
            });
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

        private WeiBo InitWeiBoData(CustomMultipartFormDataStreamProvider provider)
        {
            var vm = new WeiBo();
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            if (userinfo == null)
            {
                return null;
            }
            vm.UserId = userinfo.Id;
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
                vm.CreateTime = DateTime.Now;
                vm.UpdateTime = DateTime.Now;
            }
            return vm;
        }
        #endregion

        #region 获取微博列表
        /// <summary>
        /// 获取微博列表
        /// </summary>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">数量</param>
        /// <returns></returns>
        public HttpResponseMessage GetWeiBoList(int pageindex,int pagesize)
        {


            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Description = "ok",
                Data = ""
            });
        }

        #endregion
    }
}
