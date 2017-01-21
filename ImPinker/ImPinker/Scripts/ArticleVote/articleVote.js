//在首页index.js 中，填充数据处绑定
function articleVote(obj,articleId,voteCount) {
        if (!LoginState) {
            alert("请先登录~");
            return;
        }
        $.ajax({
            url: "/ArticleVote/UserVote",
            type: "post",
            async: true,
            data: { articleId: articleId, vote: 1 },
            success: function (data) {
                $(obj).attr("class", "praise-count-ok");
                if (data.IsSuccess == 1) {
                    showTips("谢谢点赞~~~~~", 1500, 1);
                    $(obj).find("b").html(voteCount + 1);
                } else {
                    showTips("您已赞过该篇文章~~", 1500, 2);
                }
                
            },
            error: function (data) {
                showTips("Sorry，系统出错了~", 1500, 0);
            }
        });
   
}