using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImBLL;
using ImpinkerApi.Common;
using System.Net.Http;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;
using ImModel.Enum;

namespace ImpinkerApi.Controllers
{
    public class NotifyController : BaseApiController
    {
        NotifyBll _notifyBll=new NotifyBll();
        //
        // GET: /Notify/

        public ActionResult Index()
        {
            return null;
        }

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
        public HttpResponseMessage GetNewNotifyList()
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var list = _notifyBll.GetNotifyList(userinfo.Id,NotifyTypeEnum.Remind,false);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = list.Count > 0 ? 1 : 0,
                Data = list,
                Description = "获取用户新通知列表"
            });
        }

    }
}
