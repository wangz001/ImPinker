using System.Web;

namespace Common.Utils
{
	public class URLHelper
	{
		/// <summary>
		/// 将虚拟（相对）路径转换为应用程序绝对路径。
		/// </summary>
		/// <param name="relativePath">内容的虚拟路径</param>
		/// <returns>应用程序绝对路径</returns>
		public static string ToAbsolute(string relativePath)
		{
			//判空
			if (string.IsNullOrEmpty(relativePath))
			{
				return relativePath;
			}

			//判断是否是相对路径
			bool isRelative = relativePath[0] == '~';
			if (!isRelative)
			{
				return relativePath;
			}

			string query;
			relativePath = StripQuery(relativePath, out query);

			return VirtualPathUtility.ToAbsolute(relativePath) + query;
		}

		private static string StripQuery(string path, out string query)
		{
			int queryIndex = path.IndexOf('?');
			if (queryIndex >= 0)
			{
				query = path.Substring(queryIndex);
				return path.Substring(0, queryIndex);
			}
			else
			{
				query = null;
				return path;
			}
		}
	}
}
