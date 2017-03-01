using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImpinkerApi.Common;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class AccountController : Controller
    {
        UserBll _userBll=new UserBll();
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
        
        public ActionResult Login(string username, string password)
        {
            var isSuccess = false;
            var description = "ok";
            var token = string.Empty;
            if (!string.IsNullOrEmpty(username)&&!string.IsNullOrEmpty(password))
            {
                var users = _userBll.GetModelByUserName(username);
                if (users==null)
                {
                    isSuccess = false;
                    description = "用户名不存在";
                }
                //username  用户名或者电话号码
                if (users!=null&&users.PassWord==password)
                {
                    isSuccess = true;
                    description = "登录成功";
                    var tokenStr = TokenHelper.AddOrUpdateToken(username);
                    token = tokenStr;
                }
                if (users != null && users.PassWord != password)
                {
                    isSuccess = false;
                    description = "密码错误";
                }
            }
            else
            {
                description = "用户名或密码不能为空";
            }
            return Json(new JsonResultViewModel
            {
                IsSuccess = isSuccess ? 1 : 0,
                Data = token,
                Description = description
            },JsonRequestBehavior.AllowGet);
        }
    }
}
