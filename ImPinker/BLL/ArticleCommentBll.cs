using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImDal;
using ImModel;
using ImModel.ViewModel;
using System.Data;

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
        public List<ArticleComment> GetListsByArticleId(long articleId,int rowNum,int count)
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
        public List<ArticleCommentVm> GetCommentsWithToComments(long articleId, int rowNum, int count,out int totalCount)
        {
            var returnList = new List<ArticleCommentVm>();
            var ds = dal.GetListsByArticleId(articleId, rowNum, count,out totalCount);
            var listToComment = new List<ArticleComment>();
            if (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            {
                listToComment = dal.DtToList(ds.Tables[1]);
            }
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = new ArticleCommentVm();

                    if (row["Id"] != null && row["Id"].ToString() != "")
                    {
                        model.Id = long.Parse(row["Id"].ToString());
                    }
                    if (row["ArticleId"] != null)
                    {
                        model.ArticleId = long.Parse(row["ArticleId"].ToString());
                    }
                    if (row["UserId"] != null)
                    {
                        model.UserId = int.Parse(row["UserId"].ToString());
                    }
                    if (row["Content"] != null)
                    {
                        model.Content = row["Content"].ToString();
                    }
                    if (row["ToCommentId"] != null && row["ToCommentId"].ToString() != "")
                    {
                        model.ToCommentId = int.Parse(row["ToCommentId"].ToString());
                    }
                    if (row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                    {
                        model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                    }
                    if (row["CommentVoteCount"] != null && row["CommentVoteCount"].ToString() != "")
                    {
                        model.ArticleCommentVoteCount = int.Parse(row["CommentVoteCount"].ToString());
                    }
                    model.ListToComment = new List<ArticleComment>() { listToComment.FirstOrDefault(m => m.Id == model.ToCommentId) };
                    returnList.Add(model);
                }
            }
            
            return returnList;
        }
        
    }
}
