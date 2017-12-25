using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Common.Utils;
using ImBLL;
using ImModel;
using ImpinkerApi.Common;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;
using Common.DateTimeUtil;

namespace ImpinkerApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ArticleController : BaseApiController
    {

        private readonly ArticleBll _articleBll = new ArticleBll();
        readonly string _buckeyName = ConfigurationManager.AppSettings["MyautosOssBucket"];
        #region 获取首页文章列表
        /// <summary>
        /// 获取首页轮播图文章（暂时获取最新的）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSlideArticle()
        {
            var list = _articleBll.GetIndexListByPage(1, 3);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    item.CoverImage = ImageUrlHelper.GetArticleImage(item.CoverImage, 900);
                }
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = list,
                    Description = "ok"
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "没有更多数据"
            });
        }
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="pageNum"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetByPage(int pageNum, int pageSize)
        {
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var userid = 0;
            if (userinfo != null && userinfo.Id > 0)
            {
                userid = userinfo.Id;
            }
            List<ImModel.ViewModel.ArticleViewModel> list = _articleBll.GetListByPage(pageNum, pageSize, userid);
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    item.CreateTimeStr = TUtil.DateFormatToString(item.CreateTime);
                    item.CoverImage = ImageUrlHelper.GetArticleImage(item.CoverImage, 360);
                }
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = list,
                    Description = "ok"
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "没有更多数据"
            });
        }

        #endregion

        #region 获取文章详情
        [HttpGet]
        public HttpResponseMessage GetArticleWithContent(int articleid)
        {
            var article = _articleBll.GetModelWithContent(articleid);
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = article,
                Description = "获取成功"
            });
        }
        #endregion

        #region 发布游记
        /// <summary>
        /// 新建游记
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TokenCheck]
        public HttpResponseMessage NewArticle([FromBody]ArticleViewModel article)
        {
            if (string.IsNullOrEmpty(article.ArticleName))
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Data = null,
                    Description = "名称不能为空"
                });
            }
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var model = new Article
            {
                ArticleName = article.ArticleName,
                UserId = userinfo.Id,
                Url = "",
                Description = "",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                PublishTime = DateTime.Now,
                State = (int)ArticleStateEnum.Draft
            };
            int articleId = _articleBll.Add(model);
            model.Id = articleId;
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = model,
                Description = "添加成功，已保存为草稿"
            });
        }
        /// <summary>
        /// 修改文章名称
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        [HttpPost]
        [TokenCheck]
        public HttpResponseMessage UpdateArticleTitle([FromBody]ArticleViewModel article)
        {
            if (string.IsNullOrEmpty(article.ArticleName))
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Data = null,
                    Description = "名称不能为空"
                });
            }
            var entity = _articleBll.GetModelByCache(article.Id);
            if (entity != null)
            {
                entity.ArticleName = article.ArticleName;
                entity.UpdateTime = DateTime.Now;
                var flag = _articleBll.Update(entity);
                if (flag)
                {
                    return GetJson(new JsonResultViewModel
                    {
                        IsSuccess = 1,
                        Data = article.ArticleName,
                        Description = "自动保存成功"
                    });
                }
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "保存出错"
            });
        }

        /// <summary>
        /// 保存草稿
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TokenCheck]
        public HttpResponseMessage SaveDraft([FromBody]ArticleViewModel article)
        {
            if (string.IsNullOrEmpty(article.Content))
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Data = null,
                    Description = "内容不能为空"
                });
            }
            var articleSnap = new ArticleSnaps
            {
                ArticleId = article.Id,
                Content = article.Content,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            var flag = new ArticleSnapsBll().AddDraft(articleSnap);
            if (flag)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = articleSnap,
                    Description = "自动保存成功"
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "保存出错"
            });
        }



        /// <summary>
        /// 发布游记
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TokenCheck]
        public HttpResponseMessage PublishArticle([FromBody]ArticleViewModel article)
        {
            //保存content
            var articleSnap = new ArticleSnaps
            {
                ArticleId = article.Id,
                Content = article.Content,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            var flagSnap = new ArticleSnapsBll().AddDraft(articleSnap);
            //保存article 基本信息
            var model = _articleBll.GetModelByCache(article.Id);
            model.State = (int)ArticleStateEnum.Normal;
            model.ArticleName = article.ArticleName;
            model.Description = article.Description;
            model.PublishTime = DateTime.Now;
            model.UpdateTime = DateTime.Now;
            var flagArticle = _articleBll.Update(model);
            if (flagArticle && flagSnap)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Data = model,
                    Description = "文章发布成功"
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "文章发布失败"
            });
        }

        /// <summary>
        /// 设置游记封面图
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TokenCheck]
        public async Task<HttpResponseMessage> SetCoverImage()
        {
            // 检查是否是 multipart/form-data 
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Description = "数据格式错误",
                    Data = HttpStatusCode.UnsupportedMediaType
                });
            }
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/ImageUpload/articlecoverimage/" + DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(fileSaveLocation))
            {
                Directory.CreateDirectory(fileSaveLocation);
            }
            var provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                //封装数据
                var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
                int articleid = 0;
                //取articleid
                var formData = provider.FormData;
                if (formData.HasKeys()
                    && formData.AllKeys.Contains("articleid")
                    && !string.IsNullOrEmpty(formData.Get("articleid")))
                {
                    Int32.TryParse(formData.Get("articleid"), out articleid);
                }
                if (articleid == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                    {
                        IsSuccess = 0,
                        Description = "获取文章id失败",
                        Data = ""
                    });
                }
                var files = new List<string>();
                foreach (MultipartFileData file in provider.FileData)
                {
                    //上传封面图到oss
                    string imgUrl = _articleBll.UploadArticleCoverImgToOss(_buckeyName, userinfo.Id, articleid, file.LocalFileName);
                    if (string.IsNullOrEmpty(imgUrl))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                        {
                            IsSuccess = 0,
                            Description = "上传封面图图片到oss失败",
                            Data = file.LocalFileName
                        });
                    }
                    files.Add(ImageUrlHelper.GetArticleImage(imgUrl, 360));
                    break;
                }
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Description = "ok",
                    Data = string.Join(",", files)
                });
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(e.ToString());
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Description = "error",
                    Data = ""
                });
            }
        }

        /// <summary>
        /// 上传游记图片，可多张上传
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [TokenCheck]
        public async Task<HttpResponseMessage> UploadArticleImage()
        {
            // 检查是否是 multipart/form-data 
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Description = "数据格式错误",
                    Data = HttpStatusCode.UnsupportedMediaType
                });
            }
            string fileSaveLocation = HttpContext.Current.Server.MapPath("~/ImageUpload/articleimage/" + DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(fileSaveLocation))
            {
                Directory.CreateDirectory(fileSaveLocation);
            }
            var provider = new CustomMultipartFormDataStreamProvider(fileSaveLocation);
            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
                //封装数据
                var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
                int articleid;
                //取articleid
                var formData = provider.FormData;
                if (formData.HasKeys()
                    && formData.AllKeys.Contains("articleid")
                    && !string.IsNullOrEmpty(formData.Get("articleid")))
                {
                    Int32.TryParse(formData.Get("articleid"), out articleid);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                    {
                        IsSuccess = 0,
                        Description = "获取文章id失败",
                        Data = ""
                    });
                }
                var files = new List<string>();
                foreach (MultipartFileData file in provider.FileData)
                {
                    string imgUrl = _articleBll.UploadArticleImgToOss(_buckeyName, userinfo.Id, articleid, file.LocalFileName);
                    if (string.IsNullOrEmpty(imgUrl))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                        {
                            IsSuccess = 0,
                            Description = "上传图片到oss失败",
                            Data = file.LocalFileName
                        });
                    }
                    files.Add(ImageUrlHelper.GetArticleImage(imgUrl, 900));
                }
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Description = "ok",
                    Data = string.Join(",", files)
                });
            }
            catch (Exception e)
            {
                LogHelper.Instance.Error(e.ToString());
                return Request.CreateResponse(HttpStatusCode.OK, new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Description = "上传游记图片出错",
                    Data = ""
                });
            }
        }
        #endregion

        #region 获取草稿列表

        [HttpPost]
        [TokenCheck]
        public HttpResponseMessage GetMyDraft()
        {
            const int pageNum = 1;
            const int pageSize = 30;
            int totalCount;
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var articles = _articleBll.GetUsersListByState(userinfo.Id, pageNum, pageSize, ArticleStateEnum.Draft,
                out totalCount);
            foreach (var item in articles)
            {
                item.CoverImage = ImageUrlHelper.GetArticleImage(item.CoverImage, 360);
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = articles,
                Description = "获取草稿成功"
            });
        }

        #endregion

        #region 我的文章列表

        [HttpGet]
        [TokenCheck]
        public HttpResponseMessage GetMyArticle(int pageNum, int pageSize)
        {
            int totalCount;
            var userinfo = TokenHelper.GetUserInfoByHeader(Request.Headers);
            var articles = _articleBll.GetUsersListByState(userinfo.Id, pageNum, pageSize, ArticleStateEnum.Normal, out totalCount);
            foreach (var item in articles)
            {
                item.CoverImage = ImageUrlHelper.GetArticleImage(item.CoverImage, 360);
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = articles,
                Description = "获取草稿成功"
            });
        }

        #endregion

        #region 删除文章

        [HttpPost]
        [TokenCheck]
        public HttpResponseMessage DeleteArticle([FromBody]ArticleViewModel article)
        {
            var entity = _articleBll.GetModelByCache(article.Id);
            if (entity != null)
            {
                entity.State = (int)ArticleStateEnum.Deleted;
                entity.UpdateTime = DateTime.Now;
                var flag = _articleBll.Update(entity);
                if (flag)
                {
                    return GetJson(new JsonResultViewModel
                    {
                        IsSuccess = 1,
                        Data = article.Id,
                        Description = "文章已删除"
                    });
                }
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Data = null,
                Description = "删除失败"
            });
        }

        #endregion

        #region 获取用户的文章列表

        public HttpResponseMessage GetUsersArticleByPage(int userid, int pageindex, int pagesize)
        {
            int totalCount;
            pageindex = pagesize > 0 ? pageindex : 1;
            pagesize = (pagesize > 0 && pagesize < 50) ? pagesize : 10;
            var articles = _articleBll.GetUsersListByState(userid, pageindex, pagesize, ArticleStateEnum.Normal, out totalCount);
            if (articles == null || articles.Count == 0)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Data = null,
                    Description = "没有更多数据"
                });
            }
            foreach (var item in articles)
            {
                item.CoverImage = ImageUrlHelper.GetArticleImage(item.CoverImage, 360);
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = articles,
                Description = "获取草稿成功"
            });
        }

        #endregion

        #region 时间倒叙获取用户文章和微博列表

        public HttpResponseMessage GetUserArticleAndWeiboListByPage(int userid, int pageindex, int pagesize)
        {
            pageindex = pagesize > 0 ? pageindex : 1;
            pagesize = (pagesize > 0 && pagesize < 50) ? pagesize : 10;
            var articles = _articleBll.GetUserArticleAndWeiboListByPage(userid, pageindex, pagesize);
            

            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 1,
                Data = articles,
                Description = "获取草稿成功"
            });
        }


        #endregion
    }
}
