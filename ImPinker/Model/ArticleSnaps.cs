using System;
namespace Model
{
	/// <summary>
	/// ArticleSnaps:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class ArticleSnaps
	{
		public ArticleSnaps()
		{}
		#region Model
		private int _id;
		private long? _articleid;
		private string _text;
		/// <summary>
		/// 
		/// </summary>
		public int Id
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
		public string text
		{
			set{ _text=value;}
			get{return _text;}
		}
		#endregion Model

	}
}

