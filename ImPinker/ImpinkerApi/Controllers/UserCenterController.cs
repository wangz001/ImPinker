using System.Net.Http;
using System.Web.Mvc;
using ImBLL;
using ImpinkerApi.Common;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{

    public class UserCenterController : BaseApiController
    {
        private static readonly UserBll UserBll = new UserBll();

        /// <summary>
        /// 修改用户显示名称
        /// </summary>
        /// <param name="showname"></param>
        /// <returns></returns>
        [TokenCheck]
        [HttpGet]
        public HttpResponseMessage GetNewShowName(string showname)
        {
            var userid=TokenHelper.GetUserInfoByHeader(Request.Headers).Id;
            var user = UserBll.GetModelByCache(userid);
            if (!string.IsNullOrEmpty(showname))
            {
                user.ShowName = showname;
                var falg = UserBll.Update(user);
                if (falg)
                {
                    return GetJson(new JsonResultViewModel
                    {
                        IsSuccess = 1,
                        Description = "修改成功",
                        Data = showname
                    });
                }
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Description = "修改失败",
                Data = showname
            });
        }

    }
}
