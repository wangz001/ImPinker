using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FastJSON;
using ImBLL;
using ImModel;
using ImPinker.Common;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using ImPinker.Models;
using Newtonsoft.Json;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;
using WebGrease;

namespace ImPinker.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        readonly UserBll _userBll = new UserBll();
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public string SendCheckNum(string phoneNum)
        {
            RemoveFromPhoneNumDic();//移除过期的记录，防止内存过大
            if (!CheckPhoneNum(phoneNum))
            {
                Response.ContentType = "application/json; charset=utf-8";  
                return JsonConvert.SerializeObject(new AjaxReturnViewModel
                {
                    IsSuccess = 0,
                    Description = "该手机号码已被注册，请直接登录",
                    Data = ""
                });
            }
            var checkNum = new Random().Next(100000, 999999);//验证码随机数生成
            var flag = Send(phoneNum, checkNum.ToString());
            if (flag)
            {
                var model = new CheckNumModel
                {
                    PhoneNum = phoneNum,
                    CheckNum = checkNum.ToString(),
                    SendTime = DateTime.Now
                };
                if (phoneNumDic.ContainsKey(phoneNum))
                {
                    phoneNumDic[phoneNum] = model;
                }
                else
                {
                    phoneNumDic.Add(phoneNum, model);
                }
                return JsonConvert.SerializeObject(new AjaxReturnViewModel
                {
                    IsSuccess = 1,
                    Description = "成功",
                    Data = ""
                });
            }
            return JsonConvert.SerializeObject(new AjaxReturnViewModel
            {
                IsSuccess = 0,
                Description = "发送验证码失败，再试一下",
                Data = ""
            });
        }
        #region 手机验证码
        private bool Send(string phoneNum, string checkNum)
        {
            string smsTemplateCode = ConfigurationManager.AppSettings["SmsTemplateCode"];
            const string url = "http://gw.api.taobao.com/router/rest";  //测试环境
            const string appkey = "23546735";
            const string secret = "5ccfde439d81c9ae0aeb2df33fa6421e";
            ITopClient client = new DefaultTopClient(url, appkey, secret);
            var req = new AlibabaAliqinFcSmsNumSendRequest();
            req.Extend = phoneNum;
            req.SmsType = "normal";
            req.SmsFreeSignName = "车酷网";
            req.SmsParam = string.Format("{{checknum:'{0}'}}", checkNum);
            req.RecNum = phoneNum;
            req.SmsTemplateCode = smsTemplateCode;
            AlibabaAliqinFcSmsNumSendResponse rsp = client.Execute(req);
            if (rsp.IsError)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 验证电话号码是否被注册， 是否1分钟内已发送过验证码等，后期要加上ip验证，防止恶意注册
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns>ture,可以继续注册；false：不可继续注册</returns>
        private bool CheckPhoneNum(string phoneNum)
        {
            if (phoneNumDic.ContainsKey(phoneNum))
            {
                var model = phoneNumDic[phoneNum];
                if ((DateTime.Now - model.SendTime).TotalSeconds < 80)//间隔时间小于80秒
                {
                    return false;
                }
            }
            var user = _userBll.GetModelByPhoneNum(phoneNum);
            if (user != null && user.PhoneNum.Length > 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 存储电话号码和验证码。注册成功后调用移除方法，超过30分钟的记录都移除。
        /// </summary>
        static Dictionary<string, CheckNumModel> phoneNumDic = new Dictionary<string, CheckNumModel>();

        /// <summary>
        /// 移除过期的记录
        /// </summary>
        private void RemoveFromPhoneNumDic()
        {
            if (phoneNumDic.Count > 0)
            {
                foreach (string key in phoneNumDic.Keys)
                {
                    var model = phoneNumDic[key];
                    if ((DateTime.Now - model.SendTime).TotalMinutes > 30)
                    {
                        phoneNumDic.Remove(key);
                    }
                }
            }
        }
        #endregion
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var phoneNum = model.PhoneNum;
                var checkNum = model.CheckNum;
                if (phoneNumDic.ContainsKey(phoneNum)
                    && (DateTime.Now - phoneNumDic[phoneNum].SendTime).TotalSeconds > 600)
                {
                    AddErrors(IdentityResult.Failed("手机验证码超时，10分钟内有效。请重新获取验证码"));
                    return View(model);
                }
                if (!(phoneNumDic.ContainsKey(phoneNum)
                    && phoneNumDic[phoneNum].CheckNum.Equals(checkNum)))
                {
                    AddErrors(IdentityResult.Failed("您输入的手机验证码有误"));
                    return View(model);
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {//注：aspnetuser  和localuser 分别在两个数据库中
                    AddLocalUser(user, phoneNum);
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }
            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        private void AddLocalUser(ApplicationUser user, string phoneNum)
        {
            var users = new Users
            {
                UserName = user.UserName,
                AspNetId = user.Id,
                PhoneNum = phoneNum,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                IsEnable = true
            };//自己维护的用户表
            var flag = _userBll.Add(users);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "你的密码已更改。"
                : message == ManageMessageId.SetPasswordSuccess ? "已设置你的密码。"
                : message == ManageMessageId.RemoveLoginSuccess ? "已删除外部登录名。"
                : message == ManageMessageId.Error ? "出现错误。"
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 请求重定向到外部登录提供程序
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // 从外部登录提供程序获取有关用户的信息
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region 帮助程序
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}