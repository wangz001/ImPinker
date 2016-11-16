using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Bita.Common;
using GetCarDataService.Tools;
using HtmlAgilityPack;

namespace GetCarDataService
{
	public class Common
	{
		public static HtmlDocument GetHtmlDocument(string uri, bool? utf8 = false)
		{
			var isUseProxy = ConfigHelper.GetConfigString("IsUseProxy");
			if ("true".Equals(isUseProxy))
			{
				return HttpRequestProxy.GetHtmlDocument(uri);
			}
			else
			{
				var encoding = Encoding.GetEncoding("gb2312");
				if (utf8 != null && utf8 == true)
				{
					encoding = Encoding.GetEncoding("utf-8");
				}
				var htmlWeb = new HtmlWeb { OverrideEncoding = encoding };
				HtmlDocument htmlDocument = htmlWeb.Load(uri);
				return htmlDocument;
			}
		}

		public static HtmlDocument HtmlRequest(string uri)
		{
			HtmlDocument htmlDocument = null;
			Exception err = null;
			int count = 0;
			while (count < 10)//此处用一个循环，如果网络连接超时，则重新执行
			{
				count++;
				try
				{
					htmlDocument = GetHtmlDocument(uri);
					err = null;
					break;
				}
				catch (Exception ex)
				{
					err = ex;
					Thread.Sleep(1000);
				}
			}
			if (err != null)
				WriteErrorLog("URL ERROR:" + uri + "\r\n" + err.ToString());
			return htmlDocument;
		}

		public static string GetResponseContent(string uri)
		{
			var request = (HttpWebRequest)WebRequest.Create(uri);
			var response = (HttpWebResponse)request.GetResponse();
			Stream stream = response.GetResponseStream();
			if (response.ContentEncoding.ToLower().Contains("gzip"))
			{
				if (stream != null) stream = new GZipStream(stream, CompressionMode.Decompress);
			}
			Encoding encoding = Encoding.GetEncoding("gb2312");
			var streamReader = new StreamReader(stream, encoding);
			string strResult = streamReader.ReadToEnd();
			return strResult;
		}

		public static string GetSpell(string name)
		{
			try
			{
				string name2 = name.Substring(0, 1);
				return GetStringSpell.GetChineseSpell(name2).FirstOrDefault().ToString(CultureInfo.InvariantCulture).ToUpper();
			}
			catch (Exception exception)
			{
				WriteErrorLog("spell--->" + "name:" + name + ";Exception:" + "获取首字母出错" + exception.ToString());
				Console.WriteLine(exception);
			}
			return null;
		}
		/// <summary>
		/// 记录错误日志
		/// </summary>
		/// <param name="logContent"></param>
		public static void WriteErrorLog(string logContent)
		{
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ErrorLog");
			WriteLog(path, logContent);
		}
		/// <summary>
		/// 记录程序执行信息
		/// </summary>
		/// <param name="logContent"></param>
		public static void WriteInfoLog(string logContent)
		{
			var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InfoLog");
			WriteLog(path, logContent);
		}

		private static void WriteLog(string path, string logContent)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			var logName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
			var txtLogUrl = Path.Combine(path, logName);
			using (var fileStream = File.Open(txtLogUrl, FileMode.Append, FileAccess.Write))
			using (var streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.WriteLine(logContent);
				streamWriter.Close();
				fileStream.Close();
			}
		}

		public static string GetDateTimeStr()
		{
			return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		}
	}
}
