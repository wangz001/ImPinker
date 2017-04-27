using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel;
using ImpinkerApi.Common;
using ImPinkerApi.Common;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using ImModel.Enum;
using Common.AlyOssUtil;
using Common.Utils;
using System.Drawing.Imaging;
using System.Drawing;
using Common.DateTimeUtil;

namespace ImpinkerApi.Controllers
{
    public class WeiBoController : BaseApiController
    {
        readonly WeiBoBll _weiBoBll = new WeiBoBll();
        readonly UserBll _userBll = new UserBll();
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
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/ImageUpload/" + DateTime.Now.ToString("yyyyMMdd"));
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
                    //裁剪
                    var extention = Path.GetExtension(file.LocalFileName);
                    var sLocalPath = file.LocalFileName.Replace(extention, "_s.jpg");
                    var image = Image.FromFile(file.LocalFileName);
                    if (image.Width < image.Height)
                    {
                        ImageUtils.ImgReduceCutOut(0, 0, image.Width, image.Width * 2 / 3, file.LocalFileName, sLocalPath);
                        //生成缩略图
                        ImageUtils.ThumbnailImage(sLocalPath, sLocalPath, 300, 200, ImageFormat.Jpeg);
                    }
                    else
                    {
                        //生成缩略图
                        ImageUtils.ThumbnailImage(file.LocalFileName, sLocalPath, 300, 200, ImageFormat.Jpeg);
                    }
                    var sImgUrl = imgUrl.Replace(".jpg", "_s.jpg");
                    var flag2=ObjectOperate.UploadImage(BuckeyName, sLocalPath, sImgUrl);
                    if (ossSucess&&flag2)
                    {
                        files.Add(imgUrl);
                    }
                }
                if (files.Count > 0)
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
                if (formData.AllKeys.Contains("userid") && !string.IsNullOrEmpty(formData.Get("userid")))
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
        public HttpResponseMessage GetWeiBoList(int pageindex, int pagesize)
        {
            var list = _weiBoBll.GetListByPage(pageindex, pagesize);
            if (list == null || list.Count == 0)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Description = "暂无更多数据",
                    Data = null
                });
            }
            var resultList = new List<WeiBoListViewModel>();
            foreach (var weiBo in list)
            {
                var model = new WeiBoListViewModel
                {
                    Id=weiBo.Id,
                    UserId = weiBo.UserId,
                    Description = weiBo.Description,
                    ContentValue = weiBo.ContentValue,
                    Longitude = weiBo.Longitude,
                    Lantitude = weiBo.Lantitude,
                    Height = weiBo.Height,
                    LocationText = weiBo.LocationText,
                    PublishTime=TUtil.DateFormatToString(weiBo.CreateTime),
                    IsRePost = weiBo.IsRePost
                };
                var userinfo = _userBll.GetModelByCache(weiBo.UserId);
                model.UserName = !string.IsNullOrEmpty(userinfo.ShowName) ? userinfo.ShowName : userinfo.UserName;
                model.UserHeadImage = ImageUrlHelper.GetHeadImageUrl(userinfo.ImgUrl, 100);
                resultList.Add(model);
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Description = "ok",
                Data = resultList
            });
        }

        #endregion
    }
}
