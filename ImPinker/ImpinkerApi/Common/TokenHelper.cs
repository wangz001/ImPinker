using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Providers.Entities;
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
            if (_userTokenDic.ContainsKey(username))
            {
                var token = _userTokenDic[username];
                return token;
            }
            else
            {
                var token = GenerateToken(username);
                _userTokenDic.Add(username,token);
                return token;
            }
        }
        /// <summary>
        /// 根据用户名获取token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static  string GetToken(string username)
        {
            return "";
        }
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private static string GenerateToken(string username)
        {
            return DateTime.Now.Ticks.ToString();
        }
    }
}