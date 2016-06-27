using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImPinker.Models
{
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