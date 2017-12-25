using System;
using ImDal;
using ImModel;
using Maticsoft.Common;

namespace ImBLL
{
    public class UserTokenBll
    {
        readonly UserTokenDal _dal = new UserTokenDal();
        readonly UserBll _userBll = new UserBll();

        public bool Update(int userid, string token)
        {
            bool flag;
            var objModel = _dal.GetByUserId(userid);
            if (objModel != null)
            {
                objModel.Token = token;
                flag = _dal.Update(objModel);
            }
            else
            {
                var newmodel = new UserToken
                {
                    UserId = userid,
                    Token = token,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                flag = _dal.Add(newmodel);
            }
            return flag;
        }

        public UserToken Get(int userid)
        {
            string cacheKey = "UserToken-" + userid;
            object objModel = DataCache.GetCache(cacheKey);
            if (objModel == null)
            {
                objModel = _dal.GetByUserId(userid);
                if (objModel != null)
                {
                    int modelCache = ConfigHelper.GetConfigInt("ModelCache");
                    DataCache.SetCache(cacheKey, objModel, DateTime.Now.AddMinutes(modelCache), TimeSpan.Zero);
                }
            }
            return (UserToken)objModel;
        }
        /// <summary>
        /// 根据用户名获取token字符串
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string GetTokenStr(string username)
        {
            var token = string.Empty;
            var user = _userBll.GetModelByUserName(username);
            if (user == null) return token;
            var tokenModel = Get(user.Id);
            if (tokenModel != null)
            {
                token = tokenModel.Token;
            }
            return token;
        }


    }
}
