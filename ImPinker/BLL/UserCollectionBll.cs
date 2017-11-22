using System.Data;
using Common.DateTimeUtil;
using ImDal;
using ImModel;
using ImModel.Enum;
using System;
using System.Collections.Generic;
using ImModel.ViewModel;

namespace ImBLL
{
    public class UserCollectionBll
    {
        readonly UserCollectionDal _dal = new UserCollectionDal();
        private readonly UserBll _userBll = new UserBll();
        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool AddCollect(long entityId,int userid,EntityTypeEnum entityType)
        {
            var model = _dal.GetModel(entityId, userid, entityType);
            if (model!=null)
            {//如果是取消收藏，重新修改状态为收藏
                if (model.State == UserCollectionStateEnum.UnCollect)
                {
                    model.State = UserCollectionStateEnum.Collect;
                    model.UpdateTime = DateTime.Now;
                    bool flag = _dal.UpdateCollect(model);
                    return flag;
                }
            }
            else
            {
                var newModel = new UserCollection
                {
                    EntityId = entityId,
                    EntityType=entityType,
                    UserId = userid,
                    State = UserCollectionStateEnum.Collect,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                return _dal.AddCollect(newModel);
            }
            return false;
        }
        /// <summary>
        /// 取消收藏
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool RemoveCollect(long articleId, int userid,EntityTypeEnum entityType)
        {
            var model = _dal.GetModel(articleId, userid, entityType);
            if (model!=null)
            {
                model.State = UserCollectionStateEnum.UnCollect;
                model.UpdateTime = DateTime.Now;
                bool flag = _dal.UpdateCollect(model);
                return flag;
            }
            return false;
        }

        /// <summary>
        /// 获取个人的收藏
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageNum"></param>
        /// <param name="pagecount"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<ArticleWeiboVm> GetMyListByPage(int userId,int  pageNum, int pagecount)
        {
            var resultList = new List<ArticleWeiboVm>();//注，只返回article的部分字段
            var ds = _dal.GetMyCollectsByPage(userId, pageNum, pagecount);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var vm = new ArticleWeiboVm
                    {
                        EntityId = Int32.Parse(row["EntityId"].ToString()),
                        EntityType = Int32.Parse(row["EntityType"].ToString()),
                        Userid = Int32.Parse(row["UserId"].ToString()),
                        CreateTime = DateTime.Parse(row["CreateTime"].ToString())
                    };
                    #region 组装数据
                    if (vm.EntityType == 1)
                    {
                        var articleVm = new ArticleViewModel();
                        if (row.Table.Columns.Contains("AId") && row["AId"] != null && row["AId"].ToString() != "")
                        {
                            articleVm.Id = row["AId"].ToString();
                        }
                        if (row.Table.Columns.Contains("ArticleName") && row["ArticleName"] != null)
                        {
                            articleVm.ArticleName = row["ArticleName"].ToString();
                        }
                        if (row.Table.Columns.Contains("ADescription") && row["ADescription"] != null)
                        {
                            articleVm.Description = row["ADescription"].ToString();
                        }
                        if (row.Table.Columns.Contains("AUrl") && row["AUrl"] != null)
                        {
                            articleVm.Url = row["AUrl"].ToString();
                        }
                        if (row.Table.Columns.Contains("ACoverImage") && row["ACoverImage"] != null)
                        {
                            articleVm.CoverImage = row["ACoverImage"].ToString();
                            articleVm.CoverImage = ImageUrlHelper.GetArticleImage(articleVm.CoverImage, 360);
                        }
                        if (row.Table.Columns.Contains("AKeyWords") && row["AKeyWords"] != null)
                        {
                            articleVm.KeyWords = row["AKeyWords"].ToString();
                        }
                        if (row.Table.Columns.Contains("ACreateTime") && row["ACreateTime"] != null && row["ACreateTime"].ToString() != "")
                        {
                            articleVm.CreateTime = DateTime.Parse(row["ACreateTime"].ToString());
                        }
                        if (row.Table.Columns.Contains("AUpdateTime") && row["AUpdateTime"] != null && row["AUpdateTime"].ToString() != "")
                        {
                            articleVm.UpdateTime = DateTime.Parse(row["AUpdateTime"].ToString());
                        }
                        if (row.Table.Columns.Contains("ACompany") && row["ACompany"] != null && row["ACompany"].ToString() != "")
                        {
                            articleVm.Company = row["ACompany"].ToString();
                        }
                        if (row.Table.Columns.Contains("AUserId") && row["AUserId"] != null && row["AUserId"].ToString() != "")
                        {
                            articleVm.Userid = row["AUserId"].ToString();
                            var useritem = _userBll.GetModelByCache(int.Parse(articleVm.Userid));
                            articleVm.UserName = string.IsNullOrEmpty(useritem.ShowName) ? useritem.UserName : useritem.ShowName;
                        }
                        vm.ArticleVm = articleVm;
                    }
                    else
                    {
                        var weiboVm = new WeiboVm();
                        if (row.Table.Columns.Contains("WId") && row["WId"] != null && row["WId"].ToString() != "")
                        {
                            weiboVm.Id = long.Parse(row["WId"].ToString());
                        }
                        if (row.Table.Columns.Contains("WUserid") && row["WUserid"] != null)
                        {
                            weiboVm.UserId = Int32.Parse(row["WUserid"].ToString());
                            var userinfo = _userBll.GetModelByCache(weiboVm.UserId);
                            weiboVm.UserName = !string.IsNullOrEmpty(userinfo.ShowName) ? userinfo.ShowName : userinfo.UserName;
                            weiboVm.UserHeadImage = ImageUrlHelper.GetHeadImageUrl(userinfo.ImgUrl, 100);
                        }
                        if (row.Table.Columns.Contains("WDescription") && row["WDescription"] != null)
                        {
                            weiboVm.Description = row["WDescription"].ToString();
                        }
                        if (row.Table.Columns.Contains("WContentValue") && row["WContentValue"] != null)
                        {
                            weiboVm.ContentValue = row["WContentValue"].ToString();
                            weiboVm.ContentValue = ImageUrlHelper.GetWeiboFullImageUrl(weiboVm.ContentValue, 240);
                        }
                        if (row.Table.Columns.Contains("WContentType") && row["WContentType"] != null)
                        {
                            weiboVm.ContentType = (WeiBoContentTypeEnum)Int32.Parse(row["WContentType"].ToString());
                        }
                        if (row.Table.Columns.Contains("WLocationText") && row["WLocationText"] != null)
                        {
                            weiboVm.LocationText = row["WLocationText"].ToString();
                        }
                        if (row.Table.Columns.Contains("WLongitude") && row["WLongitude"] != null)
                        {
                            weiboVm.Longitude = decimal.Parse(row["WLongitude"].ToString());
                        }
                        if (row.Table.Columns.Contains("WLatitude") && row["WLatitude"] != null)
                        {
                            weiboVm.Lantitude = decimal.Parse(row["WLatitude"].ToString());
                        }
                        if (row.Table.Columns.Contains("WHeight") && row["WHeight"] != null)
                        {
                            weiboVm.Height = decimal.Parse(row["WHeight"].ToString());
                        }
                        if (row.Table.Columns.Contains("WState") && row["WState"] != null && row["WState"].ToString() != "")
                        {
                            weiboVm.State = (WeiBoStateEnum)int.Parse(row["WState"].ToString());
                        }
                        if (row.Table.Columns.Contains("WHardWareType") && row["WHardWareType"] != null)
                        {
                            weiboVm.HardWareType = row["WHardWareType"].ToString();
                        }
                        if (row.Table.Columns.Contains("WIsRepost") && row["WIsRepost"] != null)
                        {
                            weiboVm.IsRePost = Boolean.Parse(row["WIsRepost"].ToString());
                        }
                        if (row.Table.Columns.Contains("WCreateTime") && row["WCreateTime"] != null && row["WCreateTime"].ToString() != "")
                        {
                            weiboVm.CreateTime = DateTime.Parse(row["WCreateTime"].ToString());
                            weiboVm.PublishTime = TUtil.DateFormatToString(weiboVm.CreateTime);
                        }
                        if (row.Table.Columns.Contains("WUpdateTime") && row["WUpdateTime"] != null && row["WUpdateTime"].ToString() != "")
                        {
                            weiboVm.UpdateTime = DateTime.Parse(row["WUpdateTime"].ToString());
                        }
                        vm.WeiboVm = weiboVm;
                    }
                    #endregion
                    resultList.Add(vm);
                }
            }
            return resultList;
        }
    }
}
