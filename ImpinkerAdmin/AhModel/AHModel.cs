using System;

namespace AhModel
{
	/// <summary>
	/// AHModel:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class AHModel:EntityBase
	{
	    public int MasterBrandID { get; set; }

	    public int ManufacturerID { get; set; }

	    public string Initial { set; get; }

        public string ModelName { set; get; }

        public int IsRemoved { get; set; }
	}
}

