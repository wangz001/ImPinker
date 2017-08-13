using Common.Utils;
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
        UserBll _userBll = new UserBll();

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
            return count;
        }
        /// <summary>
        /// 获取用户通知列表
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public List<NotifyVm> GetNotifyList(int userid, NotifyTypeEnum notifyTypeEnum,bool isRead)
        {
            var vmList = new List<NotifyVm>();
            List<Notify> notifylist = _notifyDal.GetNotifyList(userid,notifyTypeEnum, isRead);
            if (notifylist != null && notifylist.Count > 0)
            {
                foreach (var notify in notifylist)
                {
                    var vm = new NotifyVm
                    {
                        Id = notify.Id,
                        NotifyType = notify.NotifyType,
                        ContentStr = notify.ContentStr,
                        Target = notify.Target,
                        TargetType = notify.TargetType,
                        Action = notify.Action,
                        Sender = notify.Sender,
                        Receiver = notify.Receiver,
                        IsRead = notify.IsRead,
                        CreateTime = notify.CreateTime,
                        UpdateTime = notify.UpdateTime
                    };
                    var sender = _userBll.GetModelByCache(vm.Sender);
                    vm.SenderName = string.IsNullOrEmpty(sender.ShowName) ? sender.UserName : sender.ShowName;
                    vm.ActionName = EnumHelper.GetDescriptionFromEnumValue(typeof(ActionEnum), vm.Action);
                    vm.TargetTypeName = EnumHelper.GetDescriptionFromEnumValue(typeof(TargetTypeEnum), vm.TargetType);
                    var targetName = "";
                    switch (vm.TargetType)
                    {
                        case TargetTypeEnum.Article:
                            var article = _articleBll.GetModelByCache(vm.Target);
                            targetName = article.ArticleName;
                            break;
                        case TargetTypeEnum.Weibo:
                            var weibo = _weiboBll.GetById(vm.Target);
                            targetName = weibo.Description;
                            break;
                    }
                    vm.TargetName = targetName;
                    vmList.Add(vm);
                }
            }
            return vmList;
        }
    }
}
