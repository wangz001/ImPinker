using System;

namespace Common.Utils
{
	public class DbObjectConverter
	{
		/// <summary>
		/// 把数据库值转化成字符串
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ToString(object obj)
		{
			return string.Format("{0}", obj).Trim();
		}

		/// <summary>
		/// 把字符串转化成数据库值
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToDbString(string str)
		{
			str = str.Trim();
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			return str;
		}

		/// <summary>
		/// 把数据库值转化成相应类型的对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T ConvertTo<T>(object obj)
		{
			return DBNull.Value.Equals(obj) ? default(T) : (T)obj;
		}

	}
}
