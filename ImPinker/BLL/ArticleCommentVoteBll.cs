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

        public static bool AddVote(ImModel.ArticleCommentVote model)
        {
            return articleCommentVoteDal.AddVote(model);
        }
    }
}
