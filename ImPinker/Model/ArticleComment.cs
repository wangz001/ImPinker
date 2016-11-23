using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImModel
{
    public class ArticleComment
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }

        public int UserId { get; set; }

        public string Content { get; set; }

        public int ToUserId { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
