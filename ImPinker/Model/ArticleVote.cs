using System;
namespace Model
{
	/// <summary>
	/// ArticleVote:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class ArticleVote
	{
		public ArticleVote()
		{}
		#region Model
		private long _id;
		private long _articleid;
		private int _userid;
		private bool _vote;
		private DateTime? _createtime;
		private DateTime? _updatetime;
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
		public long ArticleId
		{
			set{ _articleid=value;}
			get{return _articleid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int UserId
		{
			set{ _userid=value;}
			get{return _userid;}
		}
		/// <summary>
		/// 1:like   0: dislike
		/// </summary>
		public bool Vote
		{
			set{ _vote=value;}
			get{return _vote;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime? UpdateTime
		{
			set{ _updatetime=value;}
			get{return _updatetime;}
		}
		#endregion Model

	}
}

