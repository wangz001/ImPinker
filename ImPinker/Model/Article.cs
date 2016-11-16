using System;
using System.ComponentModel.DataAnnotations;

namespace ImModel{

	public enum ArticleStateEnum
	{
		[Display(Name = "已删除")]
		Deleted=0,
		[Display(Name = "正常")]
		Normal=1,
		[Display(Name = "待审核")]
		BeCheck=2,
		[Display(Name = "审核未通过")]
		CheckFalse=3

	}
	/// <summary>
	/// Article:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public class Article
	{
		#region Model
		private long _id;
		private string _articlename;
		private string _url;
		private string _coverimage;
		private int _userid;
        private string _keywords;
        private string _company;
		private string _description;
	    private string _content;
		private int _state;
		private DateTime _createtime;
        private DateTime _updatetime;
        private DateTime _publishtime;
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

		public string KeyWords
		{
			set { _keywords = value; }
			get { return _keywords; }
		}

    /// <summary>
    /// 所属网站。易车、之家、e族等
    /// </summary>
        public string Company
        {
            set { _company = value; }
            get { return _company; }
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
		///  状态:      0:删除    1:正常可显示   2: 待审核   3:审核不通过
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
        public DateTime PublishTime
        {
            set { _publishtime = value; }
            get { return _publishtime; }
        }
		#endregion Model

	}
}

