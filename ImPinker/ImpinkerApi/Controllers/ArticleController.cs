﻿using System;
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

namespace ImpinkerApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ArticleController : BaseApiController
    {

        private readonly ArticleBll _articleBll = new ArticleBll();
        readonly string _buckeyName = ConfigurationManager.AppSettings["MyautosOssBucket"];
        readonly string _imgDomain = ConfigurationManager.AppSettings["ImageDomain"];

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
                    item.CoverImage = ImPinkerApi.Common.ImageUrlHelper.GetArticleCoverImage(item.CoverImage, 0);
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
            var articleSnap = new ArticleSnaps
            {
                ArticleId = article.Id,
                Content = article.Content,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            var flagSnap = new ArticleSnapsBll().AddDraft(articleSnap);
            var model = _articleBll.GetModelByCache(article.Id);
            model.State = (int)ArticleStateEnum.Normal;
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
                    files.Add(_imgDomain + imgUrl);
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
                    files.Add(_imgDomain + imgUrl);
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

    }
}
