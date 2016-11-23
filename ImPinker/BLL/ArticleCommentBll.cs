using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImDal;
using ImModel;

namespace ImBLL
{
    public class ArticleCommentBll
    {
        private ArticleCommentDal dal=new ArticleCommentDal();
        public bool Add(ArticleComment model)
        {
            return dal.Add(model);
        }
    }
}
