using ImDal;
using ImModel;
using System;
using System.Collections.Generic;
using System.Linq;
using ImModel.ViewModel;

namespace ImBLL
{
    public class WeiBoCommentBll
    {
        readonly WeiBoCommentDal _weiboCommentDal = new WeiBoCommentDal();
        private readonly UserBll _userBll = new UserBll();
        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="weiboId"></param>
        /// <param name="commentStr"></param>
        /// <param name="toWeiboCommentId"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool AddComment(long weiboId, string commentStr, long toWeiboCommentId, int userid)
        {
            var model = new WeiBoComment
            {
                WeiBoId = weiboId,
                UserId = userid,
                ContentText = commentStr,
                ToCommentId = toWeiboCommentId,
                State = 1,
                CreateTime = DateTime.Now
            };
            var falg = _weiboCommentDal.AddComment(model);
            return falg;
        }
        /// <summary>
        /// 获取微博评论列表
        /// </summary>
        /// <param name="weiboid"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<WeiboCommentVm> GetList(int weiboid,int page,int pageSize)
        {
            var returnList = new List<WeiboCommentVm>();
            List<WeiBoComment> commentList= _weiboCommentDal.GetList(weiboid,page,pageSize);
            var toCommentIds = commentList.Select(m => m.ToCommentId);
            var listToComment = _weiboCommentDal.GetList(toCommentIds.ToList());
            foreach (var comment in commentList)
            {
                var userinfo = _userBll.GetModelByCache(comment.UserId);
                var vm = new WeiboCommentVm
                {
                    Id=comment.Id,
                    WeiBoId = comment.WeiBoId,
                    UserId = comment.UserId,
                    ContentText = comment.ContentText,
                    ToCommentId = comment.ToCommentId,
                    CreateTime = comment.CreateTime,
                    UserName = string.IsNullOrEmpty(userinfo.ShowName)? userinfo.UserName:userinfo.ShowName,
                    HeadImage =ImageUrlHelper.GetHeadImageUrl(userinfo.ImgUrl, 100) 
                };
                if (vm.ToCommentId>0)
                {
                    var tocomment = listToComment.FirstOrDefault(m => m.Id == vm.ToCommentId);
                    if (tocomment != null)
                    {
                        var touserinfo = _userBll.GetModelByCache(tocomment.UserId);
                        var tocommentVm = new WeiboCommentVm
                        {
                            Id = tocomment.Id,
                            WeiBoId = tocomment.WeiBoId,
                            UserId = tocomment.UserId,
                            ContentText = tocomment.ContentText,
                            ToCommentId = tocomment.ToCommentId,
                            CreateTime = tocomment.CreateTime,
                            UserName = string.IsNullOrEmpty(touserinfo.ShowName) ? touserinfo.UserName : touserinfo.ShowName,
                            HeadImage = ImageUrlHelper.GetHeadImageUrl(touserinfo.ImgUrl, 100) 
                        };
                        vm.ToCommentItemVm = tocommentVm;
                    }
                }
                returnList.Add(vm);
            }
            return returnList;
        }
    }
}
