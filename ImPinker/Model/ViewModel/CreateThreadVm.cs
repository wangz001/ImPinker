using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel.ViewModel
{
    /// <summary>
    /// 发布帖子实体类
    /// </summary>
    public class CreateThreadVm
    {
        public long ArticleId { get; set; }
        [Required]
        [Display(Name = "帖子名称")]
        public string ArticleName { get; set; }
        /// <summary>
        /// 封面图
        /// </summary>
        [Required]
        [Display(Name = "封面图")]
        public string Coverimage { get; set; }
        public int Userid { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 帖子内容
        /// </summary>
        [Required]
        [Display(Name = "帖子正文")]
        [DataType(DataType.Html)]
        public string Content { get; set; }
        /// <summary>
        ///  状态:      0:删除    1:正常可显示   2: 待审核   3:审核不通过
        /// </summary>
        public ArticleStateEnum State { get; set; }
        public DateTime Createtime { get; set; }
        public DateTime Updatetime { get; set; }
    }
}
