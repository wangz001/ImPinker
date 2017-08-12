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
    public class WeiBoVoteBll
    {
        WeiBoVoteDal _weiboVoteDal = new WeiBoVoteDal();
        NotifyBll _notifyBll = new NotifyBll();
        /// <summary>
        /// 添加赞记录
        /// </summary>
        /// <param name="weiboId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool AddWeiBoVote(long weiboId, int userId)
        {
            var flag = false;
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
                //添加通知
                var notifyflag=_notifyBll.NewNotify(NotifyTypeEnum.Remind, (int)weiboId, TargetTypeEnum.Weibo, ActionEnum.Vote, userId, "赞了微博");
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
