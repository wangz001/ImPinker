//在articlecomment.js 中，填充数据处绑定
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
            } else {
                alert("您已经评价过该评论~");
            }
        },
        error: function (data) {
            alert(data);
        }
    });
}