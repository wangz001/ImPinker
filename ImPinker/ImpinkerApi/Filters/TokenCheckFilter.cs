using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using ImpinkerApi.Common;

namespace ImpinkerApi.Filters
{
    public class TokenCheckAttribute : AuthorizeAttribute 
    {

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            
            //从http请求的头里面获取身份验证信息，验证是否是请求发起方的ticket
            var authorization = actionContext.Request.Headers.Authorization;
            if ((authorization != null) && (authorization.Parameter != null))
            {
                //解密用户ticket,并校验用户名密码是否匹配
                var encryptTicket = authorization.Parameter;
                if (ValidateTicket(encryptTicket))
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

        /// <summary>
        /// 这里主要是授权验证的逻辑处理，返回true的则是通过授权，返回了false则不是。
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public bool ValidateTicket(string ticket)
        {
            var username = "username";
            var tokenparam = "token";
            if (!string.IsNullOrEmpty(username)&&!string.IsNullOrEmpty(tokenparam))
            {
                var token = TokenHelper.GetToken(username);
                if (!string.IsNullOrEmpty(token)&&token.Equals(tokenparam))
                {
                    return true;
                }
            }
            return false;
        }


    }
}