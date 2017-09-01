using Common.Utils;
using ImDal;
using ImModel;
using ImModel.Enum;
using ImModel.ViewModel;
using System;
using System.Collections.Generic;

namespace ImBLL
{
    public class NotifyBll
    {
        readonly ArticleBll _articleBll = new ArticleBll();
        readonly WeiBoBll _weiboBll = new WeiBoBll();
        readonly NotifyDal _notifyDal = new NotifyDal();
        readonly UserBll _userBll = new UserBll();

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
        /// 根据id获取对象
        /// </summary>
        /// <param name="notifyId"></param>
        /// <returns></returns>
        public Notify GetById(int notifyId)
        {
            var model = _notifyDal.GetById(notifyId);
            return model;
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
        /// <param name="userid"></param>
        /// <param name="notifyTypeEnum"></param>
        /// <param name="isRead"></param>
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
                    vm.SenderHeadUrl = ImageUrlHelper.GetHeadImageUrl(sender.ImgUrl, 100);
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
        /// <summary>
        /// 分页获取所有通知
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<NotifyVm> GetAllNotifyList(int userid, int page, int pagesize)
        {
            var vmList = new List<NotifyVm>();
            List<Notify> notifylist = _notifyDal.GetAllNotifyList(userid, page, pagesize);
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
                    vm.SenderHeadUrl = ImageUrlHelper.GetHeadImageUrl(sender.ImgUrl, 100);
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
        /// <summary>
        /// 修改通知的状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateNotify(int notifyId,int receiverId,bool isRead)
        {
            var model=GetById(notifyId);
            if (model!=null&&model.Receiver == receiverId)
            {
                model.IsRead = isRead;
                model.UpdateTime = DateTime.Now;
                var flag = _notifyDal.UpdateNotify(model);
                return flag;
            }
            return false;
        }
    }
}
