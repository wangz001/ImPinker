using ImDal;
using ImModel;
using ImModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImBLL
{
    public class ArticleCollectionBll
    {
        ArticleCollectionDal dal = new ArticleCollectionDal();
        public bool AddCollect(long articleId,int userid)
        {
            var isExists = dal.IsExist(articleId,userid);
            if (isExists)
            {
                //update
                return true;
            }
            else
            {
                var model = new ArticleCollection
                {
                    ArticleId = articleId,
                    UserId = userid,
                    State = ArticleCollectionStateEnum.Collect,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                return dal.AddCollect(model);
            }
        }
    }
}
