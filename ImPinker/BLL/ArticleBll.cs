﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using Common.AlyOssUtil;
using Common.DateTimeUtil;
using ImDal;
using ImModel;
using ImModel.Enum;
using ImModel.ViewModel;
using Maticsoft.Common;
using Common.Utils;
using System.IO;

namespace ImBLL
{
    /// <summary>
    /// Article
    /// </summary>
    public class ArticleBll
    {
        private readonly ArticleDal _dal = new ArticleDal();
        private readonly ArticleSnapsBll _articleSnapsBll = new ArticleSnapsBll();
        private readonly UserBll _userBll = new UserBll();
        private readonly ArticleVoteDal _articleVoteDal = new ArticleVoteDal();
        private readonly ArticleCommentDal _articleCommentDal = new ArticleCommentDal();

        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(long id)
        {
            return _dal.Exists(id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Article model)
        {
            return _dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Article model)
        {
            return _dal.Update(model);
        }

        /// <summary>
        /// 用户删除帖子
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteThread(int userid, long id)
        {
            return _dal.DeleteThread(userid, id);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Article GetModelByCache(long id)
        {
            string cacheKey = "ArticleModel-" + id;
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
            return (Article)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return _dal.GetList(strWhere);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Article> GetModelList(string strWhere)
        {
            DataSet ds = _dal.GetList(strWhere);
            return DataTableToList(ds.Tables[0]);
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Article> DataTableToList(DataTable dt)
        {
            var modelList = new List<Article>();
            int rowsCount = dt.Rows.Count;
            if (rowsCount > 0)
            {
                for (int n = 0; n < rowsCount; n++)
                {
                    Article model = _dal.DataRowToModel(dt.Rows[n]);
                    if (model != null)
                    {
                        modelList.Add(model);
                    }
                }
            }
            return modelList;
        }


        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            return _dal.GetRecordCount(strWhere);
        }
        /// <summary>
        /// 分页获取用户数据列表,包括已发布的,待审核,审核未通过d 
        /// </summary>
        public List<Article> GetMyListByPage(int userid, int pageNum, int count, out int totalaCount)
        {
            totalaCount = 0;
            var ds = _dal.GetMyListByPage(userid, pageNum, count);
            var list = new List<Article>();
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                list = DataTableToList(ds.Tables[0]);
            }
            if (ds != null && ds.Tables[1] != null)
            {
                int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out totalaCount);
            }
            return list;
        }
        /// <summary>
        /// 根据文章状态获取列表
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pageNum"></param>
        /// <param name="count"></param>
        /// <param name="stateEnum"></param>
        /// <param name="totalaCount"></param>
        /// <returns></returns>
        public List<ArticleViewModel> GetUsersListByState(int userid, int pageNum, int count, ArticleStateEnum stateEnum, out int totalaCount)
        {
            totalaCount = 0;
            var ds = _dal.GetMyListByState(userid, pageNum, count, stateEnum);
            var resultList = DsToArticleVm(ds);
            if (ds != null && ds.Tables[1] != null)
            {
                int.TryParse(ds.Tables[1].Rows[0][0].ToString(), out totalaCount);
            }
            return resultList;
        }

        /// <summary>
        /// 分页获取首页数据列表,coverimage 不为空
        /// </summary>
        public List<ArticleViewModel> GetIndexListByPage(int pageNum, int count)
        {
            var ds = _dal.GetIndexListByPage(pageNum, count);
            var listResult = DsToArticleVm(ds);
            return listResult;
        }

        /// <summary>
        /// 模型转换，带评论数和点赞数
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        private List<ArticleViewModel> DsToArticleVm(DataSet ds)
        {
            var listResult = new List<ArticleViewModel>();
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    var model = new ArticleViewModel();
                    if (row.Table.Columns.Contains("Id") && row["Id"] != null && row["Id"].ToString() != "")
                    {
                        model.Id = row["Id"].ToString();
                    }
                    if (row.Table.Columns.Contains("ArticleName") && row["ArticleName"] != null)
                    {
                        model.ArticleName = row["ArticleName"].ToString();
                    }
                    if (row.Table.Columns.Contains("Description") && row["Description"] != null)
                    {
                        model.Description = row["Description"].ToString();
                    }
                    if (model.ArticleName.Length > 25)
                    {
                        model.ArticleName = model.ArticleName.Substring(0, 25) + "……";
                    }
                    if (row.Table.Columns.Contains("Url") && row["Url"] != null)
                    {
                        model.Url = row["Url"].ToString();
                    }
                    if (row.Table.Columns.Contains("CoverImage") && row["CoverImage"] != null)
                    {
                        model.CoverImage = row["CoverImage"].ToString();
                    }
                    if (row.Table.Columns.Contains("KeyWords") && row["KeyWords"] != null)
                    {
                        model.KeyWords = row["KeyWords"].ToString();
                    }
                    ////keywords只去一个，首页及搜索页显示用
                    //var keyStr = model.KeyWords;
                    //var keyArr = keyStr.Split(',');
                    //if (keyArr.Length > 1)
                    //{
                    //    model.KeyWords = keyArr[1];
                    //}
                    //else
                    //{
                    //    model.KeyWords = "";
                    //}
                    if (row.Table.Columns.Contains("CreateTime") && row["CreateTime"] != null && row["CreateTime"].ToString() != "")
                    {
                        model.CreateTime = DateTime.Parse(row["CreateTime"].ToString());
                    }
                    if (row.Table.Columns.Contains("Company") && row["Company"] != null && row["Company"].ToString() != "")
                    {
                        model.Company = row["Company"].ToString();
                    }
                    if (row.Table.Columns.Contains("UserId") && row["UserId"] != null && row["UserId"].ToString() != "")
                    {
                        model.Userid = row["UserId"].ToString();
                        var useritem = _userBll.GetModelByCache(int.Parse(model.Userid));
                        model.UserName = string.IsNullOrEmpty(useritem.ShowName) ? useritem.UserName : useritem.ShowName;
                    }
                    if (row.Table.Columns.Contains("VoteCount") && row["VoteCount"] != null && row["VoteCount"].ToString() != "")
                    {
                        model.VoteCount = int.Parse(row["VoteCount"].ToString());
                    }
                    if (row.Table.Columns.Contains("CommentCount") && row["CommentCount"] != null && row["CommentCount"].ToString() != "")
                    {
                        model.CommentCount = int.Parse(row["CommentCount"].ToString());
                    }
                    //点赞数和浏览数------暂时处理。后期改为不从db中直接查询
                    listResult.Add(model);
                }
            }

            return listResult;
        }

        #endregion  BasicMethod

        /// <summary>
        /// 更新封面图地址
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="newUrl"></param>
        /// <returns></returns>
        public bool UpdateCoverImage(long articleId, string newUrl)
        {
            var article = GetModelByCache(articleId);
            if (article != null)
            {
                article.CoverImage = newUrl;
                article.UpdateTime = DateTime.Now;
                return _dal.Update(article);
            }
            return false;
        }
        /// <summary>
        /// 获取所有还未生成封面图oss的记录
        /// </summary>
        /// <returns></returns>
        public List<Article> GetArticlesWithoutCoverImage()
        {
            const string whereStr = " State =1 AND CoverImage IS NULL OR DATALENGTH(CoverImage)=0 ";
            var list = GetModelList(whereStr);
            return list;
        }

        /// <summary>
        /// 获取model，包含snap表的content
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArticleViewModel GetModelWithContent(long id)
        {
            var article = _dal.GetModel(id);
            var snap = _articleSnapsBll.GetModel(id);
            var userinfo = _userBll.GetModelByCache(article.UserId);
            //暂用。以后改为从缓存读取
            int voteCount = _articleVoteDal.GetArticleVoteCount(id,true);
            int commentCount = _articleCommentDal.GetArticleCommentCount(id);
            var vm = new ArticleViewModel
            {
                Id = id.ToString(CultureInfo.InvariantCulture),
                ArticleName = article.ArticleName,
                Url = article.Url,
                Userid = article.UserId.ToString(CultureInfo.InvariantCulture),
                CoverImage = ImageUrlHelper.GetArticleImage(article.CoverImage, 360),
                KeyWords = article.KeyWords,
                Description = article.Description,
                Company = article.Company,
                CreateTime = article.CreateTime,
                Content = snap == null ? null : snap.Content,
                UserName = string.IsNullOrEmpty(userinfo.ShowName) ? userinfo.UserName : userinfo.ShowName,
                UserHeadUrl = ImageUrlHelper.GetHeadImageUrl(userinfo.ImgUrl, 100),
                CommentCount=commentCount,
                VoteCount=voteCount
            };
            return vm;
        }

        /// <summary>
        /// 发布新帖子。操作article和articlesnap表，使用事务
        /// </summary>
        /// <returns></returns>
        public bool AddThread(CreateThreadVm vm)
        {
            return _dal.AddThread(vm);
        }

        public bool UpdateThread(CreateThreadVm vm)
        {
            return _dal.UpdateThread(vm);
        }

        /// <summary>
        /// 修改文章状态
        /// </summary>
        public bool UpdateState(long articleId, ArticleStateEnum articleState)
        {
            var article = GetModelByCache(articleId);
            article.State = (int)articleState;
            return Update(article);
        }

        /// <summary>
        /// 获取首页文章列表
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageCount"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<ArticleViewModel> GetListByPage(int pageNum, int pageCount, int? userid)
        {
            //如果是新用户，则推荐热门文章；老用户，则根据用户兴趣标签，智能推荐
            var list = new List<ArticleViewModel>();
            if (userid > 0)
            {
                //根据用户兴趣标签，智能推荐
            }
            else
            {
                list = GetIndexListByPage(pageNum, pageCount);
            }
            return list;
        }

        /// <summary>
        /// 上传游记封面图
        /// </summary>
        /// <param name="buckeyName"></param>
        /// <param name="userid"></param>
        /// <param name="articleid"></param>
        /// <param name="localFileName"></param>
        /// <returns></returns>
        public string UploadArticleCoverImgToOss(string buckeyName, int userid, int articleid, string localFileName)
        {
            var coverimageFormat = ConfigurationManager.AppSettings["ArticleFirstImage"];
            var imgUrl = string.Format(coverimageFormat, DateTime.Now.ToString("yyyyMMdd"), userid, articleid, DateTime.Now.Ticks);
            var extention = Path.GetExtension(localFileName);
            if (extention != null)
            {
                var sLocalPath = localFileName.Replace(extention, "_s.jpg");
                ImageUtils.GetReduceImgFromCenter(900, 600, localFileName, sLocalPath, 85);
                var flag1 = ObjectOperate.UploadImage(buckeyName, sLocalPath, imgUrl, 1024);
                if (flag1)
                {
                    var flag2 = UpdateCoverImage(articleid, imgUrl);
                    return flag2 ? imgUrl : "";
                }
            }
            return "";
        }

        /// <summary>
        /// 编写游记时，上传图片，保存到oss。返回图片路径插入到文章中
        /// </summary>
        /// <param name="buckeyName"></param>
        /// <param name="userid">用户id</param>
        /// <param name="articleid">文章id</param>
        /// <param name="sourcePath">本地文件路径</param>
        /// <returns></returns>
        public string UploadArticleImgToOss(string buckeyName, int userid, int articleid, string sourcePath)
        {
            if (string.IsNullOrEmpty(buckeyName) || userid == 0 || articleid == 0)
            {
                return "";
            }
            string imgUrlformat = ConfigurationManager.AppSettings["ArticleImage"];
            var imgUrl = string.Format(imgUrlformat, DateTime.Now.ToString("yyyyMMdd"), userid, articleid, DateTime.Now.Ticks);
            var flag1 = ObjectOperate.UploadImage(buckeyName, sourcePath, imgUrl, 1024);
            return flag1 ? imgUrl : "";
        }

        /// <summary>
        /// 根据时间倒叙获取用户的微博和article列表
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public List<ArticleWeiboVm> GetUserArticleAndWeiboListByPage(int userid, int pageindex, int pagesize)
        {
            var resultList = new List<ArticleWeiboVm>();
            var ds = _dal.GetUserArticleAndWeiboListByPage(userid, pageindex, pagesize);
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

