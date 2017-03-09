using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.Expressions;

namespace ImpinkerApi.Models
{
    /// <summary>
    /// 用户登录、注册、找回密码、验证码模型
    /// </summary>
    public class UserLoginViewModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        
    }

    public class UserRegistViewModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 密码2
        /// </summary>
        public string Password2 { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNum { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string CheckNum { get; set; }
    }

    public class FindPasswordViewModel
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNum { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string CheckNum { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 密码2
        /// </summary>
        public string Password2 { get; set; }
    }

    public class LoginByPhoneViewModel
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNum { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string CheckNum { get; set; }
    }

    public class SendCheckNumViewModel
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNum { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string CheckNum { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public int OpreateType { get; set; }
    }
    
}