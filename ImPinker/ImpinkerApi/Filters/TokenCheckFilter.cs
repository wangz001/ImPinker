using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using ImpinkerApi.Common;

namespace ImpinkerApi.Filters
{
    public class TokenCheckAttribute : AuthorizeAttribute 
    {
        /// <summary>
        /// token验证
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            //从http请求的头里面获取身份验证信息，验证是否是请求发起方的ticket
            IEnumerable<string> usernames;
            IEnumerable<string> usertokens;
            var flag1 = actionContext.Request.Headers.TryGetValues("username", out usernames);
            var flag2 = actionContext.Request.Headers.TryGetValues("usertoken", out usertokens);
            if (flag1&&flag2)
            {
                var username = usernames.ToList()[0];
                var token = usertokens.ToList()[0];
                //解密用户ticket,并校验用户名密码是否匹配
                if (TokenHelper.CheckUserToken(username,token))
                {
                    base.IsAuthorized(actionContext);
                }
                else
                {
                    HandleUnauthorizedRequest(actionContext);
                }
            }
            //如果取不到身份验证信息，并且不允许匿名访问，则返回未验证401
            else
            {
                var attributes = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>();
                bool isAnonymous = attributes.Any(a => a is AllowAnonymousAttribute);
                if (isAnonymous) base.OnAuthorization(actionContext);
                else HandleUnauthorizedRequest(actionContext);
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            base.HandleUnauthorizedRequest(actionContext);
            var challengeMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            challengeMessage.Headers.Add("www-Authenticate", "basic");
            throw new HttpResponseException(challengeMessage);
        }

        
    }
}