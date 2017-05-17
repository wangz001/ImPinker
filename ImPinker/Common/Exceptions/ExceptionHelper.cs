using System;
using System.Collections.Specialized;
using System.Web;
using BitAuto.Utils;
using log4net;

namespace Common.Exceptions
{
	public class ExceptionHelper
	{
		private const string MUrlRewriterServerVar = "HTTP_X_ORIGINAL_URL";
        private static readonly ILog Logger = LogManager.GetLogger("WebLogger");

		public static void Error_Logger(Exception exception)
		{
			//Simple record Http 404 errors
			HttpException httpException = exception as HttpException;
			if (httpException != null && httpException.GetHttpCode() == 404)
			{
				string warnMessage = string.Format(
					"Http 404{0}IP:{1}{0}Url:{2}{0}Referer:{3}",
					Environment.NewLine,
					WebUtil.GetClientIP(),
					GetRequestUrl(),
					GetUrlReferrer()
				);
				Logger.Warn(warnMessage, httpException);
				return;
			}

			string errorMessage = string.Format(
				"{0}IP:{1}{0}Url:{2}{0}Referer:{3}",
				Environment.NewLine,
				WebUtil.GetClientIP(),
				GetRequestUrl(),
				GetUrlReferrer()
			);
			Logger.Error(errorMessage, exception);
		}

		/// <summary>
		/// 获取请求的Url地址
		/// 如果地址被重写过，返回重写前的地址
		/// </summary>
		/// <returns></returns>
		private static string GetRequestUrl()
		{
			HttpRequest httpRequest = HttpContext.Current.Request;
			NameValueCollection serverVars = httpRequest.ServerVariables;
			bool isUrlRewrited = (serverVars != null && serverVars[MUrlRewriterServerVar] != null);
			if (!isUrlRewrited)
			{
				return httpRequest.Url.ToString();

			}

			string host = httpRequest.Url.IsDefaultPort ?
				httpRequest.Url.Host : httpRequest.Url.Host + ":" + httpRequest.Url.Port.ToString();
			return "http://" + host + httpRequest.RawUrl;
		}

		private static string GetUrlReferrer()
		{
			HttpContext httpContext = HttpContext.Current;
			if (httpContext == null)
			{
				return null;
			}

			HttpRequest httpRequest = httpContext.Request;
			return httpRequest.UrlReferrer == null ?
				string.Empty :
				httpRequest.UrlReferrer.ToString();
		}
	}
}
