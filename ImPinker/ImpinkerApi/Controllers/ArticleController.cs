using System;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ImBLL;
using ImModel;
using ImpinkerApi.Common;
using ImpinkerApi.Filters;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class ArticleController : BaseApiController
    {

        private readonly ArticleBll _articleBll = new ArticleBll();

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
            if (userinfo!=null&&userinfo.Id>0)
            {
                userid = userinfo.Id;
            }
            var list = _articleBll.GetListByPage(pageNum, pageSize, userid);
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
                Url="",
                Description = "",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                PublishTime = DateTime.Now,
                State = (int) ArticleStateEnum.Draft
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
                Content = HttpUtility.HtmlEncode(article.Content),
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
                Content = HttpUtility.HtmlEncode(article.Content),
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            var flagSnap = new ArticleSnapsBll().AddDraft(articleSnap);
            var model = _articleBll.GetModel(article.Id);
            model.State = (int)ArticleStateEnum.Normal;
            model.PublishTime = DateTime.Now;
            model.UpdateTime = DateTime.Now;
            var flagArticle = _articleBll.Update(model);
            if (flagArticle&&flagSnap)
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

        #endregion 

    }
}
