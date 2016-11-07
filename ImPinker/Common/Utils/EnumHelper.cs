﻿using System;
using System.Collections.Generic;

namespace Common.Utils
{
	public static class EnumHelper
	{
		public static bool TryParse<TEnum>(string value, out TEnum result)
		   where TEnum : struct
		{
			return TryParse(value, false, out result);
		}

		public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result)
			where TEnum : struct
		{
			return TryParse(value, out result,
				ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
		}

		private static bool TryParse<TEnum>(string value, out TEnum result,
			IEqualityComparer<string> stringComparer)
			where TEnum : struct
		{
			var type = typeof(TEnum);
			if (!type.IsEnum)
			{
				throw new ArgumentException(
					string.Format(
					"{0}不是枚举类型",
					type));
			}

			result = default(TEnum);
			foreach (string name in Enum.GetNames(type))
			{
				if (stringComparer.Equals(name, value))
				{
					result = (TEnum)Enum.Parse(type, name);
					return true;
				}
			}

			int valueInt;
			if (!int.TryParse(value, out valueInt))
			{
				return false;
			}

			if (Enum.IsDefined(type, valueInt))
			{
				result = (TEnum)Enum.Parse(type, value);
				return true;
			}

			return false;
		}
	}
}