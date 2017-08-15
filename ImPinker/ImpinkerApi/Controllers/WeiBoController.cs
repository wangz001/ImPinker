using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Common.Utils;
using ImBLL;
using ImModel;
using ImpinkerApi.Common;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using ImModel.Enum;
using Common.DateTimeUtil;

namespace ImpinkerApi.Controllers
{
    public class WeiBoController : BaseApiController
    {
        readonly WeiBoBll _weiBoBll = new WeiBoBll();
        readonly UserBll _userBll = new UserBll();
        readonly string _buckeyName = ConfigurationManager.AppSettings["MyautosOssBucket"];

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
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/ImageUpload/weiboimage/" + DateTime.Now.ToString("yyyyMMdd"));
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
                    //上传原图和缩略图到oss
                    string imgUrl = _weiBoBll.UploadWeiBoimgToOss(_buckeyName, weiboModel.UserId, file.LocalFileName);
                    if (string.IsNullOrEmpty(imgUrl))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                        {
                            IsSuccess = 0,
                            Description = "上传图片到oss失败，图片大小" ,
                            Data = file.LocalFileName
                        });
                    }
                    files.Add(imgUrl);
                }
                if (files.Count > 0)
                {
                    weiboModel.ContentValue = string.Join(",", files);
                }

                //数据库操作
                var flag = _weiBoBll.AddWeiBo(weiboModel);
                if (flag)
                {
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
                LogHelper.Instance.Error(e.ToString());
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Description = "上传游记图片出错",
                    Data = ""
                });
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
                    Id = weiBo.Id,
                    UserId = weiBo.UserId,
                    Description = weiBo.Description,
                    ContentValue = ImageUrlHelper.GetWeiboFullImageUrl(weiBo.ContentValue,240),
                    Longitude = weiBo.Longitude,
                    Lantitude = weiBo.Lantitude,
                    Height = weiBo.Height,
                    LocationText = weiBo.LocationText,
                    PublishTime = TUtil.DateFormatToString(weiBo.CreateTime),
                    IsRePost = weiBo.IsRePost,
                    VoteCount=weiBo.VoteCount,
                    CommentCount=weiBo.CommentCount
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

        #region 获取我的微博
        [HttpGet]
        [TokenCheck]
        public HttpResponseMessage GetMyWeiBoList(int pageindex, int pagesize)
        {
            var userid = TokenHelper.GetUserInfoByHeader(Request.Headers).Id;
            var list = _weiBoBll.GetListByPage(userid, pageindex, pagesize);
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
                    Id = weiBo.Id,
                    UserId = weiBo.UserId,
                    Description = weiBo.Description,
                    ContentValue = ImageUrlHelper.GetWeiboFullImageUrl(weiBo.ContentValue, 240),
                    Longitude = weiBo.Longitude,
                    Lantitude = weiBo.Lantitude,
                    Height = weiBo.Height,
                    LocationText = weiBo.LocationText,
                    PublishTime = TUtil.DateFormatToString(weiBo.CreateTime),
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

        #region 获取微博信息
        [HttpGet]
        public HttpResponseMessage GetWeiBoById(int weiboid)
        {
            var weiBo = _weiBoBll.GetById(weiboid);
            var userinfo = _userBll.GetModelByCache(weiBo.UserId);
            var vm = new WeiBoListViewModel
            {
                Id = weiBo.Id,
                UserId = weiBo.UserId,
                UserName=string.IsNullOrEmpty(userinfo.ShowName)? userinfo.UserName:userinfo.ShowName,
                UserHeadImage=ImageUrlHelper.GetHeadImageUrl(userinfo.ImgUrl,100),
                Description = weiBo.Description,
                ContentValue = ImageUrlHelper.GetWeiboFullImageUrl(weiBo.ContentValue, 240),
                Longitude = weiBo.Longitude,
                Lantitude = weiBo.Lantitude,
                Height = weiBo.Height,
                LocationText = weiBo.LocationText,
                PublishTime = TUtil.DateFormatToString(weiBo.CreateTime),
                IsRePost = weiBo.IsRePost
            };
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = vm != null ? 1 : 0,
                Description = "ok",
                Data = vm
            });
        }

        #endregion
    }
}
