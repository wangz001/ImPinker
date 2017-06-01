using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Common.Utils;
using ImBLL;
using ImModel;
using ImpinkerApi.Common;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class SettingController : ApiController
    {
        /// <summary>
        /// 用户反馈
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> AddFeedback()
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
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/ImageUpload");
            var provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            var files = new List<string>();
            try
            {
                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.  
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    files.Add(Path.GetFileName(file.LocalFileName));
                }
                var model=InitFeedback(provider);
                model.ContentStr = string.Join(",", files);
                var flag = new FeedbackBll().AddFeedback(model);
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = flag?1:0,
                    Description = "ok",
                    Data = model
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
        private Feedback InitFeedback(CustomMultipartFormDataStreamProvider provider)
        {
            var formData = provider.FormData;
            var vm = new Feedback();
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            vm.UserIdentity = userinfo != null ? userinfo.Id.ToString(CultureInfo.InvariantCulture) : formData.Get("imei");
            if (formData.HasKeys())
            {
                //从header获取
                if (formData.AllKeys.Contains("ContactWay") && !string.IsNullOrEmpty(formData.Get("ContactWay")))
                {
                    var aa = formData.Get("ContactWay");
                    vm.ContactWay = aa;
                }
                if (formData.AllKeys.Contains("Description") && !string.IsNullOrEmpty(formData.Get("Description")))
                {
                    var aa = formData.Get("Description");
                    vm.Description = aa;
                }
            }
            return vm;
        }

    }
}
