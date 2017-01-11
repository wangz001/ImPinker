//对评论的点赞方法
function ArticleCommentVote(obj,commentId,count) {
    if (!LoginState) {
        alert("请先登录~");
        return;
    }
    $.ajax({
        url: "/ArticleVote/ArticleCommentVote",
        type: "post",
        data: { commentId: commentId, vote: 1 },
        async:true,
        success: function (data) {
            if (data.IsSuccess == "1") {
                $(obj).find("em").html(count + 1);
                VoteAnimate(obj);
                showTips("ok，点赞成功~", 1500, 1);
            } else {
                showTips("您已经评价过该评论~",1500,2);
            }
        },
        error: function (data) {
            showTips("Sorry，出错咯~", 1500,0);
        }
    });
}