//在articlecomment.js 中，填充数据处绑定
function bindArticleCommentVote() {
    $(".comment-list .up-btn").bind("click", function () {
        if (!LoginState) {
            alert("请先登录~");
            return;
        }
        
        var commentId = $(this).attr("commentid");
        $.ajax({
            url: "/ArticleVote/ArticleCommentVote",
            type: "post",
            data: { commentId: commentId, vote: 1 },
            success: function (data) {
                alert("点赞成功");
            },
            error: function (data) {
                alert(data);
            }
        });
    });
}