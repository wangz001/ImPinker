using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Common.AlyOssUtil;
using Common.Utils;
using ImBLL;
using ImModel;
using ImModel.ViewModel;
using ImPinker.Filters;
using ImPinker.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace ImPinker.Controllers
{
    public class ArticleController : Controller
    {
        private static readonly ArticleBll ArticleBll = new ArticleBll();
        private static readonly UserBll UserBll = new UserBll();

        /// <summary>
        /// 文章详情页。爬虫抓取到的文章,即userid=2的文章。用户收藏的文章直接跳转到原始页面
        /// </summary>
        /// <param name="id">文章id：travels_id  或者  id</param>
        /// <returns></returns>
        public ActionResult Index(string id)
        {
            var vm = new ArticleViewModel();
            var idStr = id;
            long idInt = 0;
            if (!id.StartsWith("travels_"))
            {
                idStr = "travels_" + id;
            }
            long.TryParse(idStr.Replace("travels_", ""), out idInt);
            var article = SolrNetSearchBll.GetArticleById(idStr);
            if (article != null && article.Content != null && article.Content.Count > 0)
            {
                vm = article;
            }
            else
            {
                vm = ArticleBll.GetModelWithContent(idInt);
            }
            ViewBag.Article = vm;
            return View("Index");
        }

        /// <summary>
        /// 文章详情页，右侧相关文章推荐
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult RelativeArticle(ArticleViewModel articleViewModel)
        {
            var list = new List<ArticleViewModel>();
            if (!string.IsNullOrEmpty(articleViewModel.KeyWords))
            {
                list = SolrNetSearchBll.QueryByViewTpye("RelativeArticle", articleViewModel.KeyWords, false, 1, 5).ArticleList;
            }
            ViewBag.RelativeArticle = list;
            return PartialView();
        }

        /// <summary>
        /// 我的文章
        /// </summary>
        /// <returns></returns>
        [AuthorizationFilter]
        public ActionResult MyArticle(int ? pageIndex, int ? pageCount)
        {
            int pageNum = (int) (pageIndex > 0 ? pageIndex : 1);
            int pagecount = (int)(pageCount > 0 ? pageCount : 9); ;
            var userId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
            int totalCount;
            var articles = ArticleBll.GetMyListByPage(userId, pageNum, pagecount, out totalCount);
            ViewBag.pageIndex = pageNum;
            ViewBag.pageCount = pagecount;
            ViewBag.totalCount = totalCount;
            ViewBag.Articles = articles;
            return View();
        }

        #region 用户收藏网络连接相关操作
        //
        // 添加收藏
        [AuthorizationFilter]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 添加收藏
        /// </summary>
        /// <param name="createArticleVm"></param>
        /// <returns></returns>
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult Create(CreateArticleViewModel createArticleVm)
        {
            var article = new Article
            {
                Url = createArticleVm.ArticleUrl,
                ArticleName = createArticleVm.ArticleName,
                Description = createArticleVm.Description,
                KeyWords = createArticleVm.KeyWords,
                UserId = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id,
                State = (int)ArticleStateEnum.BeCheck,   //待审核状态
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Company = ""
            };
            //审核通过后添加索引
            var flag = ArticleBll.Add(article);
            return RedirectToAction("MyArticle");

        }

        //
        // GET: /Article/Edit/5
        [AuthorizationFilter]
        public ActionResult Edit(int id)
        {
            return RedirectToAction("Index");
        }

        //
        // POST: /Article/Edit/5
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        //
        // GET: /Article/Delete/5
        [AuthorizationFilter]
        public ActionResult Delete(int id)
        {
            return RedirectToAction("Index");
        }

        //
        // POST: /Article/Delete/5
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region 用户发帖子相关操作
        /// <summary>
        /// 发新帖子
        /// </summary>
        /// <returns></returns>
        [AuthorizationFilter]
        public ActionResult CreateThread()
        {
            return View();
        }

        /// <summary>
        /// 发新帖子
        /// </summary>
        /// <returns></returns>
        [AuthorizationFilter]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateThread(CreateThreadVm model)
        {
            if (string.IsNullOrEmpty(model.Content) || string.IsNullOrEmpty(model.ArticleName))
            {
                AddErrors(IdentityResult.Failed("不能为空"));
                return View(model);
            }
            if (!string.IsNullOrEmpty(model.Coverimage))
            {
                var sourcepath = Server.MapPath("/") + model.Coverimage;
                string ImgUrlformat = "articlefirstimg/{0}/{1}_{2}.jpg";
                var coverimage = string.Format(ImgUrlformat, DateTime.Now.ToString("yyyyMMdd"), UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id, DateTime.Now.Ticks);

                //缩放
                ImageUtils.ThumbnailImage(sourcepath, sourcepath, 360, 240, ImageFormat.Jpeg);
                string buckeyName = "myautos";
                var ossSucess = ObjectOperate.UploadImage(buckeyName, sourcepath, coverimage);
                if (ossSucess)
                {
                    var vm = new CreateThreadVm
                    {
                        ArticleName = model.ArticleName,
                        Content = model.Content,
                        Userid = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id,
                        Coverimage = coverimage,
                        Keywords = "",
                        Description = "",
                        Createtime = DateTime.Now,
                        Updatetime = DateTime.Now,
                        State = ArticleStateEnum.Normal
                    };
                    var flag = ArticleBll.AddThread(vm);
                    if (flag)
                    {
                        return RedirectToAction("MyArticle");
                    }
                }
            }
            AddErrors(IdentityResult.Failed("有错误"));
            return View(model);
        }

        /// <summary>
        /// 修改帖子
        /// </summary>
        /// <returns></returns>
        [AuthorizationFilter]
        public ActionResult UpdateThread(int articleId)
        {
            var userid = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
            var article = ArticleBll.GetModelByCache(articleId);
            var snap = new ArticleSnapsBll().GetModel(articleId);
            if (article != null && article.UserId == userid)
            {
                ViewBag.ArticleContent = snap.Content.Replace("\"", "'");
                ViewBag.Article = article;
                return View();
            }
            ViewBag.Article = null;
            ViewBag.ArticleContent = "";
            return View();
        }

        /// <summary>
        /// 修改帖子
        /// </summary>
        /// <returns></returns>
        [AuthorizationFilter]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateThread(CreateThreadVm model)
        {
            var userid = UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id;
            
            if (string.IsNullOrEmpty(model.Content) || string.IsNullOrEmpty(model.ArticleName))
            {
                AddErrors(IdentityResult.Failed("不能为空"));
                return View(model);
            }
            if (!string.IsNullOrEmpty(model.Coverimage))
            {
                var coverimage = model.Coverimage;
                if (!model.Coverimage.StartsWith("articlefirstimg/"))
                {
                    var sourcepath = Server.MapPath("/") + model.Coverimage;
                    string ImgUrlformat = "articlefirstimg/{0}/{1}_{2}.jpg";
                    coverimage = string.Format(ImgUrlformat, DateTime.Now.ToString("yyyyMMdd"), UserBll.GetModelByAspNetId(User.Identity.GetUserId()).Id, DateTime.Now.Ticks);

                    //缩放
                    ImageUtils.ThumbnailImage(sourcepath, sourcepath, 360, 240, ImageFormat.Jpeg);
                    string buckeyName = "myautos";
                    var ossSucess = ObjectOperate.UploadImage(buckeyName, sourcepath, coverimage);
                }
                var article = ArticleBll.GetModelByCache(model.ArticleId);
                var vm = new CreateThreadVm
                   {
                       ArticleId = model.ArticleId,
                       ArticleName = model.ArticleName,
                       Content = model.Content,
                       Userid = userid,
                       Coverimage = coverimage,
                       Keywords = "",
                       Description = "",
                       Createtime = article.CreateTime,
                       Updatetime = DateTime.Now,
                       State = ArticleStateEnum.Normal
                   };
                var flag = ArticleBll.UpdateThread(vm);
                if (flag)
                {
                    return RedirectToAction("MyArticle");
                }
            }
            AddErrors(IdentityResult.Failed("有错误"));
            return View(model);
        }

        /// <summary>
        /// 删除帖子
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        [AuthorizationFilter]
        [HttpGet]
        public ActionResult DeleteThread(long articleId)
        {
            var userinfo = UserBll.GetModelByAspNetId(User.Identity.GetUserId());
            var flag = ArticleBll.DeleteThread(userinfo.Id,articleId);
            return RedirectToAction("Index","UserCenter");
        }
        

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }



        /// <summary>
        /// ueeditor 插件初始化，获取配置，上传等
        /// </summary>
        [AuthorizationFilter]
        public void InitUeEditor()
        {
            Handler action = null;
            var context = System.Web.HttpContext.Current;
            var aa = context.Request["action"];
            switch (aa)
            {
                case "config":
                    action = new ConfigHandler(context);
                    break;
                case "uploadimage":
                    action = new UploadHandler(context, new UploadConfig()
                    {
                        AllowExtensions = Config.GetStringList("imageAllowFiles"),
                        PathFormat = Config.GetString("imagePathFormat"),
                        SizeLimit = Config.GetInt("imageMaxSize"),
                        UploadFieldName = Config.GetString("imageFieldName")
                    });
                    break;
                case "uploadscrawl":
                    action = new UploadHandler(context, new UploadConfig()
                    {
                        AllowExtensions = new string[] { ".png" },
                        PathFormat = Config.GetString("scrawlPathFormat"),
                        SizeLimit = Config.GetInt("scrawlMaxSize"),
                        UploadFieldName = Config.GetString("scrawlFieldName"),
                        Base64 = true,
                        Base64Filename = "scrawl.png"
                    });
                    break;
                case "uploadvideo":
                    action = new UploadHandler(context, new UploadConfig()
                    {
                        AllowExtensions = Config.GetStringList("videoAllowFiles"),
                        PathFormat = Config.GetString("videoPathFormat"),
                        SizeLimit = Config.GetInt("videoMaxSize"),
                        UploadFieldName = Config.GetString("videoFieldName")
                    });
                    break;
                case "uploadfile":
                    action = new UploadHandler(context, new UploadConfig()
                    {
                        AllowExtensions = Config.GetStringList("fileAllowFiles"),
                        PathFormat = Config.GetString("filePathFormat"),
                        SizeLimit = Config.GetInt("fileMaxSize"),
                        UploadFieldName = Config.GetString("fileFieldName")
                    });
                    break;
                case "listimage":
                    action = new ListFileManager(context, Config.GetString("imageManagerListPath"), Config.GetStringList("imageManagerAllowFiles"));
                    break;
                case "listfile":
                    action = new ListFileManager(context, Config.GetString("fileManagerListPath"), Config.GetStringList("fileManagerAllowFiles"));
                    break;
                case "catchimage":
                    action = new CrawlerHandler(context);
                    break;
                default:
                    action = new NotSupportedHandler(context);
                    break;
            }
            action.Process();
        }

        #endregion


        public int pageindex { get; set; }
    }
}
