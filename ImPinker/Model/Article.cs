using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Model{
	/// <summary>
	/// Article:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public partial class Article
	{
		public Article()
		{}
		#region Model
		private long _id;
		private string _articlename;
		private string _url;
		private string _coverimage;
		private int _userid;
		private string _description;
		private int _state;
		private DateTime _createtime;
		private DateTime _updatetime;
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
        [Display(Name = "标题")]
		public string ArticleName
		{
			set{ _articlename=value;}
			get{return _articlename;}
		}
		/// <summary>
		/// 
		/// </summary>
        [Display(Name = "链接")]
        public string Url
		{
			set{ _url=value;}
			get{return _url;}
		}
		/// <summary>
		/// 
		/// </summary>
		public string CoverImage
		{
			set{ _coverimage=value;}
			get{return _coverimage;}
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
		/// 
		/// </summary>
		public string Description
		{
			set{ _description=value;}
			get{return _description;}
		}
		/// <summary>
		///  状态:    -1:删除   0:默认    1:正常可显示   2: 待审核   3:审核不通过
		/// </summary>
		public int State
		{
			set{ _state=value;}
			get{return _state;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime UpdateTime
		{
			set{ _updatetime=value;}
			get{return _updatetime;}
		}
		#endregion Model

	}
}

