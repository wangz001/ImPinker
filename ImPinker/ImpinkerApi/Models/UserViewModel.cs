using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImpinkerApi.Models
{
    /// <summary>
    /// 用户登录模型
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
}