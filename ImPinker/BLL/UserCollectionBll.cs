using ImDal;
using ImModel;
using ImModel.Enum;
using System;
using System.Collections.Generic;

namespace ImBLL
{
    public class UserCollectionBll
    {
        readonly UserCollectionDal _dal = new UserCollectionDal();
        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool AddCollect(long entityId,int userid,EntityTypeEnum entityType)
        {
            var model = _dal.GetModel(entityId, userid, entityType);
            if (model!=null)
            {//如果是取消收藏，重新修改状态为收藏
                if (model.State == UserCollectionStateEnum.UnCollect)
                {
                    model.State = UserCollectionStateEnum.Collect;
                    model.UpdateTime = DateTime.Now;
                    bool flag = _dal.UpdateCollect(model);
                    return flag;
                }
            }
            else
            {
                var newModel = new UserCollection
                {
                    EntityId = entityId,
                    EntityType=entityType,
                    UserId = userid,
                    State = UserCollectionStateEnum.Collect,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                return _dal.AddCollect(newModel);
            }
            return false;
        }
        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool RemoveCollect(long articleId, int userid,EntityTypeEnum entityType)
        {
            var model = _dal.GetModel(articleId, userid, entityType);
            if (model!=null)
            {
                model.State = UserCollectionStateEnum.UnCollect;
                model.UpdateTime = DateTime.Now;
                bool flag = _dal.UpdateCollect(model);
                return flag;
            }
            return false;
        }

        /// <summary>
        /// 获取个人的收藏
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageNum"></param>
        /// <param name="pagecount"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<Article> GetMyListByPage(int userId,int  pageNum, int pagecount, out int totalCount)
        {
            var list = new List<Article>();//注，只返回article的部分字段
            var dt = _dal.GetMyCollectsByPage(userId,pageNum,pagecount,out totalCount);
            if (dt != null && dt.Rows.Count > 0)
            {
                list = new ArticleBll().DataTableToList(dt);
            }
            return list;
        }
    }
}
