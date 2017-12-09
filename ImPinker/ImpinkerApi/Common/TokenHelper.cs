using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Security;
using ImBLL;
using ImModel;
using Maticsoft.Common;

namespace ImpinkerApi.Common
{
    public class TokenHelper
    {
        static Dictionary<string, string> _userTokenDic = new Dictionary<string, string>();
        static readonly UserBll _userBll=new UserBll();
        /// <summary>
        /// 添加或更新token，用户登录的时候返回token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string AddOrUpdateToken(string username)
        {
            var token = GenerateToken(username);
            //改为cache存储
            string cacheKey = "UserLoginToken-" + username;
            //object objModel = DataCache.GetCache(cacheKey);
            DataCache.SetCache(cacheKey, token, DateTime.Now.AddHours(24), TimeSpan.Zero);//默认缓存1天
            return token;
            
            //if (_userTokenDic.ContainsKey(username))
            //{
            //    _userTokenDic[username] = token;
            //}
            //else
            //{
            //    _userTokenDic.Add(username, token);
            //}
            //return token;
        }
        /// <summary>
        /// 根据用户名获取token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetToken(string username)
        {
            //改为cache存储
            string cacheKey = "UserLoginToken-" + username;
            string token = (string)DataCache.GetCache(cacheKey);
            if (token != null)
            {
                return token;
            }
            return "";

            //if (_userTokenDic.ContainsKey(username))
            //{
            //    return _userTokenDic[username];
            //}
            //return "";
        }
        /// <summary>
        /// 验证token是否有效
        /// </summary>
        /// <param name="username"></param>
        /// <param name="tokenstr"></param>
        /// <returns></returns>
        public static bool CheckUserToken(string username, string tokenstr)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(tokenstr))
            {
                var token = GetToken(username);
                if (!string.IsNullOrEmpty(token) && token.Equals(tokenstr))
                {
                    return true;
                }
            }
            return false;
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
        /// <summary>
        /// 根据header信息。获取用户信息。该方法的调用必须先进行权限验证
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static Users GetUserInfoByHeader(HttpRequestHeaders headers)
        {
            IEnumerable<string> usernames;
            IEnumerable<string> usertokens;
            var flag1 = headers.TryGetValues("username", out usernames);
            var flag2 = headers.TryGetValues("usertoken", out usertokens);
            if (flag1 && flag2)
            {
                var username = usernames.ToList()[0];
                var token = usertokens.ToList()[0];
                if (CheckUserToken(username, token))
                {
                    var user = _userBll.GetModelByUserName(username);
                    return user;
                }
            }
            return null;
        }
    }
}