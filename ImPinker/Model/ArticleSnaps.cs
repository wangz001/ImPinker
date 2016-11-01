using System;
namespace Model
{
	/// <summary>
	/// 文章快照
	/// </summary>
	[Serializable]
	public class ArticleSnaps
	{
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

