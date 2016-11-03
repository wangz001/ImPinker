using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Common.Utils
{
	public static class HtmlHelperExtensions
	{
		/// <summary>
		/// 递归获取include文件内容
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="serverPath"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static IHtmlString ServerSideRecursionInclude(this HtmlHelper helper, string serverPath)
		{

			try
			{
				string content = RecursionReadHtml(serverPath, Encoding.UTF8);
				return new HtmlString(content);

			}
			catch (Exception ex)
			{
				return new HtmlString("");
			}
		}

		/// <summary>
		/// 读取静态文件
		/// </summary>
		/// <param name="serverPath"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		private static string ReadLocalFile(string serverPath, Encoding encoding)
		{
			try
			{
				string path = HttpContext.Current.Server.MapPath(serverPath);
				using (var streamReader = new StreamReader(path, encoding))
				{
					var html = streamReader.ReadToEnd();
					return html;
				}
			}
			catch (Exception oe)
			{
				return string.Empty;
			}

		}
		/// <summary>
		/// 递归读取文件信息
		/// </summary>
		/// <param name="serverPath"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		private static string RecursionReadHtml(string serverPath, Encoding encoding)
		{
			string content = ReadLocalFile(serverPath, encoding);
			string pattern = @"<!--#include\s+?file=""(?<src>.+?)""\s*?-->";
			MatchCollection mc = Regex.Matches(content, pattern);
			if (mc.Count > 0)
			{
				for (int i = 0; i < mc.Count; i++)
				{
					string path = mc[i].Groups["src"].Value;
					content = content.Replace(mc[i].Value, RecursionReadHtml(path, encoding));
				}
			}
			return content;
		}

	}
}
