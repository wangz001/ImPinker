using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using Common.DateTimeUtil;
using ImDal;
using ImModel;
using ImModel.ViewModel;
using Maticsoft.Common;

namespace ImBLL
{
    /// <summary>
    /// Article
    /// </summary>
    public class ArticleBll
    {
        private readonly ArticleDal dal = new ArticleDal();
        private readonly ArticleSnapsBll articleSnapsBll = new ArticleSnapsBll();

        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(long Id)
        {
            return dal.Exists(Id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public bool Add(Article model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Article model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(long Id)
        {
            return dal.Delete(Id);
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool DeleteList(string Idlist)
        {
            return dal.DeleteList(Idlist);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Article GetModel(long Id)
        {
            return dal.GetModel(Id);
        }

        /// <summary>
        /// 得到一个对象实体，从缓存中
        /// </summary>
        public Article GetModelByCache(long Id)
        {
            string CacheKey = "ArticleModel-" + Id;
            object objModel = DataCache.GetCache(CacheKey);
            if (objModel == null)
            {
                try
                {
                    objModel = dal.GetModel(Id);
                    if (objModel != null)
                    {
                        int ModelCache = ConfigHelper.GetConfigInt("ModelCache");
                        DataCache.SetCache(CacheKey, objModel, DateTime.Now.AddMinutes(ModelCache), TimeSpan.Zero);
                    }
                }
                catch { }
            }
            return (Article)objModel;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            return dal.GetList(strWhere);
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<Article> GetModelList(string strWhere)
        {
            DataSet ds = dal.GetList(strWhere);
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
                    Article model = dal.DataRowToModel(dt.Rows[n]);
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
            return dal.GetRecordCount(strWhere);
        }
        /// <summary>
        /// 分页获取用户数据列表
        /// </summary>
        public DataSet GetMyListByPage(int userid, int pageNum, int count)
        {
            return dal.GetMyListByPage(userid, pageNum, count);
        }

        /// <summary>
        /// 分页获取首页数据列表,coverimage 不为空
        /// </summary>
        public List<ArticleViewModel> GetIndexListByPage(int pageNum, int count)
        {
            var imgDomain = ConfigurationManager.AppSettings["ArtilceCoverImageDomain"];
            var articleNameLists = new List<string>();
            var listResult = new List<ArticleViewModel>();
            var ds = dal.GetIndexListByPage(pageNum, count);
            List<Article> articles = DataTableToList(ds.Tables[0]);
            if (articles != null && articles.Count > 0)
            {
                foreach (var article in articles)
                {
                    if (article.ArticleName.Length > 25)
                    {
                        article.ArticleName = article.ArticleName.Substring(0, 25) + "……";
                    }
                    if (articleNameLists.Contains(article.ArticleName))
                    {//去除标题重复的数据,解决fblife 同一文章发在不同域名的问题
                        continue;
                    }
                    if (string.IsNullOrEmpty(article.CoverImage))
                    {
                        continue; //无图的不要
                    }
                    articleNameLists.Add(article.ArticleName);

                    //keywords只去一个，首页及搜索页显示用
                    var keyStr = article.KeyWords;
                    var keyArr = keyStr.Split(',');
                    if (keyArr.Length > 1)
                    {
                        article.KeyWords = keyArr[1];
                    }
                    else
                    {
                        article.KeyWords = "";
                    }
                    listResult.Add(new ArticleViewModel()
                    {
                        Id = article.Id.ToString(),
                        ArticleName = article.ArticleName,
                        Url = article.Url,
                        Description = article.Description,
                        KeyWords = article.KeyWords,
                        CoverImage = imgDomain + article.CoverImage,
                        Company = article.Company,
                        CreateTime = article.CreateTime,
                        CreateTimeStr = TUtil.DateFormatToString(article.CreateTime)
                    });
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
                return dal.Update(article);
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
            var article = dal.GetModel(id);
            var snap = articleSnapsBll.GetModel(id);
            if (article==null||snap==null)
            {
                return null;
            }
            var vm = new ArticleViewModel()
            {
                Id = id.ToString(),
                ArticleName = article.ArticleName,
                Url = article.Url,
                Userid = article.UserId.ToString(),
                CoverImage = article.CoverImage,
                KeyWords = article.KeyWords,
                Description = article.Description,
                Company = article.Company,
                CreateTime = article.CreateTime,
                Content = new List<Object>() { snap.Content }
            };
            return vm;
        }
        /// <summary>
        /// 发布新帖子。操作article和articlesnap表，使用事务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddThread(CreateThreadVm vm)
        {
            return dal.AddThread(vm);
        }

        public bool UpdateThread(CreateThreadVm vm)
        {
            return dal.UpdateThread(vm);
        }
    }
}

