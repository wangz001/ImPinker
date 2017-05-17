using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using Common.AlyOssUtil;
using ImDal;
using ImModel;
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
        /// 分页获取首页数据列表,coverimage 不为空
        /// </summary>
        public List<ArticleViewModel> GetIndexListByPage(int pageNum, int count)
        {
            var listResult = new List<ArticleViewModel>();
            var ds = _dal.GetIndexListByPage(pageNum, count);
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
                    //keywords只去一个，首页及搜索页显示用
                    var keyStr = model.KeyWords;
                    var keyArr = keyStr.Split(',');
                    if (keyArr.Length > 1)
                    {
                        model.KeyWords = keyArr[1];
                    }
                    else
                    {
                        model.KeyWords = "";
                    }
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
                    }
                    if (row.Table.Columns.Contains("voteCount") && row["voteCount"] != null && row["voteCount"].ToString() != "")
                    {
                        model.VoteCount = int.Parse(row["voteCount"].ToString());
                    }
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

            var vm = new ArticleViewModel
            {
                Id = id.ToString(CultureInfo.InvariantCulture),
                ArticleName = article.ArticleName,
                Url = article.Url,
                Userid = article.UserId.ToString(CultureInfo.InvariantCulture),
                CoverImage = article.CoverImage,
                KeyWords = article.KeyWords,
                Description = article.Description,
                Company = article.Company,
                CreateTime = article.CreateTime,
                Content = snap == null ? null : new List<Object> { snap.Content }
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
        public string UploadArticleCoverImgToOss(string buckeyName, int userid,int articleid, string localFileName)
        {
            var coverimageFormat = ConfigurationManager.AppSettings["ArticleFirstImage"];
            var imgUrl = string.Format(coverimageFormat, DateTime.Now.ToString("yyyyMMdd"), userid, articleid, DateTime.Now.Ticks);
            var extention = Path.GetExtension(localFileName);
            if (extention != null)
            {
                var sLocalPath = localFileName.Replace(extention, "_s.jpg");
                ImageUtils.GetReduceImgFromCenter(360, 240, localFileName, sLocalPath, 85);
                var flag1 = ObjectOperate.UploadImage(buckeyName, sLocalPath, imgUrl, 1024);
                if (flag1)
                {
                    var flag2=UpdateCoverImage(articleid, imgUrl);
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
            var flag1 = ObjectOperate.UploadImage(buckeyName, sourcePath, imgUrl,1024);
            return flag1 ? imgUrl : "";
        }
    }
}

