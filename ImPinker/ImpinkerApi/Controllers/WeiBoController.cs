using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImModel;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class WeiBoController : Controller
    {
        WeiBoBll _weiBoBll=new WeiBoBll();
        //
        // GET: /WeiBo/

        public ActionResult Index()
        {
            return null;
        }

        public ActionResult Add(NewWeiBoViewModel vm)
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
                    return Json(new JsonResultViewModel
                    {
                        Data = weiboId,
                        IsSuccess = 1,
                        Description = "ok"
                    });
                }
            }

            return Json(new JsonResultViewModel
            {
                Data = "",
                IsSuccess = 10,
                Description = "error"
            });
        }
    }
}
