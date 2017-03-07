using System.Net.Http;
using System.Web.Http;
using ImBLL;
using ImpinkerApi.Common;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class AccountController : BaseApiController
    {
        readonly UserBll _userBll = new UserBll();

        /// <summary>
        /// 登录验证。登录成功，返回token
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Login(UserLoginViewModel loginViewModel)
        {
            var username = loginViewModel.Username;
            var password = loginViewModel.Password;
            var isSuccess = false;
            var description = string.Empty;
            var token = string.Empty;
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                //username  用户名或者电话号码
                var users = _userBll.GetModelByUserName(username);
                if (users == null)
                {
                    description = "用户不存在";
                }
                if (users != null && users.PassWord != password)
                {
                    description = "密码错误";
                }
                if (users != null && users.PassWord == password)
                {
                    isSuccess = true;
                    description = "登录成功";
                    var tokenStr = TokenHelper.AddOrUpdateToken(username);
                    token = tokenStr;
                }
            }
            else
            {
                description = "用户名或密码不能为空";
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = isSuccess ? 1 : 0,
                Data = token,
                Description = description
            });
        }
    }
}
