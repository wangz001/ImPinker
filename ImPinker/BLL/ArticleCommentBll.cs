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
            int totalcount;
            var ds= dal.GetListsByArticleId(articleId,rowNum,count,out totalcount);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                return dal.DtToList(ds.Tables[0]);
            }
            return null;
        }

        /// <summary>
        /// 分页获取文章评论，带有引用的评论
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="rowNum"></param>
        /// <param name="count"></param>
        /// <param name="totalCount">总记录数，分页用</param>
        /// <returns></returns>
        public List<ArticleCommentVm> GetCommentsWithToComments(string articleId, int rowNum, int count,out int totalCount)
        {
            var returnList = new List<ArticleCommentVm>();
            var ds = dal.GetListsByArticleId(articleId, rowNum, count,out totalCount);
            var list1 = new List<ArticleComment>();
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                list1 = dal.DtToList(ds.Tables[0]);
            }
            var list2 = new List<ArticleComment>();
            if (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            {
                list2 = dal.DtToList(ds.Tables[1]);
            }

            foreach (var articleComment in list1)
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
