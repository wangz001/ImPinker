using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls.Expressions;
using ImModel;

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
    /// <summary>
    /// 第三方登录viewmodel
    /// </summary>
    public class LoginOauthViewModel
    {
        /// <summary>
        /// 第三方登录类型
        /// </summary>
        public string OauthType { get; set; }
        /// <summary>
        /// 第三方登录id，在user表中存在username字段
        /// </summary>
        public string OpenId { get; set; }

        public string ShowName { get; set; }

        public string HeadImage { get; set; }

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
        /// 操作类型
        /// </summary>
        public int OpreateType { get; set; }
    }


    /// <summary>
    /// 返回用户详细信息类
    /// </summary>
    public class UserReturnViewModel
    {
        /// <summary>
        /// 登录成功后，生成的token
        /// </summary>
        public string Token { get; set; }

        public int Id { get; set; }

        public string UserName { get; set; }

        public string ShowName { get; set; }
        public string ImgUrl { get; set; }
        public string PhoneNum { get; set; }
        public string Email { get; set; }
        public int Sex { get; set; }
    }
    
}