using System;

namespace AhModel
{
	/// <summary>
	/// AHStylePropertyValue:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class AHStylePropertyValue:EntityBase
	{
		
		/// <summary>
		/// 
		/// </summary>
		public int PropertyID
		{
		    set;
		    get;
		}
		/// <summary>
		/// 
		/// </summary>
		public int StyleID
		{
		    set;
		    get;
		}
		/// <summary>
		/// 
		/// </summary>
		public string Value
		{
		    set;
		    get;
		}
		

	}
}

