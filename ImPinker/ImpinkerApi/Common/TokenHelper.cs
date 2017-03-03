using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Providers.Entities;
using System.Web.Security;
using ImpinkerApi.Models;

namespace ImpinkerApi.Common
{
    public class TokenHelper
    {
        static Dictionary<string, string> _userTokenDic = new Dictionary<string, string>();

        /// <summary>
        /// 添加或更新token，用户登录的时候返回token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string AddOrUpdateToken(string username)
        {
            var token = GenerateToken(username);
            if (_userTokenDic.ContainsKey(username))
            {
                _userTokenDic[username] = token;
            }
            else
            {
                _userTokenDic.Add(username, token);
            }
            return token;
        }
        /// <summary>
        /// 根据用户名获取token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetToken(string username)
        {
            if (_userTokenDic.ContainsKey(username))
            {
                return _userTokenDic[username];
            }
            return "";
        }
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private static string GenerateToken(string username)
        {
            // ReSharper disable once CSharpWarnings::CS0618
            var token = FormsAuthentication.HashPasswordForStoringInConfigFile(username + DateTime.Now.Ticks, "MD5");
            return token;
        }
    }
}