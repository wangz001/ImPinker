using System.Collections.Generic;
using System.Linq;
using ImDal;
using ImModel;
using ImModel.Enum;
using ImModel.ViewModel;
using SolrNet.Utils;

namespace ImBLL
{
    public class ArticleCommentBll
    {
        private readonly ArticleCommentDal _dal=new ArticleCommentDal();
        private readonly UserBll _userBll=new UserBll();
        readonly NotifyBll _notifyBll = new NotifyBll();
        public bool Add(ArticleComment model)
        {
            var flag= _dal.Add(model);
            if (!flag)
            {
                return false;
            }
            //添加通知
            flag = _notifyBll.NewNotify(NotifyTypeEnum.Remind, (int)model.ArticleId, TargetTypeEnum.Article, ActionEnum.Comment, model.UserId, model.Content);
            return flag;
        }
        /// <summary>
        /// 分页获取文章评论
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="rowNum"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<ArticleComment> GetListsByArticleId(long articleId,int rowNum,int count)
        {
            int totalcount;
            var ds= _dal.GetListsByArticleId(articleId,rowNum,count,out totalcount);
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                return _dal.DtToList(ds.Tables[0]);
            }
            return null;
        }

        /// <summary>
        /// 分页获取文章评论，带有引用的评论
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="pageNum"></param>
        /// <param name="count"></param>
        /// <param name="totalCount">总记录数，分页用</param>
        /// <returns></returns>
        public List<ArticleCommentVm> GetCommentsWithToComments(long articleId, int pageNum, int count,out int totalCount)
        {
            var returnList = new List<ArticleCommentVm>();
            var ds = _dal.GetListsByArticleId(articleId, pageNum, count,out totalCount);
            var listToCommentVm = new List<ArticleCommentVm>();
            if (ds != null && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            {
                List<ArticleComment>  listToComment = _dal.DtToList(ds.Tables[1]);
                foreach (ArticleComment comment in listToComment)
                {
                    var userinfo = _userBll.GetModelByCache(comment.UserId);
                    var vm = new ArticleCommentVm
                    {
                        Id = comment.Id,
                        ArticleId = comment.ArticleId,
                        UserId = comment.UserId,
                        ToCommentId = comment.ToCommentId,
                        Content = comment.Content,
                        CreateTime = comment.CreateTime,
                        UserName = string.IsNullOrEmpty(userinfo.ShowName) ? userinfo.UserName : userinfo.ShowName,
                        HeadImage =ImageUrlHelper.GetHeadImageUrl( userinfo.ImgUrl, 100)
                    };
                    listToCommentVm.Add(vm);
                }
            }
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                var list = _dal.DtToList(ds.Tables[0]);
                foreach (ArticleComment comment in list)
                {
                    var userinfo = _userBll.GetModelByCache(comment.UserId);
                    var model = new ArticleCommentVm
                    {
                        Id = comment.Id,
                        ArticleId = comment.ArticleId,
                        UserId = comment.UserId,
                        ToCommentId = comment.ToCommentId,
                        Content = comment.Content,
                        CreateTime = comment.CreateTime,
                        UserName = string.IsNullOrEmpty(userinfo.ShowName) ? userinfo.UserName : userinfo.ShowName,
                        HeadImage = ImageUrlHelper.GetHeadImageUrl(userinfo.ImgUrl, 100)
                    };
                    model.ListToComment = new List<ArticleCommentVm> { listToCommentVm.FirstOrDefault(m => m.Id == model.ToCommentId) };
                    returnList.Add(model);
                }
            }
            
            return returnList;
        }
        
    }
}
