using ImDal;
using ImModel;
using ImModel.Enum;
using System;

namespace ImBLL
{
    public class WeiBoVoteBll
    {
        readonly WeiBoVoteDal _weiboVoteDal = new WeiBoVoteDal();
        readonly NotifyBll _notifyBll = new NotifyBll();
        /// <summary>
        /// 添加赞记录
        /// </summary>
        /// <param name="weiboId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool AddWeiBoVote(long weiboId, int userId)
        {
            bool flag;
            var model = new WeiBoVote
            {
                WeiBoId=weiboId,
                UserId=userId,
                Vote=true,
                CreateTime=DateTime.Now,
                UpdateTime=DateTime.Now
            };
            bool isExists = IsExists(weiboId,userId);
            if (isExists)
            {
                flag = _weiboVoteDal.UpdtateStete(model);
            }
            else
            {
                flag = _weiboVoteDal.Add(model);
                if (!flag)
                {
                    return false;
                }
                //添加通知
                flag = _notifyBll.NewNotify(NotifyTypeEnum.Remind, (int)weiboId, TargetTypeEnum.Weibo, ActionEnum.Vote, userId, "赞了微博");
            }
            return flag;
        }

        public bool CancelWeiboVote(long weiboId, int userId)
        {
            var flag = false;
            var model = new WeiBoVote
            {
                WeiBoId = weiboId,
                UserId = userId,
                Vote = false,
                UpdateTime = DateTime.Now
            };
            bool isExists = IsExists(weiboId, userId);
            if (isExists)
            {
                flag = _weiboVoteDal.UpdtateStete(model);
            }
            return flag;
        }
        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="weiboId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool IsExists(long weiboId, int userId)
        {
            bool flag = _weiboVoteDal.IsExists(weiboId, userId);
            return flag;
        }
    }
}
