using ImDal;
using ImModel;
using ImModel.Enum;
using ImModel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImBLL
{
    public class NotifyBll
    {
        ArticleBll _articleBll = new ArticleBll();
        WeiBoBll _weiboBll = new WeiBoBll();
        NotifyDal _notifyDal = new NotifyDal();

        public bool NewNotify(NotifyTypeEnum notigyType, int targetId, TargetTypeEnum targetType, ActionEnum action,int senderId,string content)
        {
            var receiverId = 0;
            switch (targetType){
                case TargetTypeEnum.Article:
                    var article = _articleBll.GetModelByCache(targetId);
                    receiverId = article.UserId;
                    break;

                case TargetTypeEnum.Weibo:
                    var weibo = _weiboBll.GetById(targetId);
                    receiverId = weibo.UserId;
                    break;
            }
            var notify = new Notify
            {
                NotifyType=notigyType,
                ContentStr = content,
                Target = targetId,
                TargetType = targetType,
                Action = action,
                Sender = senderId,
                Receiver = receiverId,
                IsRead = false,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            long id = _notifyDal.Add(notify);
            return id>0;
        }

        /// <summary>
        /// 获取用户的提醒总数
        /// </summary>
        /// <returns></returns>
        public int GetNewNotifyCount(int userid)
        {
            int count = _notifyDal.GetNotifyCount(userid, false);

            return 0;
        }
        /// <summary>
        /// 获取用户通知列表
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public List<Notify> GetNotifyList(int userid, NotifyTypeEnum notifyTypeEnum,bool isRead)
        {
            List<Notify> notifylist = _notifyDal.GetNotifyList(userid,notifyTypeEnum, isRead);


            return notifylist;
        }
    }
}
