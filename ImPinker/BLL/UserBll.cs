using System;
using System.Collections.Generic;
using System.Data;
using ImDal;
using ImModel;
using Maticsoft.Common;
using System.Configuration;
using Common.AlyOssUtil;
using Common.Utils;

namespace ImBLL
{
    /// <summary>
    /// User
    /// </summary>
    public class UserBll
    {
        private readonly User _dal = new User();
        #region  BasicMethod

        /// <summary>
        /// 得到最大ID
        /// </summary>
        public int GetMaxId()
        {
            return _dal.GetMaxId();
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            return _dal.Exists(id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Users model)
        {
            if (string.IsNullOrEmpty(model.AspNetId))
            {
                //api 手机验证码登录
                model.AspNetId = model.PhoneNum;
            }
            model.CreateTime = DateTime.Now;
            model.UpdateTime = DateTime.Now;
            return _dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Users model)
        {
            return _dal.Update(model);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Users GetModelByCache(int id)
        {
            string cacheKey = "UserModel-" + id;
            object objModel = DataCache.GetCache(cacheKey);
            if (objModel == null)
            {
                objModel = _dal.GetModel(id);
                if (objModel != null)
                {
                    int modelCache = ConfigHelper.GetConfigInt("ModelCache");
                    DataCache.SetCache(cacheKey, objModel, DateTime.Now.AddMinutes(modelCache), TimeSpan.Zero);
                }

            }
            return (Users)objModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspnetId">系统生成的md5  id</param>
        /// <returns></returns>
        public Users GetModelByAspNetId(string aspnetId)
        {
            string cacheKey = "UserModel-" + aspnetId;
            object objModel = DataCache.GetCache(cacheKey);
            if (objModel == null)
            {
                objModel = _dal.GetModelByAspNetId(aspnetId);
                if (objModel != null)
                {
                    int modelCache = ConfigHelper.GetConfigInt("ModelCache");
                    DataCache.SetCache(cacheKey, objModel, DateTime.Now.AddMinutes(modelCache), TimeSpan.Zero);
                }
            }
            return (Users)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return _dal.GetList(strWhere);
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int top, string strWhere, string filedOrder)
        {
            return _dal.GetList(top, strWhere, filedOrder);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Users> GetModelList(string strWhere)
        {
            DataSet ds = _dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Users> DataTableToList(DataTable dt)
        {
            var modelList = new List<Users>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                for (int n = 0; n < rowsCount; n++)
                {
                    var model = _dal.DataRowToModel(dt.Rows[n]);
                    if (model != null)
                    {
                        modelList.Add(model);
                    }
                }
            }
            return modelList;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetAllList()
        {
            return GetList("");
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            return _dal.GetRecordCount(strWhere);
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            return _dal.GetListByPage(strWhere, orderby, startIndex, endIndex);
        }


        #endregion  BasicMethod

        /// <summary>
        /// 修改用户头像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newimgUrl"></param>
        /// <returns></returns>
        public bool UpdateHeadImg(int userId, string newimgUrl)
        {
            var user = _dal.GetModel(userId);
            user.ImgUrl = newimgUrl;
            user.UpdateTime = DateTime.Now;
            return _dal.Update(user);
        }
        /// <summary>
        /// 判断该电话号码是否被注册
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        public Users GetModelByPhoneNum(string phoneNum)
        {
            if (string.IsNullOrEmpty(phoneNum))
            {
                return null;
            }
            var user = _dal.GetModelByPhoneNum(phoneNum);
            return user;
        }
        /// <summary>
        /// 根据用户名获取用户（登录用）
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Users GetModelByUserName(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            var user = _dal.GetModelByUserName(username);
            return user;
        }

        /// <summary>
        /// 上传用户头像到oss，并修改数据库
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="localPath"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UpdateHeadImage(string bucketName, string localPath, int userId)
        {
            //headimg/limit/{0}/{1}_{2}.jpg
            var headImageLimit = ConfigurationManager.AppSettings["HeadImageLimit"];
            //保存到数据库中的路径和oss的路径
            var limitimgUrl = string.Format(headImageLimit, DateTime.Now.ToString("yyyyMMdd"), userId, DateTime.Now.ToString("yyyyMMddHHmmss"));
            //上传到oss
            var ossSucess = ObjectOperate.UploadImage(bucketName, localPath, limitimgUrl,500);
            if (ossSucess)
            {
                //缩放、保存为3种格式
                int[] imgSize = { 180, 100, 40 };
                //本地路径
                var limitPath = AppDomain.CurrentDomain.BaseDirectory + "ImageUpload\\headimage\\" + limitimgUrl;
                foreach (var size in imgSize)
                {
                    var saveimgUrl = limitimgUrl.Replace("headimg/limit", "headimg/" + size);
                    var tempimgPath = limitPath.Replace("headimg/limit", "headimg/" + size);
                    ImageUtils.GetReduceImgFromCenter(size, size, localPath, tempimgPath, 90);
                    var flag = ObjectOperate.UploadImage(bucketName, tempimgPath, saveimgUrl,30);
                    if (!flag)
                    {
                        return false;
                    }
                }
                //更新数据库
                var issuccess = UpdateHeadImg(userId, limitimgUrl);
                return issuccess;
            }
            return false;
        }

    }
}

