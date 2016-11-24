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
        /// <summary>
        /// 分页获取文章评论
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="rowNum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<ArticleComment> GetListsByArticleId(string articleId,int rowNum,int count)
        {
            return dal.GetListsByArticleId(articleId,rowNum,count);
        }
    }
}
