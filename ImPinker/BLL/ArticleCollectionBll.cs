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
        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool AddCollect(long articleId,int userid)
        {
            var isExists = dal.IsExist(articleId,userid);
            if (isExists)
            {
                return false;
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
        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool RemoveCollect(long articleId, int userid)
        {
            var model = dal.GetModel(articleId, userid);
            if (model!=null)
            {
                model.State = ArticleCollectionStateEnum.UnCollect;
                model.UpdateTime = DateTime.Now;
                bool flag = dal.UpdateCollect(model);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取个人的收藏
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<Article> GetMyListByPage(int userId,int  pageNum, int pagecount, out int totalCount)
        {
            var list = new List<Article>();//注，只返回article的部分字段
            var dt = dal.GetMyCollectsByPage(userId,pageNum,pagecount,out totalCount);
            if (dt != null && dt.Rows.Count > 0)
            {
                list = new ArticleBll().DataTableToList(dt);
            }
            return list;
        }
    }
}
