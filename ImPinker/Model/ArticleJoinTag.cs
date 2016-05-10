using System;
namespace Model
{
	/// <summary>
	/// ArticleJoinTag:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class ArticleJoinTag
	{
		public ArticleJoinTag()
		{}
		#region Model
		private long _id;
		private long? _articleid;
		private int? _tagid;
		private DateTime? _createtime;
		/// <summary>
		/// 
		/// </summary>
		public long Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 
		/// </summary>
		public long? ArticleId
		{
			set{ _articleid=value;}
			get{return _articleid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? TagId
		{
			set{ _tagid=value;}
			get{return _tagid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
		#endregion Model

	}
}

