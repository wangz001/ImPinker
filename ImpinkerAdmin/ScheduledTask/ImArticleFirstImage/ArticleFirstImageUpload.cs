using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Configuration;
using Common.AlyOssUtil;
using Common.Logging;
using Common.Utils;
using ImBLL;
using ImModel.ViewModel;
using Quartz;
using Quartz.Impl;
using ImModel;

namespace GetCarDataService.ImArticleFirstImage
{
    /// <summary>
    /// 根据articlesnap表的firstimageurl，生成缩略图，上传到oss，并更新到article主表（同时更新索引）
    /// </summary>
    [DisallowConcurrentExecution]
    public class ArticleFirstImageUpload : IJob
    {
        static ArticleBll articleBll = new ArticleBll();
        static ArticleSnapsBll articlesnapBll = new ArticleSnapsBll();
        private const string buckeyName = "myautos";
        const string ImgUrlformat = "articlefirstimg/{0}/{1}_{2}.jpg";
        public void Execute(IJobExecutionContext context)
        {
            Common.WriteInfoLog("生成封面图服务启动");
            try
            {
                Start();
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("生成封面图服务exception:" + e);
            }
            Common.WriteInfoLog("生成封面图服务结束");
        }
        public void Start()
        {
            var solrIndexList = new List<ArticleViewModel>();//准备添加索引的集合
            var articleList = articleBll.GetArticlesWithoutCoverImage();
            foreach (var article in articleList)
            {
                var flag=UpdateArticleImage(article,ref solrIndexList);
            }
            //更新索引
            SolrNetSearchBll.AddIndex(solrIndexList);
            Common.WriteInfoLog("本次处理文章数：" + solrIndexList.Count);
            Console.WriteLine("本次处理文章数：" + solrIndexList.Count);
        }

        /// <summary>
        /// 修改文章的封面图
        /// </summary>
        /// <param name="article"></param>
        /// <param name="solrIndexList">待创建solr索引的article集合</param>
        /// <returns></returns>
        public static bool UpdateArticleImage(Article article,ref List<ArticleViewModel> solrIndexList)
        {
            //标志生成封面图是否成功
            var isSuccess = false;
            try
            {
                var articleSnap = articlesnapBll.GetModel(article.Id);
                if (articleSnap != null && !string.IsNullOrEmpty(articleSnap.FirstImageUrl))
                {
                    var firstimageUrl = articleSnap.FirstImageUrl;
                    var imgurl = string.Format(ImgUrlformat, DateTime.Now.ToString("yyyyMMdd"), article.Id, DateTime.Now.Ticks);
                    var flag = UploadToOss(firstimageUrl, imgurl);
                    if (flag)
                    {
                        var updateflag = articleBll.UpdateCoverImage(article.Id, imgurl);
                        if (!updateflag)
                        {
                            Common.WriteErrorLog("生成封面图错误：" + imgurl);
                            return false;
                        }
                        var vm = new ArticleViewModel
                        {
                            Id = "travels_" + article.Id,
                            ArticleName = article.ArticleName,
                            Company = article.Company,
                            CoverImage = imgurl,
                            KeyWords = article.KeyWords,
                            Description = article.Description,
                            Content = new List<object> { articleSnap.Content },
                            Url = article.Url,
                            CreateTime = article.CreateTime,
                            UpdateTime = article.UpdateTime,
                            Userid = article.UserId.ToString()
                        };
                        solrIndexList.Add(vm);
                        isSuccess = true;
                    }
                    else
                    {
                        Common.WriteErrorLog("上传图片到oss错误：" + imgurl);
                    }
                }
                if (!isSuccess)
                {
                    //生成封面图失败，将article状态设置为待审核，后期由人工操作，上传封面图
                    articleBll.UpdateState(article.Id, ArticleStateEnum.BeCheck);
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
                Common.WriteErrorLog("生成封面图服务exception:" + e);
            }
            return isSuccess;
        }

        /// <summary>
        /// 生成360*240小图，上传到oss
        /// </summary>
        /// <param name="imgUrl"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool UploadToOss(string imgUrl, string key)
        {
            if (string.IsNullOrEmpty(imgUrl))
            {
                return false;
            }
            try
            {
                var tempdir = string.Format(AppDomain.CurrentDomain.BaseDirectory + "\\Upload\\temp_{0}.jpg", DateTime.Now.Ticks);
                const int width = 360;
                const int height = 240;
                //下载
                Uri myUri = new Uri(imgUrl);
                WebRequest webRequest = WebRequest.Create(myUri);
                WebResponse webResponse = webRequest.GetResponse();
                Bitmap myImage = new Bitmap(webResponse.GetResponseStream());
                MemoryStream ms = new MemoryStream();
                myImage.Save(ms, ImageFormat.Jpeg);
                if (!Directory.Exists(Path.GetDirectoryName(tempdir)))//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(tempdir));
                }
                myImage.Save(tempdir);
                //剪切(如果高大于1.5倍宽，剪切)
                if (1.5 * (myImage.Width) < myImage.Height)
                {
                    ImageUtils.ImgReduceCutOut(0, 0, myImage.Width, myImage.Width * 2 / 3, tempdir, tempdir);
                }
                //缩放
                ImageUtils.ThumbnailImage(tempdir, tempdir, width, height, ImageFormat.Jpeg);
                //保存
                bool flag = ObjectOperate.UploadImage(buckeyName, tempdir, key);
                return flag;
            }
            catch (Exception e)
            {
                Common.WriteErrorLog("生成封面图错误：" + key + ".Exception: " + e);
                return false;
            }
        }
    }
}
