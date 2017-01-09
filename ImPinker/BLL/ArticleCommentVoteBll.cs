using ImDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImBLL
{
    public class ArticleCommentVoteBll
    {
        private static ArticleCommentVoteDal articleCommentVoteDal = new ArticleCommentVoteDal();

        /// <summary>
        /// 添加赞。articleCommentId，UserId 唯一性约束
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddVote(ImModel.ArticleCommentVote model)
        {
            if (IsExist(model.ArticleCommentId, model.UserId))
            {
                return false;
            }
            return articleCommentVoteDal.AddVote(model);
        }
        /// <summary>
        /// 判断对象是否存在
        /// </summary>
        /// <param name="articleCommentId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsExist(long articleCommentId, int userId)
        {
            var flag = false;
            if (articleCommentId > 0 && userId > 0)
            {
                flag = articleCommentVoteDal.IsExist(articleCommentId, userId);
            }
            return flag;
        }
    }
}
