using System;
using System.Web.Http.Filters;
using Common.Exceptions;

namespace ImpinkerApi.Filters
{
    [AttributeUsage(AttributeTargets.All)]
    public class ExceptionContextFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            ExceptionHelper.Error_Logger(actionExecutedContext.Exception);
            base.OnException(actionExecutedContext);
        }
    }
}