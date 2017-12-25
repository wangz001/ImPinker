using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Security;
using ImBLL;
using ImModel;

namespace ImpinkerApi.Common
{
    public class TokenHelper
    {
        static readonly UserBll UserBll=new UserBll();
        static readonly UserTokenBll UserTokenBll=new UserTokenBll();
        /// <summary>
        /// 添加或更新token，用户登录的时候返回token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string AddOrUpdateToken(string username)
        {
            var user = UserBll.GetModelByUserName(username);
            var token = GenerateToken(username);
            var flag=UserTokenBll.Update(user.Id, token);
            return token;
        }
        /// <summary>
        /// 根据用户名获取token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetToken(string username)
        {
            var token = UserTokenBll.GetTokenStr(username);
            return token;
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
                    var user = UserBll.GetModelByUserName(username);
                    return user;
                }
            }
            return null;
        }
    }
}