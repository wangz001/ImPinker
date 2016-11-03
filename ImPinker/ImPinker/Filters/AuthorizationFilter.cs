using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Security;
using ImPinker.Models;
using Microsoft.AspNet.Identity;

namespace ImPinker.Filters
{
    public class AuthorizationFilter : FilterAttribute,IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //[ValidateAntiForgeryToken]   貌似更好使。不容易被伪造。待定
            var userid = HttpContext.Current.User.Identity.GetUserId();
            var isLogin=HttpContext.Current.User.Identity.IsAuthenticated;
            if (!isLogin)
            {
                filterContext.Result = new RedirectResult("/Account/Login"); 
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //filterContext.Result = new RedirectResult("/Account/Login"); 
        }
    }
}