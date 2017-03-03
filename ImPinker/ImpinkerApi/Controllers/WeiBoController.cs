using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel;
using ImpinkerApi.Models;

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
    }
}
