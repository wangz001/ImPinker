using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImDal;
using ImModel;
using ImModel.ViewModel;

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
            var list= dal.GetListsByArticleId(articleId,rowNum,count);
            
            return list;
        }

        public List<ArticleCommentVm> GetCommentsWithToComments(string articleId, int rowNum, int count)
        {
            var returnList = new List<ArticleCommentVm>();
            var list = GetListsByArticleId(articleId, rowNum, count);
            var toCommentIds = list.Select(m => m.ToCommentId).Distinct();
            //关联评论
            var commentIds = toCommentIds as int[] ?? toCommentIds.ToArray();
            
                var list2 = dal.GetListsByIds(commentIds.ToList());
                
            foreach (var articleComment in list)
            {
                var model = new ArticleCommentVm()
                {
                    Id = articleComment.Id,
                    ArticleId = articleComment.ArticleId,
                    Content = articleComment.Content,
                    ToCommentId = articleComment.ToCommentId,
                    UserId = articleComment.UserId,
                    CreateTime = articleComment.CreateTime

                };
                if (articleComment.ToCommentId>0)
                {
                    model.ListToComment = new List<ArticleComment>(){list2.FirstOrDefault(m => m.Id == articleComment.ToCommentId)};
                }
                returnList.Add(model);
            }
            return returnList;
        }
    }
}
