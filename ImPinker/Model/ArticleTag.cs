using System;

namespace ImModel
{
	/// <summary>
	/// ArticleTag:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class ArticleTag
	{
		public ArticleTag()
		{}
		#region Model
		private int _id;
		private byte[] _tagname;
		private int? _userid;
		private bool _isdelete;
		private DateTime? _createtime;
		private DateTime? _updatetime;
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
		public byte[] TagName
		{
			set{ _tagname=value;}
			get{return _tagname;}
		}
		/// <summary>
		/// 
		/// </summary>
		public int? UserId
		{
			set{ _userid=value;}
			get{return _userid;}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsDelete
		{
			set{ _isdelete=value;}
			get{return _isdelete;}
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

