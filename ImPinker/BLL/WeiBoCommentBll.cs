using ImDal;
using ImModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImBLL
{
    public class WeiBoCommentBll
    {
        WeiBoCommentDal _weiboCommentDal = new WeiBoCommentDal();
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
    }
}
