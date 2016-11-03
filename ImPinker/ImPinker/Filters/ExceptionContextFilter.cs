using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Exceptions;

namespace ImPinker.Filters
{
    public class ExceptionContextFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            ExceptionHelper.Error_Logger(filterContext.Exception);
        }
    }
}