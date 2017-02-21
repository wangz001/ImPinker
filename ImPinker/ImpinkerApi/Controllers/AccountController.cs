using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        public JsonResult Index()
        {
            return Json(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = "aaa",
                Description = "ok"
            },JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var isSuccess = false;
            var description = "ok";
            if (!string.IsNullOrEmpty(username)&&!string.IsNullOrEmpty(password))
            {
                //username  用户名或者电话号码
                if (username=="admin"&&password=="123")
                {
                    isSuccess = true;
                }
                else
                {
                    description = "用户名或密码错误";
                }
            }
            else
            {
                description = "用户名或密码不能为空";
            }
            return Json(new JsonResultViewModel
            {
                IsSuccess = isSuccess ? 1 : 0,
                Data = "aaa",
                Description = description
            });
        }

    }
}
