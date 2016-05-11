using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImPinker.Models
{
	public class ArticleViewModel
	{
		[Display(Name = "标题")]
		public string ArticleName { get; set; }

		[Display(Name = "链接")]
		public string ArticleUrl { get; set; }

		[Display(Name = "推荐理由")]
		public string Description { get; set; }

		[Display(Name = "标签")]
		public string KeyWords { get; set; }

		public string LikeNum { get; set; }

		public string DisLikeNum { get; set; }

		public string PublisherName { get; set; }



	}

	public class CreateArticleViewModel
	{
		[Required]
		[Display(Name = "标题")]
		public string ArticleName { get; set; }

		[Required]
		[Display(Name = "链接")]
		public string ArticleUrl { get; set; }

		[Display(Name = "推荐理由")]
		public string Description { get; set; }

		[Display(Name = "标签")]
		public string KeyWords { get; set; }
	}
}