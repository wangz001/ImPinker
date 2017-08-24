using ImBLL;
using ImpinkerApi.Common;
using System.Net.Http;
using System.Web.Http;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;
using ImModel.Enum;

namespace ImpinkerApi.Controllers
{
    public class NotifyController : BaseApiController
    {
        readonly NotifyBll _notifyBll=new NotifyBll();
       
        /// <summary>
        /// 获取用户的新通知
        /// </summary>
        /// <returns></returns>
        [TokenCheck]
        [HttpGet]
        public HttpResponseMessage GetNewNotifyCount()
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var count = _notifyBll.GetNewNotifyCount(userinfo.Id);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = count>0 ? 1 : 0,
                Data = count,
                Description = "获取新通知"
            });
        }
        /// <summary>
        /// 获取用户新通知列表
        /// </summary>
        /// <returns></returns>
        [TokenCheck]
        [HttpGet]
        public HttpResponseMessage GetNewNotifyList(int isRead)
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var list = _notifyBll.GetNotifyList(userinfo.Id, NotifyTypeEnum.Remind, isRead>0);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = list.Count > 0 ? 1 : 0,
                Data = list,
                Description = "获取用户新通知列表"
            });
        }
        /// <summary>
        /// 标记为已读
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [TokenCheck]
        [HttpPost]
        public HttpResponseMessage UpdateNotifyState([FromBody]NotifyViewModel vm)
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var flag = _notifyBll.UpdateNotify(vm.NotifyId,(int)userinfo.Id, vm.IsRead>0);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = flag ? 1 : 0,
                Data = flag,
                Description = "标记为已读"
            });
        }
    }
}
